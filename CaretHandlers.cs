using System.Collections.Generic;
using Microsoft.VisualStudio.Text;

namespace IndentGuide {
    /// <summary>
    /// Does not highlight any guides.
    /// </summary>
    class CaretNone : CaretHandlerBase {
        public CaretNone(VirtualSnapshotPoint location, int tabSize)
            : base(location, tabSize) {
        }

        public override void AddLine(LineSpan lineSpan, bool willUpdateImmediately) {
            if (lineSpan.Highlight) {
                lineSpan.Highlight = false;
                if (!willUpdateImmediately) {
                    Modified.Add(lineSpan);
                }
            }
        }

        public override IEnumerable<LineSpan> GetModified() {
            return Modified;
        }
    }

    /// <summary>
    /// Highlights the nearest guide to the left of the caret.
    /// </summary>
    class CaretNearestLeft : CaretHandlerBase {
        private LineSpan Nearest;
        private readonly int MinimumLength;

        public CaretNearestLeft(VirtualSnapshotPoint location, int tabSize)
            : this(location, tabSize, 2) { }
        
        public CaretNearestLeft(VirtualSnapshotPoint location, int tabSize, int minimumLength)
            : base(location, tabSize) {
            Nearest = null;
            MinimumLength = minimumLength - 1;
        }

        public override void AddLine(LineSpan line, bool willUpdateImmediately) {
            if (line.FirstLine <= LineNumber &&
                LineNumber <= line.LastLine &&
                (line.LastLine - line.FirstLine) >= MinimumLength &&
                !line.Type.HasFlag(LineSpanType.AtText) &&
                line.Indent <= Position &&
                (Nearest == null || line.Indent > Nearest.Indent)) {
                Nearest = line;
            }
            
            if (line.Highlight) {
                line.Highlight = false;
                if (!willUpdateImmediately) {
                    Modified.Add(line);
                }
            }
        }

        public override IEnumerable<LineSpan> GetModified() {
            if (Nearest != null) {
                while (Modified.Remove(Nearest)) { }
                Nearest.Highlight = true;
                Modified.Add(Nearest);
                Nearest = null;
            }
            return Modified;
        }
    }
}
