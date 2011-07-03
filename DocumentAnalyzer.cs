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
        Normal = 0,
        AtText = 1,
        EmptyLine = 2,
        EmptyLineAtText = EmptyLine | AtText
    }

    [DebuggerDisplay("Indent {Indent} lines {First}-{Last}, {Type}")]
    class LineSpan
    {
        public int First;
        public int Last;
        public int Indent;
        public LineSpanType Type;
        public int FirstPosition;
        public int LastPosition;

        public LineSpan(int first, int firstPos, int indent, LineSpanType type)
        {
            First = first;
            FirstPosition = firstPos;
            Last = first;
            LastPosition = firstPos;
            Indent = indent;
            Type = type;
        }

        public bool Extend(LineSpanType type, int indent, int last, int lastPos)
        {
            if(Type != type || Indent != indent)
                return false;

            if (Last + 1 == last)
            {
                Last = last;
                LastPosition = lastPos;
                return true;
            }

            return false;
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
            var active = new List<LineSpan>();
            var result = new List<LineSpan>();
            var emptyLines = new List<int>();
            
            foreach (var line in Snapshot.Lines)
            {
                int lineNumber = line.LineNumber;

                if (!line.IsEmpty())
                {
                    int actualPos = 0;
                    int spaceCount = IndentSize;
                    int end = line.End;
                    for (int i = line.Start; i <= end; ++i)
                    {
                        char c = i == end ? ' ' : Snapshot[i];

                        if (actualPos > 0 && (actualPos % IndentSize) == 0 && Snapshot.Length > i)
                        {
                            var type = (c == ' ' || c == '\t') ? LineSpanType.Normal : LineSpanType.AtText;
                            if (!active.Any(s => s.Extend(type, actualPos, lineNumber, i)))
                            {
                                var ls = new LineSpan(lineNumber, i, actualPos, type);
                                active.Add(ls);
                                result.Add(ls);
                            }
                        }

                        if (c == '\t')
                            actualPos = ((actualPos / IndentSize) + 1) * IndentSize;
                        else if (c == ' ')
                            actualPos += 1;
                        else
                            break;
                    }

                    if (actualPos > 0 && (actualPos % IndentSize) != 0)
                    {
                        if (!active.Any(s => s.Extend(LineSpanType.AtText, actualPos, lineNumber, end)))
                        {
                            var ls = new LineSpan(lineNumber, end, actualPos, LineSpanType.AtText);
                            active.Add(ls);
                            result.Add(ls);
                        }
                    }

                    active.RemoveAll(s => s.Last != lineNumber);
                }
                else
                {
                    emptyLines.Add(lineNumber);
                }
            }

            active.Clear();
            
            FillEmptyLines(result, emptyLines);

            Lines = result;
        }

        private void FillEmptyLines(List<LineSpan> result, List<int> emptyLines)
        {
            if (Mode == EmptyLineMode.NoGuides)
                return;

            foreach (int i in emptyLines)
            {
                IList<LineSpan> matches;
                try
                {
                    if (Mode == EmptyLineMode.SameAsLineAboveActual || Mode == EmptyLineMode.SameAsLineAboveLogical)
                    {
                        int source = result.Where(s => s.Last < i).Max(s => s.Last);
                        matches = result.Where(s => s.Last == source).ToList();
                    }
                    else if (Mode == EmptyLineMode.SameAsLineBelowActual || Mode == EmptyLineMode.SameAsLineBelowLogical)
                    {
                        int source = result.Where(s => s.First > i).Max(s => s.First);
                        matches = result.Where(s => s.First == source).ToList();
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (InvalidOperationException)
                {
                    continue;
                }

                int pos = Snapshot.GetLineFromLineNumber(i).Start.Position;
                foreach (var s in matches)
                {
                    if (!s.Extend(s.Type | LineSpanType.EmptyLine, s.Indent, i, pos))
                    {
                        result.Add(new LineSpan(i, pos, s.Indent, s.Type | LineSpanType.EmptyLine));
                    }
                }
            }
        }

        public bool Update()
        {
            if (Lines == null)
            {
                Reset();
                return true;
            }

            var changes = Snapshot.Version.Changes;

            if (changes == null || !changes.Any())
                return false;

            foreach (var change in changes)
            {
                
            }


            Reset();    // TODO: Properly handle the changes
            return true;
        }
    }
}
