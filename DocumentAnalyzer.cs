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

            for (int line = 0; line < lineInfo.Count; ++line)
            {
                if (lineInfo[line].HasValue)
                {
                    int indent = lineInfo[line].Value;

                    if (indent == 0)
                    {
                        foreach (var kv in indentInfo)
                        {
                            if (kv.Value < line)
                                result.Add(new LineSpan(kv.Value, line - 1, kv.Key, LineSpanType.Normal));
                        }
                        indentInfo.Clear();
                    }
                    else
                    {
                        foreach (var kv in indentInfo.Where(kv => kv.Key >= indent).ToList())
                        {
                            if (kv.Value < line)
                                result.Add(new LineSpan(kv.Value, line - 1, kv.Key, LineSpanType.Normal));
                            indentInfo.Remove(kv.Key);
                        }
                        indentInfo[indent] = line + 1;
                    }
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
