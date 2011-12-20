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

        public readonly EmptyLineMode Mode;
        public readonly int IndentSize;

        public DocumentAnalyzer(ITextSnapshot snapshot, EmptyLineMode mode, int indentSize)
        {
            Snapshot = snapshot;
            Lines = null;
            Mode = mode;
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
            bool reverse = (Mode == EmptyLineMode.SameAsLineBelowActual || Mode == EmptyLineMode.SameAsLineBelowLogical);
            bool atAllIndents = true;
            int lineStart = !reverse ? 0 : (lineInfo.Count - 1);
            int lineStep = !reverse ? 1 : -1;
            
            for (int line = lineStart; 0 <= line && line < lineInfo.Count; line += lineStep)
            {
                int lineNext = line + lineStep;
                int linePrev = line - lineStep;

                if (lineInfo[line].HasValue)
                {
                    int indent = lineInfo[line].Value;

                    if (atAllIndents)
                    {
                        for (int i = IndentSize; i < indent; i += IndentSize)
                        {
                            if (!indentInfo.ContainsKey(i))
                                indentInfo[i] = line;
                        }
                    }

                    var last = result.LastOrDefault();
                    if (last != null && last.Type.HasFlag(LineSpanType.AtText) &&
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
                            if ((kv.Value < line && !reverse) || (kv.Value > line && reverse))
                                result.Add(new LineSpan(kv.Value, linePrev, kv.Key, LineSpanType.Normal));
                            indentInfo.Remove(kv.Key);
                        }
                        indentInfo[indent] = lineNext;
                        result.Add(new LineSpan(line, line, indent, LineSpanType.AtText));
                    }
                    else
                    {
                        foreach (var kv in indentInfo)
                        {
                            if ((kv.Value < line && !reverse) || (kv.Value > line && reverse))
                                result.Add(new LineSpan(kv.Value, linePrev, kv.Key, LineSpanType.Normal));
                        }
                        indentInfo.Clear();
                    }
                }
                else if (Mode == EmptyLineMode.NoGuides)
                {
                    foreach (var key in indentInfo.Keys.ToList())
                    {
                        if ((indentInfo[key] < line && !reverse) || (indentInfo[key] > line && reverse))
                            result.Add(new LineSpan(indentInfo[key], linePrev, key, LineSpanType.Normal));
                        indentInfo[key] = lineNext;
                    }
                }
                else if (Mode == EmptyLineMode.SameAsLineAboveActual || Mode == EmptyLineMode.SameAsLineBelowActual)
                {
                    if (indentInfo.Any())
                    {
                        int key = indentInfo.Keys.Max();
                        if ((indentInfo[key] < line && !reverse) || (indentInfo[key] > line && reverse))
                            result.Add(new LineSpan(indentInfo[key], linePrev, key, LineSpanType.Normal));
                        indentInfo[key] = lineNext;
                    }
                }
            }

            if (reverse)
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

        private List<LineSpanType[]> Transpose(List<List<LineSpanType>> source)
        {
            var result = new List<LineSpanType[]>();

            int i = 0;
            bool anyLeft = true;
            while (anyLeft)
            {
                anyLeft = false;
                var lines = new LineSpanType[source.Count];
                result.Add(lines);
                for (int j = 0; j < source.Count; ++j)
                {
                    var li = source[j];
                    if (li != null && i < li.Count) lines[j] = li[i];
                    anyLeft |= li != null && (i + 1) < li.Count;
                }

                i += 1;
            }

            return result;
        }

        public bool Update()
        {
            // HACK: Removed incremental updates for now
            Reset();
            return true;
        }
    }
}
