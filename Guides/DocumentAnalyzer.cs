/* ****************************************************************************
 * Copyright 2013 Steve Dower
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy 
 * of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 * ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IndentGuide.Utils;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace IndentGuide {
    [Flags]
    public enum LineSpanType {
        Normal = 0,
        PageWidthMarker = 1,
        
        NeedsTextAtEnd = 2
    }

    [DebuggerDisplay("Indent {Indent} lines {FirstLine}-{LastLine}, {Type}")]
    public sealed class LineSpan : IEquatable<LineSpan> {
        public int FirstLine;
        public int LastLine;
        public int Indent;
        public LineSpanType Type;
        public bool Changed;
        public int FormatIndex;
        public bool Highlight;
        private HashSet<LineSpan> _LinkedLines;
        
        public LineSpan(int first, int last, int indent, LineSpanType type) {
            FirstLine = first;
            LastLine = last;
            Indent = indent;
            Type = type;
            Changed = true;
            FormatIndex = 0;
            Highlight = false;
        }

        public IEnumerable<LineSpan> LinkedLines {
            get {
                if (_LinkedLines == null) {
                    return Enumerable.Empty<LineSpan>();
                } else {
                    return _LinkedLines.AsEnumerable();
                }
            }
        }

        private static HashSet<LineSpan> LinkedLinesInternal(LineSpan line, CancellationToken cancel) {
#if PERFORMANCE
            object cookie = null;
            try {
                PerformanceLogger.Start(ref cookie);
                return LinkedLinesInternal_Performance(line, cancel);
            } catch (OperationCanceledException) {
                PerformanceLogger.Mark("Cancel");
                throw;
            } finally {
                PerformanceLogger.End(cookie);
            }
        }

        private static HashSet<LineSpan> LinkedLinesInternal_Performance(LineSpan line, CancellationToken cancel) {
#endif
            var result = new HashSet<LineSpan>();
            var queue = new Queue<LineSpan>();
            queue.Enqueue(line);
            while (queue.Any()) {
                var ls = queue.Dequeue();
                if (result.Add(ls) && ls._LinkedLines != null) {
                    foreach (var ls2 in ls._LinkedLines) {
                        cancel.ThrowIfCancellationRequested();

                        queue.Enqueue(ls2);
                    }
                }
            }

            return result;
        }

        public static void Link(LineSpan existingLine, LineSpan newLine, CancellationToken cancel) {
            foreach (var line in LinkedLinesInternal(existingLine, cancel)) {
                if (line._LinkedLines == null) {
                    line._LinkedLines = new HashSet<LineSpan> { newLine };
                } else {
                    line._LinkedLines.Add(newLine);
                }

                if (newLine._LinkedLines == null) {
                    newLine._LinkedLines = new HashSet<LineSpan> { line };
                } else {
                    newLine._LinkedLines.Add(line);
                }
            }
        }

        public static void Unlink(LineSpan removeLine, CancellationToken cancel) {
            foreach (var line in LinkedLinesInternal(removeLine, cancel)) {
                if (line._LinkedLines != null) {
                    line._LinkedLines.Remove(removeLine);
                }

                if (removeLine._LinkedLines != null) {
                    removeLine._LinkedLines.Remove(line);
                }
            }
        }

        public static void MoveLinks(LineSpan fromLine, LineSpan toLine, CancellationToken cancel) {
            var links = LinkedLinesInternal(fromLine, cancel);
            links.Remove(fromLine);

            foreach (var line in links) {
                if (line._LinkedLines != null) {
                    line._LinkedLines.Remove(fromLine);
                }

                if (fromLine._LinkedLines != null) {
                    fromLine._LinkedLines.Remove(line);
                }
            }

            foreach (var line in links) {
                if (line._LinkedLines == null) {
                    line._LinkedLines = new HashSet<LineSpan> { toLine };
                } else {
                    line._LinkedLines.Add(toLine);
                }

                if (toLine._LinkedLines == null) {
                    toLine._LinkedLines = new HashSet<LineSpan>();
                } else {
                    toLine._LinkedLines.Add(line);
                }
            }
        }

        public bool Equals(LineSpan other) {
            if (other == null) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return FirstLine == other.FirstLine &&
                LastLine == other.LastLine &&
                Indent == other.Indent &&
                Type == other.Type;
        }

        public override bool Equals(object obj) {
            return Equals(obj as LineSpan);
        }

        public override int GetHashCode() {
            return new { FirstLine, LastLine, Indent, Type }.GetHashCode();
        }
    }

    public class DocumentAnalyzer {
        private CancellationTokenSource CurrentCancel;
        private readonly SemaphoreSlim ResetMutex = new SemaphoreSlim(1, 1);
        private readonly List<LineSpanChunk> Chunks = new List<LineSpanChunk>();

        public readonly ITextSnapshot OriginalSnapshot;
        public readonly LineBehavior Behavior;
        public readonly int IndentSize;
        public readonly int TabSize;

        private readonly int ChunkSize;

        public int LongestLine { get; private set; }

        public ITextSnapshot Snapshot { get; private set; }

        public DocumentAnalyzer(
            ITextSnapshot snapshot,
            LineBehavior behavior,
            int indentSize,
            int tabSize,
            int chunkSize = 30
        ) {
            OriginalSnapshot = snapshot;
            LongestLine = 0;
            Behavior = behavior.Clone();
            IndentSize = indentSize;
            TabSize = tabSize;
            ChunkSize = chunkSize;
        }

        public IEnumerable<LineSpan> GetLines(int firstLine, int lastLine) {
            var intersection = new List<LineSpanChunk>();
            lock (Chunks) {
                int start = 0;
                foreach (var c in Chunks) {
                    int end = start + c.TextLineCount;
                    if (lastLine >= start && firstLine <= end) {
                        intersection.Add(c);
                    }
                    start = end;
                }
            }
            return intersection.SelectMany(c => c.Lines).Where(ls => ls.Type == LineSpanType.Normal).Distinct();
        }

        public IEnumerable<LineSpan> GetAllLines() {
            var chunks = new List<LineSpanChunk>();
            lock (Chunks) {
                chunks = Chunks.ToList();
            }
            return chunks.SelectMany(c => c.Lines).Where(ls => ls.Type == LineSpanType.Normal).Distinct();
        }

        public async Task Reset() {
            var cts = new CancellationTokenSource();
            var cancel = cts.Token;
            var cts2 = Interlocked.Exchange(ref CurrentCancel, cts);
            if (cts2 != null) {
                cts2.Cancel();
                cts2.Dispose();
            }

            await ResetMutex.WaitAsync();

            try {
                // We need to collect infomation about lines on the UI thread.
                var snapshot = OriginalSnapshot.TextBuffer.CurrentSnapshot;
                var chunkInfo = new List<LineInfo[]>(snapshot.LineCount / ChunkSize + 1);

                int lineCount = snapshot.LineCount;
                for (int lineNumber = 0; lineNumber < lineCount; lineNumber += ChunkSize) {
                    int chunkSize = lineCount - lineNumber;
                    if (chunkSize > ChunkSize) {
                        chunkSize = ChunkSize;
                    }

                    var lineInfo = new LineInfo[chunkSize];
                    SetLineInfo(
                        lineInfo,
                        snapshot,
                        lineNumber,
                        lineNumber + chunkSize - 1,
                        cancel
                    );
                    chunkInfo.Add(lineInfo);
                }

                ValidateChunks(chunkInfo);

                await Task.Run(() => {
                    var lineSpans = GetLineSpans(chunkInfo, cancel);

                    lock (Chunks) {
                        Chunks.Clear();
                        Chunks.AddRange(lineSpans);
                    }
                });

                Snapshot = snapshot;
            } finally {
                ResetMutex.Release();
            }
        }

        public async Task Update(TextViewLayoutChangedEventArgs changes) {
            if (Snapshot != OriginalSnapshot.TextBuffer.CurrentSnapshot) {
                await Reset();
            }
        }


        private void SetLineInfo(
            LineInfo[] lineInfo,
            ITextSnapshot snapshot,
            int firstLine,
            int lastLine,
            CancellationToken cancel
        ) {
#if PERFORMANCE
            object cookie = null;
            try {
                PerformanceLogger.Start(ref cookie);
                SetLineInfo_Performance(lineInfo, snapshot, firstLine, lastLine, cancel);
            } catch (OperationCanceledException) {
                PerformanceLogger.Mark("Cancel");
                throw;
            } finally {
                PerformanceLogger.End(cookie);
            }
        }

        private void SetLineInfo_Performance(
            LineInfo[] lineInfo,
            ITextSnapshot snapshot,
            int firstLine,
            int lastLine,
            CancellationToken cancel
        ) {
#endif
            var contentType = snapshot.ContentType != null ? snapshot.ContentType.TypeName : null;
            bool isCSharp = string.Equals(contentType, "csharp", StringComparison.OrdinalIgnoreCase);
            bool isCPlusPlus = string.Equals(contentType, "c/c++", StringComparison.OrdinalIgnoreCase);

            for (int lineNumber = firstLine; lineNumber <= lastLine; ++lineNumber) {
                cancel.ThrowIfCancellationRequested();

                var line = snapshot.GetLineFromLineNumber(lineNumber);

                var curLine = lineInfo[lineNumber - firstLine];
                curLine.Number = lineNumber;
                curLine.HasText = false;
                curLine.TextAt = 0;

                var text = line.GetText();
                if (string.IsNullOrEmpty(text)) {
                    lineInfo[lineNumber - firstLine] = curLine;
                    continue;
                }

                var normalizedLength = text.ActualLength(TabSize);
                if (normalizedLength > LongestLine) {
                    LongestLine = normalizedLength;
                }

                bool allWhitespace;
                int spaces = text.LeadingWhitespace(TabSize, out allWhitespace);
                curLine.HasText = !allWhitespace;
                curLine.TextAt = spaces;

                if (isCSharp || isCPlusPlus) {
                    // Left-aligned pragmas don't reduce the indent to zero.
                    if (spaces == 0 && text.StartsWith("#")) {
                        curLine.SkipLine = true;
                    }
                }

                lineInfo[lineNumber - firstLine] = curLine;
            }

            cancel.ThrowIfCancellationRequested();
        }

        [Conditional("DEBUG")]
        private void ValidateChunks(IEnumerable<LineInfo[]> chunks) {
            // Verify that line numbers have been set correctly
            uint lineNumber = 0;
            foreach (var line in chunks.SelectMany(i => i)) {
                Debug.Assert(line.Number == lineNumber);
                lineNumber += 1;
            }
        }

        private List<LineSpanChunk> GetLineSpans(IList<LineInfo[]> chunkInfo, CancellationToken cancel) {
#if PERFORMANCE
            object cookie = null;
            try {
                PerformanceLogger.Start(ref cookie);
                return GetLineSpans_Performance(chunkInfo, cancel);
            } catch (OperationCanceledException) {
                PerformanceLogger.Mark("Cancel");
                throw;
            } finally {
                PerformanceLogger.End(cookie);
            }
        }

        private List<LineSpanChunk> GetLineSpans_Performance(IList<LineInfo[]> chunkInfo, CancellationToken cancel) {
#endif
            int lineCount = 0, lastLineCount = 0;
            var result = new List<LineSpanChunk>(chunkInfo.Count);
            var builder = new LineSpanBuilder(IndentSize, Behavior);
            foreach (var chunk in chunkInfo) {
                foreach (var line in chunk) {
                    cancel.ThrowIfCancellationRequested();

                    builder.AddNextLine(line);
                    lineCount += 1;
                }
                result.Add(new LineSpanChunk(builder.GetLines(), lineCount - lastLineCount));
                lastLineCount = lineCount;
            }
            builder.NoMoreLines();
            result.Add(new LineSpanChunk(builder.GetLines(), lineCount - lastLineCount));
            return result;
        }


        #region LineInfo structure

        private struct LineInfo {
            private uint _packedValue;
            private const uint _numberMask = 0x000FFFFFu;
            private const uint _hasTextMask = 0x80000000u;
            private const uint _skipLineMask = 0x40000000u;
            private const uint _textAtMask = 0x3FF00000u;
            private const int _textAtShift = 20;
            private const uint _textAtValueMask = _textAtMask >> _textAtShift;

            public int Number {
                get { return (int)(_packedValue & _numberMask); }
                set {
                    if (value < 0 || ((uint)value & ~_numberMask) != 0) {
                        throw new ArgumentOutOfRangeException(
                            string.Format("Value {0} is too large for LineInfo.Number", value)
                        );
                    }
                    _packedValue = (_packedValue & ~_numberMask) | ((uint)value & _numberMask);
                }
            }

            public bool HasText {
                get { return (_packedValue & _hasTextMask) != 0; }
                set {
                    if (value) {
                        _packedValue = _packedValue | _hasTextMask;
                    } else {
                        _packedValue = _packedValue & ~_hasTextMask;
                    }
                }
            }

            public bool SkipLine {
                get { return (_packedValue & _skipLineMask) != 0; }
                set {
                    if (value) {
                        _packedValue = _packedValue | _skipLineMask;
                    } else {
                        _packedValue = _packedValue & ~_hasTextMask;
                    }
                }
            }

            public int TextAt {
                get {
                    return (int)((_packedValue & _textAtMask) >> _textAtShift);
                }
                set {
                    if (value < 0 || ((uint)value & ~_textAtValueMask) != 0) {
                        throw new ArgumentOutOfRangeException(
                            string.Format("Value {0} is too large for LineInfo.TextAt", value)
                        );
                    }
                    _packedValue = (_packedValue & ~_textAtMask) | (((uint)value & _textAtValueMask) << _textAtShift);
                }
            }

            public override string ToString() {
                return string.Format("Line {0}{1}{2}",
                    Number,
                    HasText ? string.Format(" text at {0}", TextAt) : "",
                    SkipLine ? " (skip)" : ""
                );
            }
        }

        #endregion

        #region LineSpanChunk struct

        private class LineSpanChunk {
            private readonly ICollection<LineSpan> _lines;
            private int _textLineCount;

            public LineSpanChunk(ICollection<LineSpan> lines, int textLineCount) {
                _lines = lines;
                _textLineCount = textLineCount;
            }

            public ICollection<LineSpan> Lines {
                get { return _lines; }
            }

            public int TextLineCount {
                get { return _textLineCount; }
                set { _textLineCount = value; }
            }
        }

        #endregion

        #region LineSpanBuilder class

        class LineSpanBuilder {
            private readonly int _indentSize;
            private readonly LineBehavior _options;

            private LineInfo _previousLine;
            private bool _previousSkipLine;
            private int _lastTextToEmptyLine;
            private int _lastEmptyToTextLine;

            private List<LineSpan> _completedSpans;
            private readonly List<LineSpan> _activeSpans;
            
            public LineSpanBuilder(int indentSize, LineBehavior options) {
                _indentSize = indentSize;
                _options = options;

                _activeSpans = new List<LineSpan>();
                _completedSpans = new List<LineSpan>();
            }

            private int GetFormatIndex(int indent) {
                if (indent % _indentSize == 0) {
                    return indent / _indentSize;
                } else {
                    return LineFormat.UnalignedFormatIndex;
                }
            }

            public ICollection<LineSpan> GetLines() {
                var res = _completedSpans;
                _completedSpans = new List<LineSpan>();

                for (int i = 0; i < res.Count; ++i) {
                    res[i].Type = LineSpanType.Normal;
                    res[i].FormatIndex = GetFormatIndex(res[i].Indent);
                }

                res.Capacity += _activeSpans.Count;
                for (int i = 0; i < _activeSpans.Count; ++i) {
                    var span = _activeSpans[i];
                    if (span != null) {
                        span.Type = LineSpanType.Normal;
                        span.FormatIndex = GetFormatIndex(span.Indent);
                        res.Add(span);
                    }
                }
                return res;
            }

            public void AddNextLine(LineInfo line) {
                if (line.Number == _previousLine.Number) {
                    _previousLine = line;
                    return;
                }
                Debug.Assert(line.Number == _previousLine.Number + 1);
                if (line.SkipLine) {
                    if (!_previousSkipLine) {
                        FromNoSkipToSkip(line.Number);
                    } else {
                        FromSkipToSkip(line.Number);
                    }
                    _previousSkipLine = true;
                    _previousLine.Number = line.Number;
                    return;
                } else if (_previousSkipLine) {
                    FromSkipToNoSkip(line.Number, line.HasText, line.TextAt);
                }
                _previousSkipLine = false;

                if (line.HasText && !_previousLine.HasText) {
                    FromEmptyToText(line.Number, line.TextAt);
                } else if (!line.HasText && _previousLine.HasText) {
                    FromTextToEmpty(line.Number);
                } else if (line.HasText && _previousLine.HasText) {
                    FromTextToText(line.Number, line.TextAt);
                }
                _previousLine = line;
            }

            public void NoMoreLines() {
                if (!_options.ExtendInwardsOnly) {
                    foreach (var span in _activeSpans) {
                        if (span == null) {
                            continue;
                        }

                        span.LastLine = _previousLine.Number;
                    }
                }
            }

            private readonly IndentSet[] _setCache = new IndentSet[129];

            private IndentSet GetIndents(int textAt) {
                if (textAt > 128) {
                    textAt = 128;
                }

                var set = _setCache[textAt];
                if (set.Any()) {
                    return set;
                }
                
                if (_options.VisibleAligned) {
                    for (int i = 0; i < textAt; i += _indentSize) {
                        set.Set(i);
                    }
                }
                if (_options.VisibleUnaligned || (textAt % _indentSize) == 0) {
                    set.Set(textAt);
                }

                _setCache[textAt] = set;
                return set;
            }

            private LineSpan StartSpan(int line, int indent, LineSpanType lineType) {
                var span = new LineSpan(line, line, indent, lineType);
                for (int i = 0; i < _activeSpans.Count; ++i) {
                    if (_activeSpans[i] == null) {
                        _activeSpans[i] = span;
                        return span;
                    }
                }
                _activeSpans.Add(span);
                return span;
            }

            private void StartSpans(
                IndentSet indents,
                int line,
                int textAt,
                LineSpanType lineType = LineSpanType.Normal
            ) {
                foreach (var indent in indents.GetAll()) {
                    var firstLine = line;
                    bool extendToTop = _options.VisibleEmpty &&
                        !_options.ExtendInwardsOnly &&
                        _lastEmptyToTextLine == 0 &&
                        line > 0;
                    if (extendToTop) {
                        firstLine = 0;
                    }

                    if (indent == textAt) {
                        if (_options.VisibleAtTextEnd) {
                            var span = StartSpan(firstLine, indent, lineType);
                            span.LastLine = line;
                        }
                    } else if (indent < textAt) {
                        var span = StartSpan(firstLine, indent, lineType);
                        span.LastLine = line;
                    }
                }
            }

            private void FromEmptyToText(int line, int textAt) {
                var indents = GetIndents(textAt);
                
                if (!_options.VisibleEmpty) {
                    Debug.Assert(!_activeSpans.Any());
                    StartSpans(indents, line, textAt);
                    return;
                }

                for (int i = 0; i < _activeSpans.Count; ++i) {
                    var span = _activeSpans[i];
                    if (span == null) {
                        continue;
                    }

                    if (indents.Remove(span.Indent)) {
                        if (span.Indent == textAt) {
                            span.LastLine = line - 1;
                            _activeSpans[i] = null;
                            _completedSpans.Add(span);
                        } else {
                            span.LastLine = line;
                        }
                    } else if (span.Indent == textAt) {
                        span.LastLine = line - 1;
                        _activeSpans[i] = null;
                        _completedSpans.Add(span);
                    } else if (span.Type.HasFlag(LineSpanType.NeedsTextAtEnd)) {
                        _activeSpans[i] = null;
                    } else {
                        if (!_options.ExtendInwardsOnly) {
                            span.LastLine = line - 1;
                        }
                        _activeSpans[i] = null;
                        _completedSpans.Add(span);
                    }
                }

                StartSpans(indents, line, textAt);

                if (_lastEmptyToTextLine == 0 && !_options.ExtendInwardsOnly && _options.VisibleEmpty) {
                    foreach (var span in _activeSpans) {
                        if (span == null) {
                            continue;
                        }
                        indents.Remove(span.Indent);
                    }
                    foreach (var indent in indents.GetAll()) {
                        if (indent < textAt || (indent == textAt && _options.VisibleEmptyAtEnd)) {
                            _completedSpans.Add(new LineSpan(0, line - 1, indent, LineSpanType.Normal));
                        }
                    }
                }

                _lastEmptyToTextLine = line;
            }

            private void FromTextToEmpty(int line) {
                if (!_options.VisibleEmpty) {
                    foreach (var span in _activeSpans) {
                        if (span == null) {
                            continue;
                        }

                        span.LastLine = line - 1;
                        if (span.LastLine >= span.FirstLine) {
                            _completedSpans.Add(span);
                        }
                    }
                    _activeSpans.Clear();
                    return;
                }

                var indents = GetIndents(_previousLine.TextAt);
                for (int i = 0; i < _activeSpans.Count; ++i) {
                    var span = _activeSpans[i];
                    if (span == null) {
                        continue;
                    }

                    if (indents.Remove(span.Indent)) {
                        if (!_options.ExtendInwardsOnly) {
                            span.LastLine = line;
                        }
                    }
                }

                StartSpans(
                    indents,
                    line,
                    int.MaxValue,
                    _options.ExtendInwardsOnly ? LineSpanType.NeedsTextAtEnd : LineSpanType.Normal
                );

                _lastTextToEmptyLine = line;
            }

            private void FromTextToText(int line, int textAt) {
                var indents = GetIndents(textAt);

                for (int i = 0; i < _activeSpans.Count; ++i) {
                    var span = _activeSpans[i];
                    if (span == null) {
                        continue;
                    }

                    if (indents.Remove(span.Indent)) {
                        if (span.Indent == textAt) {
                            _activeSpans[i] = null;
                            _completedSpans.Add(span);
                        } else {
                            span.LastLine = line;
                        }
                    } else {
                        _activeSpans[i] = null;
                        if (span.LastLine >= span.FirstLine) {
                            _completedSpans.Add(span);
                        }
                    }
                }

                StartSpans(indents, line, textAt);
            }


            private void FromNoSkipToSkip(int line) {
                for (int i = 0; i < _activeSpans.Count; ++i) {
                    var span = _activeSpans[i];
                    if (span == null) {
                        continue;
                    }

                    span.LastLine = line - 1;
                    _completedSpans.Add(span);
                    var newSpan = new LineSpan(line, line, span.Indent, span.Type);
                    LineSpan.Link(span, newSpan, CancellationToken.None);
                    _activeSpans[i] = newSpan;
                }
            }

            private void FromSkipToSkip(int line) {
            }

            private void FromSkipToNoSkip(int line, bool hasText, int textAt) {
                for (int i = 0; i < _activeSpans.Count; ++i) {
                    var span = _activeSpans[i];
                    if (span == null) {
                        continue;
                    }

                    bool remove = false;

                    if (hasText) {
                        if (span.Indent > textAt || (span.Indent == textAt && !_options.VisibleAtTextEnd)) {
                            remove = true;
                        }
                    } else if (!_options.VisibleEmpty) {
                        remove = true;
                    }

                    if (remove) {
                        LineSpan.Unlink(span, CancellationToken.None);
                        _activeSpans[i] = null;
                    } else {
                        span.FirstLine = span.LastLine = line;
                    }
                }

                if (!hasText && _options.VisibleEmpty) {
                    var indents = GetIndents(_previousLine.TextAt);
                    for (int i = 0; i < _activeSpans.Count; ++i) {
                        var span = _activeSpans[i];
                        if (span == null) {
                            continue;
                        }

                        if (indents.Remove(span.Indent)) {
                            span.FirstLine = span.LastLine = line;
                        }
                    }
                    StartSpans(indents, line, int.MaxValue);
                }
            }
        }

        #endregion

        #region IndentSet class

        internal struct IndentSet {
            public static readonly IndentSet Empty = new IndentSet();

            private static readonly ulong[] Masks =
                Enumerable.Range(0, 64).Select(i => 1ul << i).ToArray();
            private const ulong H1 = 0x00000000FFFFFFFFul;
            private const ulong H2 = 0xFFFFFFFF00000000ul;

            private const ulong Q1 = 0x000000000000FFFFul;
            private const ulong Q2 = 0x00000000FFFF0000ul;
            private const ulong Q3 = 0x0000FFFF00000000ul;
            private const ulong Q4 = 0xFFFF000000000000ul;

            private const int Q1Start = 0, Q1Stop = 16;
            private const int Q2Start = 16, Q2Stop = 32;
            private const int Q3Start = 32, Q3Stop = 48;
            private const int Q4Start = 48, Q4Stop = 64;

            private const int Count = 64;


            private ulong _value1, _value2;

            public bool Any() {
                return _value1 != 0 || _value2 != 0;
            }

            public void Set(int indent) {
                if (indent < Count) {
                    _value1 |= Masks[indent];
                } else if (indent < Count * 2) {
                    _value2 |= Masks[indent - Count];
                }
            }

            public bool Remove(int indent) {
                bool res = false;
                if (indent < Count) {
                    var m = Masks[indent];
                    res = (_value1 & m) != 0;
                    _value1 &= ~m;
                } else if (indent < Count * 2) {
                    var m = Masks[indent - Count];
                    res = (_value2 & m) != 0;
                    _value2 &= ~m;
                }
                return res;
            }

            public IEnumerable<int> GetAll() {
                var v = _value1;
                ulong m;
                if (v != 0) {
                    if ((v & H1) != 0) {
                        m = Q1;
                        for (int i = Q1Start; (v & m) != 0 && i < Q1Stop; ++i, m <<= 1) {
                            if ((v & Masks[i]) != 0) {
                                yield return i;
                            }
                        }
                        m = Q2;
                        for (int i = Q2Start; (v & m) != 0 && i < Q2Stop; ++i, m <<= 1) {
                            if ((v & Masks[i]) != 0) {
                                yield return i;
                            }
                        }
                    }
                    if ((v & H2) != 0) {
                        m = Q3;
                        for (int i = Q3Start; (v & m) != 0 && i < Q3Stop; ++i, m <<= 1) {
                            if ((v & Masks[i]) != 0) {
                                yield return i;
                            }
                        }
                        m = Q4;
                        for (int i = Q4Start; (v & m) != 0 && i < Q4Stop; ++i, m <<= 1) {
                            if ((v & Masks[i]) != 0) {
                                yield return i;
                            }
                        }
                    }
                }
                v = _value2;
                if (v != 0) {
                    if ((v & H1) != 0) {
                        m = Q1;
                        for (int i = Q1Start; (v & m) != 0 && i < Q1Stop; ++i, m <<= 1) {
                            if ((v & Masks[i]) != 0) {
                                yield return i + Count;
                            }
                        }
                        m = Q2;
                        for (int i = Q2Start; (v & m) != 0 && i < Q2Stop; ++i, m <<= 1) {
                            if ((v & Masks[i]) != 0) {
                                yield return i + Count;
                            }
                        }
                    }
                    if ((v & H2) != 0) {
                        m = Q3;
                        for (int i = Q3Start; (v & m) != 0 && i < Q3Stop; ++i, m <<= 1) {
                            if ((v & Masks[i]) != 0) {
                                yield return i + Count;
                            }
                        }
                        m = Q4;
                        for (int i = Q4Start; (v & m) != 0 && i < Q4Stop; ++i, m <<= 1) {
                            if ((v & Masks[i]) != 0) {
                                yield return i + Count;
                            }
                        }
                    }
                }
            }

            internal string Dump() {
                return string.Format("{0:X016} {1:X016}", _value2, _value1);
            }
        }

        #endregion
    }
}
