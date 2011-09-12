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

            var lineInfo = new List<List<LineSpanType>>(Snapshot.LineCount);

            foreach (var line in Snapshot.Lines)
            {
                int lineNumber = line.LineNumber;
                while (lineInfo.Count <= lineNumber) lineInfo.Add(null);

                if (line.IsEmpty())
                    continue;

                var indents = new List<LineSpanType>();
                lineInfo[lineNumber] = indents;

                int actualPos = 0;
                int spaceCount = IndentSize;
                int end = line.End;
                for (int i = line.Start; i <= end; ++i)
                {
                    char c = i == end ? ' ' : Snapshot[i];

                    if (actualPos > 0 && (actualPos % IndentSize) == 0 && Snapshot.Length > i)
                    {
                        var type = (c == ' ' || c == '\t') ? LineSpanType.Normal : LineSpanType.AtText;
                        indents.Add(type);
                    }

                    if (c == '\t')
                        actualPos = ((actualPos / IndentSize) + 1) * IndentSize;
                    else if (c == ' ')
                        actualPos += 1;
                    else
                        break;
                }

                if (actualPos > 0 && (actualPos % IndentSize) != 0)
                    indents.Add(LineSpanType.AtText);
            }

            FillEmptyLines(lineInfo);

            // TODO: Rewrite the loop below to avoid the need to transpose.
            var indentInfo = Transpose(lineInfo);

            var result = new List<LineSpan>();

            for (int indent = 0; indent < indentInfo.Count; indent += 1)
            {
                var lines = indentInfo[indent];
                if (lines.Length == 0) continue;

                int first = 0;
                var previous = lines[0];
                for (int i = 1; i < lines.Length; i += 1)
                {
                    if (lines[i] == previous)
                        continue;

                    if (previous != LineSpanType.None)
                        result.Add(new LineSpan(first, i - 1, indent, previous));
                    first = i;
                    previous = lines[i];
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

        private void FillEmptyLines(List<List<LineSpanType>> lineInfo)
        {
            if (Mode == EmptyLineMode.NoGuides)
                return;

            for (int i = 0; i < lineInfo.Count; ++i)
            {
                if (lineInfo[i] != null) continue;

                var indents = new List<LineSpanType>();
                lineInfo[i] = indents;

                if (Mode == EmptyLineMode.SameAsLineAboveActual || Mode == EmptyLineMode.SameAsLineAboveLogical)
                {
                    int source = i - 1;
                    while (source >= 0 && lineInfo[source] == null) --source;
                    if (source < 0) continue;

                    indents.AddRange(lineInfo[source]);
                }
                else if (Mode == EmptyLineMode.SameAsLineBelowActual || Mode == EmptyLineMode.SameAsLineBelowLogical)
                {
                    int source = i + 1;
                    while (source < lineInfo.Count && lineInfo[source] == null) ++source;
                    if (source >= lineInfo.Count) continue;

                    indents.AddRange(lineInfo[source]);
                }
            }
        }

        public bool Update()
        {
            // HACK: Removed incremental updates for now
            Reset();
            return true;
        }
    }
}
