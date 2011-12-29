using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using System.Diagnostics;

namespace IndentGuide
{
    [Flags]
    enum LineSpanType
    {
        None = 0,
        Normal = 1,
        AtText = 2,
        EmptyLine = 4,
        EmptyLineAtText = EmptyLine | AtText
    }

    [DebuggerDisplay("Indent {Indent} lines {FirstLine}-{LastLine}, {Type}")]
    class LineSpan
    {
        public int FirstLine;
        public int LastLine;
        public int Indent;
        public LineSpanType Type;
        public bool Changed;
        public object Adornment;

        public LineSpan(int first, int last, int indent, LineSpanType type)
        {
            FirstLine = first;
            LastLine = last;
            Indent = indent;
            Type = type;
            Changed = true;
            Adornment = null;
        }
    }

    class DocumentAnalyzer
    {
        private List<LineSpan> _Lines;
        public List<LineSpan> Lines
        {
            get
            {
                if (_Lines == null) Reset();
                return _Lines;
            }
            private set
            {
                _Lines = value;
            }
        }

        public ITextSnapshot Snapshot { get; private set; }

        public readonly IndentTheme Theme;
        public readonly int IndentSize;

        public DocumentAnalyzer(ITextSnapshot snapshot, IndentTheme theme, int indentSize)
        {
            Snapshot = snapshot;
            Lines = null;
            Theme = theme;
            IndentSize = indentSize;
        }

        public void Reset()
        {
            Snapshot = Snapshot.TextBuffer.CurrentSnapshot;

            var lineInfo = new List<int?>(Snapshot.LineCount);

            foreach (var line in Snapshot.Lines)
            {
                int lineNumber = line.LineNumber;
                while (lineInfo.Count <= lineNumber) lineInfo.Add(null);

                var text = line.GetText();
                if (string.IsNullOrWhiteSpace(text))
                    continue;

                lineInfo[lineNumber] = text.LeadingWhitespace(IndentSize);
            }
            lineInfo.Add(0);

            var indentInfo = new Dictionary<int, int>();
            var result = new List<LineSpan>();
            int lineStart = Theme.TopToBottom ? 0 : (lineInfo.Count - 1);
            int lineStep = Theme.TopToBottom ? 1 : -1;
            
            for (int line = lineStart; 0 <= line && line < lineInfo.Count; line += lineStep)
            {
                int lineNext = line + lineStep;
                int linePrev = line - lineStep;

                if (lineInfo[line].HasValue)
                {
                    int indent = lineInfo[line].Value;

                    if (Theme.VisibleAligned)
                    {
                        for (int i = IndentSize; i < indent; i += IndentSize)
                        {
                            if (!indentInfo.ContainsKey(i))
                                indentInfo[i] = line;
                        }
                    }

                    var last = result.LastOrDefault();
                    if (last != null && last.Type.HasFlag(LineSpanType.AtText) &&
                        Theme.VisibleAtTextEnd &&
                        last.Indent == indent && last.LastLine == linePrev)
                    {
                        last.LastLine = line;
                        indentInfo[indent] = lineNext;
                        continue;
                    }

                    if (indent > 0)
                    {
                        foreach (var kv in indentInfo.Where(kv => kv.Key >= indent).ToList())
                        {
                            if ((kv.Value < line && Theme.TopToBottom) || (kv.Value > line && !Theme.TopToBottom))
                                result.Add(new LineSpan(kv.Value, linePrev, kv.Key, LineSpanType.Normal));
                            indentInfo.Remove(kv.Key);
                        }
                        indentInfo[indent] = lineNext;
                        if (Theme.VisibleAtTextEnd)
                            result.Add(new LineSpan(line, line, indent, LineSpanType.AtText));
                    }
                    else
                    {
                        foreach (var kv in indentInfo)
                        {
                            if ((kv.Value < line && Theme.TopToBottom) || (kv.Value > line && !Theme.TopToBottom))
                                result.Add(new LineSpan(kv.Value, linePrev, kv.Key, LineSpanType.Normal));
                        }
                        indentInfo.Clear();
                    }
                }
                else if (!Theme.VisibleEmpty)
                {
                    foreach (var key in indentInfo.Keys.ToList())
                    {
                        if ((indentInfo[key] < line && Theme.TopToBottom) || (indentInfo[key] > line && !Theme.TopToBottom))
                            result.Add(new LineSpan(indentInfo[key], linePrev, key, LineSpanType.Normal));
                        indentInfo[key] = lineNext;
                    }
                }
                else if (!Theme.VisibleEmptyAtEnd)
                {
                    if (indentInfo.Any())
                    {
                        int key = indentInfo.Keys.Max();
                        if ((indentInfo[key] < line && Theme.TopToBottom) || (indentInfo[key] > line && !Theme.TopToBottom))
                            result.Add(new LineSpan(indentInfo[key], linePrev, key, LineSpanType.Normal));
                        indentInfo[key] = lineNext;
                    }
                }
            }

            if (!Theme.TopToBottom)
            {
                foreach (var ls in result)
                {
                    int temp = ls.LastLine;
                    ls.LastLine = ls.FirstLine;
                    ls.FirstLine = temp;
                }
            }

            Lines = result;
        }

        public bool Update()
        {
            // HACK: Removed incremental updates for now
            Reset();
            return true;
        }
    }
}
