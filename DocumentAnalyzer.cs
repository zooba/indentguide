using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.Text;

namespace IndentGuide {
    [Flags]
    public enum LineSpanType {
        None = 0,
        Normal = 1,
        AtText = 2,
        EmptyLine = 4,
        EmptyLineAtText = EmptyLine | AtText
    }

    [DebuggerDisplay("Indent {Indent} lines {FirstLine}-{LastLine}, {Type}")]
    public sealed class LineSpan {
        public int FirstLine;
        public int LastLine;
        public int Indent;
        public LineSpanType Type;
        public bool Changed;
        public object Adornment;
        public SnapshotSpan? Span;
        public int FormatIndex;
        public bool Highlight;

        public LineSpan(int first, int last, int indent, LineSpanType type) {
            FirstLine = first;
            LastLine = last;
            Indent = indent;
            Type = type;
            Changed = true;
            Adornment = null;
            Span = null;
            FormatIndex = 0;
            Highlight = false;
        }
    }

    class DocumentAnalyzer {
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

        public readonly LineBehavior Behavior;
        public readonly int IndentSize;
        public readonly int TabSize;

        public DocumentAnalyzer(ITextSnapshot snapshot, LineBehavior behavior, int indentSize, int tabSize) {
            Snapshot = snapshot;
            Lines = null;
            Behavior = behavior.Clone();
            IndentSize = indentSize;
            TabSize = tabSize;
        }

        public void Reset() {
            Snapshot = Snapshot.TextBuffer.CurrentSnapshot;
            var contentType = Snapshot.ContentType.TypeName;
            bool isCSharp = string.Equals(contentType, "csharp", StringComparison.OrdinalIgnoreCase);
            bool isCPlusPlus = string.Equals(contentType, "c/c++", StringComparison.OrdinalIgnoreCase);

            // Maps every line number to the amount of leading whitespace on that line.
            var lineInfo = new List<int?>(Snapshot.LineCount);

            foreach (var line in Snapshot.Lines) {
                int lineNumber = line.LineNumber;
                while (lineInfo.Count <= lineNumber) lineInfo.Add(null);

                var text = line.GetText();
                if (string.IsNullOrWhiteSpace(text))
                    continue;

                int spaces = text.LeadingWhitespace(TabSize);
                lineInfo[lineNumber] = spaces;

                if (isCSharp || isCPlusPlus) {
                    // Left-aligned pragmas don't reduce the indent to zero.
                    if (spaces == 0 && text.StartsWith("#")) {
                        lineInfo[lineNumber] = -1;
                    }
                }
            }
            lineInfo.Add(0);

            // Maps amount of leading whitespace to the line where the indent started.
            var indentInfo = new Dictionary<int, int>();
            var result = new List<LineSpan>();
            int lineStart = Behavior.TopToBottom ? 0 : (lineInfo.Count - 1);
            int lineStep = Behavior.TopToBottom ? 1 : -1;

            for (int line = lineStart; 0 <= line && line < lineInfo.Count; line += lineStep) {
                int lineNext = line + lineStep;
                int linePrev = line - lineStep;

                if (lineInfo[line].HasValue) {
                    int indent = lineInfo[line].Value;

                    if (indent == -1) {
                        foreach (var key in indentInfo.Keys.ToList()) {
                            var value = indentInfo[key];
                            if ((value < line && Behavior.TopToBottom) || (value > line && !Behavior.TopToBottom))
                                result.Add(new LineSpan(value, linePrev, key, LineSpanType.Normal));
                            indentInfo[key] = lineNext;
                        }
                        continue;
                    }

                    if (Behavior.VisibleAligned) {
                        for (int i = IndentSize; i < indent; i += IndentSize) {
                            if (!indentInfo.ContainsKey(i))
                                indentInfo[i] = line;
                        }
                    }

                    var last = result.LastOrDefault();
                    if (last != null && last.Type.HasFlag(LineSpanType.AtText) &&
                        Behavior.VisibleAtTextEnd &&
                        last.Indent == indent && last.LastLine == linePrev) {
                        last.LastLine = line;
                        indentInfo[indent] = lineNext;
                        continue;
                    }

                    if (indent > 0) {
                        foreach (var kv in indentInfo.Where(kv => kv.Key >= indent).ToList()) {
                            if ((kv.Value < line && Behavior.TopToBottom) || (kv.Value > line && !Behavior.TopToBottom))
                                result.Add(new LineSpan(kv.Value, linePrev, kv.Key, LineSpanType.Normal));
                            indentInfo.Remove(kv.Key);
                        }
                        indentInfo[indent] = lineNext;
                        if (Behavior.VisibleAtTextEnd)
                            result.Add(new LineSpan(line, line, indent, LineSpanType.AtText));
                    } else {
                        foreach (var kv in indentInfo) {
                            if ((kv.Value < line && Behavior.TopToBottom) || (kv.Value > line && !Behavior.TopToBottom))
                                result.Add(new LineSpan(kv.Value, linePrev, kv.Key, LineSpanType.Normal));
                        }
                        indentInfo.Clear();
                    }
                } else if (!Behavior.VisibleEmpty) {
                    foreach (var key in indentInfo.Keys.ToList()) {
                        if ((indentInfo[key] < line && Behavior.TopToBottom) || (indentInfo[key] > line && !Behavior.TopToBottom))
                            result.Add(new LineSpan(indentInfo[key], linePrev, key, LineSpanType.Normal));
                        indentInfo[key] = lineNext;
                    }
                } else if (!Behavior.VisibleEmptyAtEnd) {
                    if (indentInfo.Any()) {
                        int key = indentInfo.Keys.Max();
                        if ((indentInfo[key] < line && Behavior.TopToBottom) || (indentInfo[key] > line && !Behavior.TopToBottom))
                            result.Add(new LineSpan(indentInfo[key], linePrev, key, LineSpanType.Normal));
                        indentInfo[key] = lineNext;
                    }
                }
            }

            if (!Behavior.TopToBottom) {
                foreach (var ls in result) {
                    int temp = ls.LastLine;
                    ls.LastLine = ls.FirstLine;
                    ls.FirstLine = temp;
                }
            }

            Lines = result;
        }

        public bool Update() {
            if (Snapshot != Snapshot.TextBuffer.CurrentSnapshot) {
                Reset();
                return true;
            }
            return false;
        }
    }
}
