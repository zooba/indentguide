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
using System.Threading;
using System.Threading.Tasks;
using IndentGuide.Utils;
using Microsoft.VisualStudio.Text;

namespace IndentGuide {
    public enum LineSpanType {
        None,
        Normal,
        PageWidthMarker
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
        private HashSet<LineSpan> _linkedLines;
        
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
                if (_linkedLines == null) {
                    return Enumerable.Empty<LineSpan>();
                } else {
                    return _linkedLines.AsEnumerable();
                }
            }
        }

        private static IEnumerable<LineSpan> LinkedLinesInternal(LineSpan line) {
            object perfCookie = null;
            PerformanceLogger.Start(ref perfCookie);

            var result = new HashSet<LineSpan>();
            var queue = new Queue<LineSpan>();
            queue.Enqueue(line);
            while (queue.Any()) {
                var ls = queue.Dequeue();
                if (result.Add(ls)) {
                    if (ls._linkedLines != null) {
                        foreach (var ls2 in ls._linkedLines) {
                            queue.Enqueue(ls2);
                        }
                    }
                }
            }

            PerformanceLogger.End(perfCookie);

            return result;
        }

        public static void Link(LineSpan existingLine, LineSpan newLine) {
            foreach (var line in LinkedLinesInternal(existingLine)) {
                if (line._linkedLines == null) {
                    line._linkedLines = new HashSet<LineSpan> { newLine };
                } else {
                    line._linkedLines.Add(newLine);
                }

                if (newLine._linkedLines == null) {
                    newLine._linkedLines = new HashSet<LineSpan> { line };
                } else {
                    newLine._linkedLines.Add(line);
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

        private List<LineSpan> _Lines;
        public List<LineSpan> Lines {
            get {
                if (_Lines == null) Reset();
                return _Lines;
            }
            private set {
                _Lines = value;
            }
        }

        public ITextSnapshot Snapshot { get; private set; }

        public int LongestLine { get; private set; }

        public readonly LineBehavior Behavior;
        public readonly int IndentSize;
        public readonly int TabSize;

        public DocumentAnalyzer(ITextSnapshot snapshot, LineBehavior behavior, int indentSize, int tabSize) {
            Snapshot = snapshot;
            Lines = null;
            LongestLine = 0;
            Behavior = behavior.Clone();
            IndentSize = indentSize;
            TabSize = tabSize;
        }

        private struct LineInfo {
            public int Number;
            public bool HasText;
            public int TextAt;
            public HashSet<int> GuidesAt;

            public bool AnyGuides {
                get { return GuidesAt != null && GuidesAt.Count > 0; }
            }

            public bool AnyGuidesAfter(int indent) {
                return GuidesAt != null && GuidesAt.Any(i => i > indent);
            }

            public override string ToString() {
                var gas = GuidesAt == null ?
                    "" :
                    string.Join(", ", GuidesAt.OrderBy(k => k).Select(k => k.ToString()));
                if (HasText) {
                    return string.Format("{0}:{1}", TextAt, gas);
                } else if (TextAt == int.MaxValue) {
                    return "##:" + gas;
                } else {
                    return "-:" + gas;
                }
            }
        }

        public Task Reset() {
            try {
                var context = TaskScheduler.FromCurrentSynchronizationContext();
                var cts = new CancellationTokenSource();
                var token = cts.Token;
                var cts2 = Interlocked.Exchange(ref CurrentCancel, cts);
                if (cts2 != null) {
                    cts2.Cancel();
                    cts2.Dispose();
                }
                var snapshot = Snapshot.TextBuffer.CurrentSnapshot;
                var worker = Task.Factory.StartNew(() => ResetImpl(snapshot, token), token);
                var continuation = worker.ContinueWith(task => {
                    Lines = task.Result;
                    Snapshot = snapshot;
                    var cts3 = Interlocked.Exchange(ref CurrentCancel, null);
                    if (cts3 != null) {
                        cts3.Dispose();
                    }
                }, 
                    cts.Token,
                    TaskContinuationOptions.OnlyOnRanToCompletion,
                    context
                );
                return continuation;
            } catch (Exception ex) {
                Trace.TraceWarning("Asynchronous Reset() failed; running synchronously:\n{0}", ex);
                var snapshot = Snapshot.TextBuffer.CurrentSnapshot;
                try {
                    Lines = ResetImpl(snapshot, default(CancellationToken));
                    Snapshot = snapshot;
                } catch (OperationCanceledException) {
                }
                return null;
            }
        }

        private List<LineSpan> ResetImpl(ITextSnapshot snapshot, CancellationToken cancel) {
            bool isCSharp = false, isCPlusPlus = false;
            if (snapshot.ContentType != null) {
                var contentType = snapshot.ContentType.TypeName;
                isCSharp = string.Equals(contentType, "csharp", StringComparison.OrdinalIgnoreCase);
                isCPlusPlus = string.Equals(contentType, "c/c++", StringComparison.OrdinalIgnoreCase);
            }

            LongestLine = 0;

            cancel.ThrowIfCancellationRequested();

            // Maps every line number to the amount of leading whitespace on that line.
            var lineInfo = new List<LineInfo>(snapshot.LineCount + 2);

            lineInfo.Add(new LineInfo());

            object perfCookieTotal = null, perfCookieLines = null;
            PerformanceLogger.Start(ref perfCookieTotal);
            PerformanceLogger.Start(ref perfCookieLines, "_Lines");

            foreach (var line in snapshot.Lines) {
#if !PERFORMANCE
                cancel.ThrowIfCancellationRequested();
#endif

                int lineNumber = line.LineNumber + 1;
                while (lineInfo.Count <= lineNumber) {
                    lineInfo.Add(new LineInfo { Number = lineInfo.Count });
                }

                var text = line.GetText();
                if (string.IsNullOrWhiteSpace(text)) {
                    continue;
                }

                var curLine = lineInfo[lineNumber];

                var normalizedLength = text.ActualLength(TabSize);
                if (normalizedLength > LongestLine) {
                    LongestLine = normalizedLength;
                }

                int spaces = text.LeadingWhitespace(TabSize);
                curLine.HasText = true;
                curLine.TextAt = spaces;

                if (spaces > 0) {
                    if (Behavior.VisibleUnaligned || (spaces % IndentSize) == 0) {
                        curLine.GuidesAt = curLine.GuidesAt ?? new HashSet<int>();
                        curLine.GuidesAt.Add(spaces);
                    }

                    if (Behavior.VisibleAligned) {
                        curLine.GuidesAt = curLine.GuidesAt ?? new HashSet<int>();
                        var guides = curLine.GuidesAt;
                        for (int i = 0; i < spaces; i += IndentSize) {
                            guides.Add(i);
                        }
                    }
                }

                if (isCSharp || isCPlusPlus) {
                    // Left-aligned pragmas don't reduce the indent to zero.
                    if (spaces == 0 && text.StartsWith("#")) {
                        curLine.HasText = false;
                        curLine.TextAt = int.MaxValue;
                        curLine.GuidesAt = null;
                    }
                }

                lineInfo[lineNumber] = curLine;
            }

            lineInfo.Add(new LineInfo { Number = snapshot.LineCount + 1, TextAt = 0 });

            PerformanceLogger.End(perfCookieLines);
#if !PERFORMANCE
            cancel.ThrowIfCancellationRequested();
#else
            if (cancel.IsCancellationRequested) {
                PerformanceLogger.End(perfCookieTotal);
                PerformanceLogger.Mark("Cancelled");
                cancel.ThrowIfCancellationRequested();
            }
#endif

            if (Behavior.VisibleEmpty) {
                object perfCookieVisibleEmpty = null;
                PerformanceLogger.Start(ref perfCookieVisibleEmpty, "_VisibleEmpty");

                LineInfo preceding, following;

                preceding = following = lineInfo.First();
                for (int line = 1; line < lineInfo.Count - 1; ++line) {
#if !PERFORMANCE
                    cancel.ThrowIfCancellationRequested();
#endif

                    var curLine = lineInfo[line];
                    if (line >= following.Number) {
                        var nextLineIndex = lineInfo.FindIndex(line + 1, (li => li.HasText));
                        following = (nextLineIndex >= 0) ? lineInfo[nextLineIndex] : lineInfo.Last();
                    }
                    
                    if (curLine.HasText || curLine.TextAt == 0) {
                        IEnumerable<int> newGuides;
                        if (preceding.AnyGuides && following.AnyGuides) {
                            newGuides = preceding.GuidesAt.Union(following.GuidesAt);
                        } else {
                            newGuides = preceding.GuidesAt ?? following.GuidesAt;
                        }

                        if (newGuides != null) {
                            if (curLine.HasText) {
                                newGuides = newGuides.Where(i => i <= curLine.TextAt);
                            } else if (Behavior.ExtendInwardsOnly) {
                                newGuides = newGuides.Where(i => i <= preceding.TextAt && i <= following.TextAt);
                            }
                            if (curLine.AnyGuides) {
                                curLine.GuidesAt.UnionWith(newGuides);
                            } else {
                                curLine.GuidesAt = new HashSet<int>(newGuides);
                                lineInfo[line] = curLine;
                            }
                        }
                    }

                    if (curLine.HasText) {
                        preceding = curLine;
                    }
                }

                preceding = following = lineInfo.Last();
                for (int line = lineInfo.Count - 2; line > 0; --line) {
#if !PERFORMANCE
                    cancel.ThrowIfCancellationRequested();
#endif

                    var curLine = lineInfo[line];
                    if (line <= following.Number) {
                        var nextLineIndex = lineInfo.FindLastIndex(line - 1, (li => li.HasText));
                        following = (nextLineIndex >= 0) ? lineInfo[nextLineIndex] : lineInfo.First();
                    }

                    if (curLine.HasText || curLine.TextAt == 0) {
                        IEnumerable<int> newGuides;
                        if (preceding.AnyGuides && following.AnyGuides) {
                            newGuides = preceding.GuidesAt.Union(following.GuidesAt);
                        } else {
                            newGuides = preceding.GuidesAt ?? following.GuidesAt;
                        }

                        if (newGuides != null) {
                            if (curLine.HasText) {
                                newGuides = newGuides.Where(i => i <= curLine.TextAt);
                            } else if (Behavior.ExtendInwardsOnly) {
                                newGuides = newGuides.Where(i => i <= preceding.TextAt && i <= following.TextAt);
                            }
                            if (curLine.AnyGuides) {
                                curLine.GuidesAt.UnionWith(newGuides);
                            } else {
                                curLine.GuidesAt = new HashSet<int>(newGuides);
                                lineInfo[line] = curLine;
                            }
                        }
                    }

                    if (curLine.HasText) {
                        preceding = curLine;
                    }
                }

                PerformanceLogger.End(perfCookieVisibleEmpty);
            }

#if !PERFORMANCE
            cancel.ThrowIfCancellationRequested();
#else
            if (cancel.IsCancellationRequested) {
                PerformanceLogger.End(perfCookieTotal);
                PerformanceLogger.Mark("Cancelled");
                cancel.ThrowIfCancellationRequested();
            }
#endif

            object perfCookieLineSpans = null;
            PerformanceLogger.Start(ref perfCookieLineSpans, "_LineSpans");

            var result = new List<LineSpan>();
            LineSpan linkToLine;
            var linkTo = new Dictionary<int, LineSpan>();

            for (int lineNumber = 1; lineNumber < lineInfo.Count - 1; ) {
#if !PERFORMANCE
                cancel.ThrowIfCancellationRequested();
#endif

                var curLine = lineInfo[lineNumber];
                if (!curLine.AnyGuides) {
                    lineNumber += 1;
                    continue;
                }

                int indent = curLine.GuidesAt.Min();
                curLine.GuidesAt.Remove(indent);

                if (linkTo.TryGetValue(indent, out linkToLine)) {
                    linkTo.Remove(indent);
                } else {
                    linkToLine = null;
                }

                if (!curLine.AnyGuides) {
                    if (!Behavior.VisibleEmptyAtEnd && !curLine.HasText ||
                        !Behavior.VisibleAtTextEnd && curLine.HasText && curLine.TextAt == indent) {
                        continue;
                    }
                }

                int lastLineNumber = lineNumber + 1;
                while (lastLineNumber < lineInfo.Count &&
                    lineInfo[lastLineNumber].AnyGuides &&
                    lineInfo[lastLineNumber].GuidesAt.Remove(indent)
                ) {
#if !PERFORMANCE
                    cancel.ThrowIfCancellationRequested();
#endif

                    if (lineInfo[lastLineNumber].HasText) {
                        if (!Behavior.VisibleAtTextEnd && lineInfo[lastLineNumber].TextAt == indent) {
                            break;
                        }
                    } else if (lineInfo[lastLineNumber].TextAt == int.MaxValue) {
                        break;
                    } else if (!Behavior.VisibleEmptyAtEnd && !lineInfo[lastLineNumber].AnyGuidesAfter(indent)) {
                        break;
                    }
                    lastLineNumber += 1;
                }

                int formatIndex = indent / IndentSize;
                if (indent % IndentSize != 0) {
                    formatIndex = LineFormat.UnalignedFormatIndex;
                }

                var ls = new LineSpan(lineNumber - 1, lastLineNumber - 2, indent, LineSpanType.Normal) {
                    FormatIndex = formatIndex
                };
                result.Add(ls);

                if (linkToLine != null) {
                    LineSpan.Link(linkToLine, ls);
                }

                if (lineInfo[lastLineNumber].TextAt == int.MaxValue) {
                    linkTo[indent] = ls;
                }
            }

            PerformanceLogger.End(perfCookieLineSpans);
            PerformanceLogger.End(perfCookieTotal);

#if PERFORMANCE
            if (cancel.IsCancellationRequested) {
                PerformanceLogger.Mark("Cancelled");
            }
#endif
            cancel.ThrowIfCancellationRequested();

            if (snapshot != snapshot.TextBuffer.CurrentSnapshot) {
                throw new OperationCanceledException();
            }
            return result;
        }

        public Task Update() {
            if (Snapshot != Snapshot.TextBuffer.CurrentSnapshot) {
                return Reset();
            }
            return null;
        }
    }
}
