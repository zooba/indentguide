/* ****************************************************************************
 * Copyright 2012 Steve Dower
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

using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.Text;

namespace IndentGuide {
    /// <summary>
    /// Does not highlight any guides.
    /// </summary>
    class CaretNone : CaretHandlerBase, ICaretHandlerMetadata {
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

        public int GetSortOrder(CultureInfo culture) {
            return 0;
        }

        public string GetDisplayName(CultureInfo culture) {
            return ResourceLoader.LoadString("CaretNoneDisplayName", culture);
        }

        public string GetDocumentation(CultureInfo culture) {
            return ResourceLoader.LoadString("CaretNoneDocumentation", culture);
        }
    }

    class CaretNearestLeftBase : CaretHandlerBase {
        private LineSpan Nearest;
        private readonly int MinimumLength;

        protected CaretNearestLeftBase(VirtualSnapshotPoint location, int tabSize, int minimumLength)
            : base(location, tabSize) {
            Nearest = null;
            MinimumLength = minimumLength - 1;
        }

        public override void AddLine(LineSpan line, bool willUpdateImmediately) {
            if (line.FirstLine <= LineNumber &&
                LineNumber <= line.LastLine &&
                (line.LastLine - line.FirstLine) >= MinimumLength &&
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

    /// <summary>
    /// Highlights the nearest guide, not including small guides.
    /// </summary>
    class CaretNearestLeft : CaretNearestLeftBase, ICaretHandlerMetadata {
        public CaretNearestLeft(VirtualSnapshotPoint location, int tabSize)
            : base(location, tabSize, 2) { }

        public int GetSortOrder(CultureInfo culture) {
            return 10;
        }

        public string GetDisplayName(CultureInfo culture) {
            return ResourceLoader.LoadString("CaretNearestLeftDisplayName", culture);
        }

        public string GetDocumentation(CultureInfo culture) {
            return ResourceLoader.LoadString("CaretNearestLeftDocumentation", culture);
        }
    }

    /// <summary>
    /// Highlights the nearest guide, including small guides.
    /// </summary>
    class CaretNearestLeft2 : CaretNearestLeftBase, ICaretHandlerMetadata {
        public CaretNearestLeft2(VirtualSnapshotPoint location, int tabSize)
            : base(location, tabSize, 1) { }

        public int GetSortOrder(CultureInfo culture) {
            return 11;
        }

        public string GetDisplayName(CultureInfo culture) {
            return ResourceLoader.LoadString("CaretNearestLeft2DisplayName", culture);
        }

        public string GetDocumentation(CultureInfo culture) {
            return ResourceLoader.LoadString("CaretNearestLeft2Documentation", culture);
        }
    }

    /// <summary>
    /// Highlights any guide that is touched by the caret.
    /// </summary>
    class CaretAdjacent : CaretHandlerBase, ICaretHandlerMetadata {
        private readonly int MinimumLength;

        public int GetSortOrder(CultureInfo culture) {
            return 50;
        }

        public CaretAdjacent(VirtualSnapshotPoint location, int tabSize)
            : this(location, tabSize, 1) { }

        public CaretAdjacent(VirtualSnapshotPoint location, int tabSize, int minimumLength)
            : base(location, tabSize) {
            MinimumLength = minimumLength - 1;
        }

        public override void AddLine(LineSpan line, bool willUpdateImmediately) {
            bool isTouching = false;
            
            if (line.FirstLine - 1 <= LineNumber &&
                LineNumber <= line.LastLine + 1 &&
                (line.LastLine - line.FirstLine) >= MinimumLength &&
                line.Indent == Position) {
                isTouching = true;
            }

            if (line.Highlight != isTouching) {
                line.Highlight = isTouching;
                if (!willUpdateImmediately) {
                    Modified.Add(line);
                }
            }
        }

        public override IEnumerable<LineSpan> GetModified() {
            return Modified;
        }

        public string GetDisplayName(CultureInfo culture) {
            return ResourceLoader.LoadString("CaretAdjacentDisplayName", culture);
        }

        public string GetDocumentation(CultureInfo culture) {
            return ResourceLoader.LoadString("CaretAdjacentDocumentation", culture);
        }
    }

    /// <summary>
    /// Highlights guides that start or end on the lines above or below the
    /// caret.
    /// </summary>
    class CaretAboveBelowEnds : CaretHandlerBase, ICaretHandlerMetadata {
        public int GetSortOrder(CultureInfo culture) {
            return 80;
        }

        public CaretAboveBelowEnds(VirtualSnapshotPoint location, int tabSize)
            : base(location, tabSize) {
        }

        public override void AddLine(LineSpan line, bool willUpdateImmediately) {
            bool isTouching = false;

            if (line.FirstLine - 1 == LineNumber ||
                line.LastLine + 1 == LineNumber) {
                isTouching = true;
            }

            if (line.Highlight != isTouching) {
                line.Highlight = isTouching;
                if (!willUpdateImmediately) {
                    Modified.Add(line);
                }
            }
        }

        public override IEnumerable<LineSpan> GetModified() {
            return Modified;
        }

        public string GetDisplayName(CultureInfo culture) {
            return ResourceLoader.LoadString("CaretAboveBelowEndsDisplayName", culture);
        }

        public string GetDocumentation(CultureInfo culture) {
            return ResourceLoader.LoadString("CaretAboveBelowEndsDocumentation", culture);
        }
    }
}
