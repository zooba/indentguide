using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        private class LineInfo {
            public int Number;
            public bool HasText = false;
            public int TextAt = 0;
            public readonly HashSet<int> GuidesAt = new HashSet<int>();

            public override string ToString() {
                var gas = string.Join(", ", GuidesAt.OrderBy(k => k).Select(k => k.ToString()));
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
                var snapshot = Snapshot.TextBuffer.CurrentSnapshot;
                var worker = new Task<List<LineSpan>>(ResetImpl, snapshot, cts.Token);
                var continuation = worker.ContinueWith(task => {
                    if (task.Result == null) {
                        cts.Cancel();
                    } else {
                        Lines = task.Result;
                        Snapshot = snapshot;
                    }
                }, cts.Token,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                context);
                worker.Start();
                return continuation;
            } catch (Exception ex) {
                Trace.TraceWarning("Asynchronous Reset() failed; running synchronously:\n{0}", ex);
                var snapshot = Snapshot.TextBuffer.CurrentSnapshot;
                Lines = ResetImpl(snapshot);
                Snapshot = snapshot;
                return null;
            }
        }

        private List<LineSpan> ResetImpl(object snapshot_obj) {
            var snapshot = snapshot_obj as ITextSnapshot;
            var contentType = snapshot.ContentType.TypeName;
            bool isCSharp = string.Equals(contentType, "csharp", StringComparison.OrdinalIgnoreCase);
            bool isCPlusPlus = string.Equals(contentType, "c/c++", StringComparison.OrdinalIgnoreCase);

            // Maps every line number to the amount of leading whitespace on that line.
            var lineInfo = new List<LineInfo>(snapshot.LineCount + 2);

            lineInfo.Add(new LineInfo { Number = 0, TextAt = 0 });

            foreach (var line in snapshot.Lines) {
                int lineNumber = line.LineNumber + 1;
                while (lineInfo.Count <= lineNumber) {
                    lineInfo.Add(new LineInfo { Number = lineInfo.Count });
                }

                var text = line.GetText();
                if (string.IsNullOrWhiteSpace(text))
                    continue;

                int spaces = text.LeadingWhitespace(TabSize);
                lineInfo[lineNumber].HasText = true;
                lineInfo[lineNumber].TextAt = spaces;

                if (spaces > 0) {
                    if (Behavior.VisibleUnaligned || (spaces % IndentSize) == 0) {
                        lineInfo[lineNumber].GuidesAt.Add(spaces);
                    }

                    if (Behavior.VisibleAligned) {
                        var guides = lineInfo[lineNumber].GuidesAt;
                        for (int i = IndentSize; i < spaces; i += IndentSize) {
                            guides.Add(i);
                        }
                    }
                }

                if (isCSharp || isCPlusPlus) {
                    // Left-aligned pragmas don't reduce the indent to zero.
                    if (spaces == 0 && text.StartsWith("#")) {
                        lineInfo[lineNumber].HasText = false;
                        lineInfo[lineNumber].TextAt = int.MaxValue;
                        lineInfo[lineNumber].GuidesAt.Clear();
                    }
                }
            }

            lineInfo.Add(new LineInfo { Number = snapshot.LineCount + 1, TextAt = 0 });

            if (Behavior.VisibleEmpty) {
                LineInfo preceding, following;

                preceding = following = lineInfo.First();
                for (int line = 1; line < lineInfo.Count - 1; ++line) {
                    var curLine = lineInfo[line];
                    if (line >= following.Number) {
                        var nextLineIndex = lineInfo.FindIndex(line + 1, (li => li.HasText));
                        following = (nextLineIndex >= 0) ? lineInfo[nextLineIndex] : lineInfo.Last();
                    }
                    
                    IEnumerable<int> newGuides = preceding.GuidesAt.Union(following.GuidesAt);
                    if (curLine.HasText) {
                        newGuides = newGuides.Where(i => i <= curLine.TextAt);
                    } else if (Behavior.ExtendInwardsOnly) {
                        newGuides = newGuides.Where(i => i <= preceding.TextAt && i <= following.TextAt);
                    }
                    curLine.GuidesAt.UnionWith(newGuides);
                    
                    if (curLine.HasText) preceding = curLine;
                }

                preceding = following = lineInfo.Last();
                for (int line = lineInfo.Count - 2; line > 0; --line) {
                    var curLine = lineInfo[line];
                    if (line <= following.Number) {
                        var nextLineIndex = lineInfo.FindLastIndex(line - 1, (li => li.HasText));
                        following = (nextLineIndex >= 0) ? lineInfo[nextLineIndex] : lineInfo.First();
                    }

                    IEnumerable<int> newGuides = preceding.GuidesAt.Union(following.GuidesAt);
                    if (curLine.HasText) {
                        newGuides = newGuides.Where(i => i <= curLine.TextAt);
                    } else if (Behavior.ExtendInwardsOnly) {
                        newGuides = newGuides.Where(i => i <= preceding.TextAt && i <= following.TextAt);
                    }
                    curLine.GuidesAt.UnionWith(newGuides);

                    if (curLine.HasText) preceding = curLine;
                }
            }

            var result = new List<LineSpan>();

            for (int lineNumber = 1; lineNumber < lineInfo.Count - 1; ) {
                var curLine = lineInfo[lineNumber];
                if (!curLine.GuidesAt.Any()) {
                    lineNumber += 1;
                    continue;
                }

                int indent = curLine.GuidesAt.Min();
                curLine.GuidesAt.Remove(indent);

                if (!curLine.GuidesAt.Any()) {
                    if (!Behavior.VisibleEmptyAtEnd && !curLine.HasText ||
                        !Behavior.VisibleAtTextEnd && curLine.HasText && curLine.TextAt == indent) {
                        continue;
                    }
                }

                int lastLineNumber = lineNumber + 1;
                while (lastLineNumber < lineInfo.Count && lineInfo[lastLineNumber].GuidesAt.Remove(indent)) {
                    if (lineInfo[lastLineNumber].HasText) {
                        if (!Behavior.VisibleAtTextEnd && lineInfo[lastLineNumber].TextAt == indent) {
                            break;
                        }
                    } else if (lineInfo[lastLineNumber].TextAt == int.MaxValue) {
                        break;
                    } else if (!Behavior.VisibleEmptyAtEnd && !lineInfo[lastLineNumber].GuidesAt.Any(i => i > indent)) {
                        break;
                    }
                    lastLineNumber += 1;
                }

                int formatIndex = indent / IndentSize;
                if (indent % IndentSize != 0) {
                    formatIndex = IndentTheme.UnalignedFormatIndex;
                }

                result.Add(new LineSpan(lineNumber - 1, lastLineNumber - 2, indent, LineSpanType.Normal) {
                    FormatIndex = formatIndex
                });
            }

            if (snapshot != snapshot.TextBuffer.CurrentSnapshot) {
                return null;
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
