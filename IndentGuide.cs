using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace IndentGuide
{
    /// <summary>
    /// Manages indent guides for a particular text view.
    /// </summary>
    public class IndentGuideView
    {
        IAdornmentLayer Layer;
        IWpfTextView View;
        Brush GuideBrush;
        IndentTheme Theme;
        int VersionNumber;

        /// <summary>
        /// Instantiates a new indent guide manager for a view.
        /// </summary>
        /// <param name="view">The text view to provide guides for.</param>
        /// <param name="service">The Indent Guide service.</param>
        public IndentGuideView(IWpfTextView view, IIndentGuide service)
        {
            VersionNumber = 0;

            View = view;
            View.LayoutChanged += View_LayoutChanged;

            Layer = view.GetAdornmentLayer("IndentGuide");

            Theme = service.DefaultTheme;
            Theme.Updated += Theme_Updated;

            GuideBrush = new SolidColorBrush(Theme.LineFormat.LineColor.ToSWMC());
            if (GuideBrush.CanFreeze) GuideBrush.Freeze();
        }

        /// <summary>
        /// Raised when the display changes.
        /// </summary>
        void View_LayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            UpdateAdornments();
        }

        /// <summary>
        /// Raised when the theme is updated.
        /// </summary>
        void Theme_Updated(object sender, EventArgs e)
        {
            UpdateAdornments();
        }



        /// <summary>
        /// Recreates all adornments.
        /// </summary>
        void UpdateAdornments()
        {
            Debug.Assert(View != null);
            Debug.Assert(Layer != null);
            Debug.Assert(View.TextViewLines != null);
            if (View == null || Layer == null || View.TextViewLines == null) return;

            if (!Theme.LineFormat.Visible)
            {
                Layer.RemoveAllAdornments();
                return;
            }

            int previousVersionNumber = VersionNumber;
            VersionNumber = (VersionNumber == int.MaxValue) ? int.MinValue : VersionNumber + 1;
            int tabSize = View.Options.GetOptionValue(DefaultOptions.TabSizeOptionId);


            var lines = View.TextViewLines.Cast<IWpfTextViewLine>().ToList();
            if (Theme.EmptyLineMode == EmptyLineMode.SameAsLineBelowActual ||
                Theme.EmptyLineMode == EmptyLineMode.SameAsLineBelowLogical)
            {
                lines.Reverse();
            }

            bool logical = (Theme.EmptyLineMode == EmptyLineMode.SameAsLineAboveLogical ||
                Theme.EmptyLineMode == EmptyLineMode.SameAsLineBelowLogical);

            var activeGuides = new Dictionary<int, Tuple<double, IWpfTextViewLine>>();
            var previousGuidesAt = new Dictionary<int, double>();
            IWpfTextViewLine previousLine = null;

            foreach (var line in lines)
            {
                Dictionary<int, double> guidesAt = null;
                bool excludeLast = !logical;

                if (line.IsEmpty())
                {
                    if (Theme.EmptyLineMode == EmptyLineMode.NoGuides)
                        guidesAt = null;
                    else
                        guidesAt = previousGuidesAt;
                }
                else
                {
                    guidesAt = GetIndentLocations(tabSize, line);
                    excludeLast = true;
                }

                int exclude = 0;
                if (excludeLast && guidesAt != null && guidesAt.Any())
                {
                    exclude = guidesAt.Max(kv => kv.Key);
                }

                foreach (var kv in activeGuides.ToList())
                {
                    if (guidesAt == null || !guidesAt.ContainsKey(kv.Key) || kv.Key == exclude)
                    {
                        var pos = kv.Value.Item1;
                        var firstLine = kv.Value.Item2;
                        var lastLine = previousLine ?? firstLine;
                        AddGuide(firstLine, lastLine, pos);
                        activeGuides.Remove(kv.Key);
                    }
                }

                if (guidesAt != null)
                {
                    foreach (var kv in guidesAt)
                    {
                        if (!activeGuides.ContainsKey(kv.Key) && kv.Key != exclude)
                            activeGuides[kv.Key] = new Tuple<double, IWpfTextViewLine>(kv.Value, line);
                    }
                }

                previousGuidesAt = guidesAt;
                previousLine = line;
            }

            foreach (var kv in activeGuides)
            {
                var pos = kv.Value.Item1;
                var firstLine = kv.Value.Item2;
                var lastLine = previousLine ?? firstLine;
                AddGuide(firstLine, lastLine, pos);
            }

            Layer.RemoveAdornmentsByTag(previousVersionNumber);
        }

        private Dictionary<int, double> GetIndentLocations(int tabSize, IWpfTextViewLine line)
        {
            var locations = new Dictionary<int, double>();
            var snapshot = line.Snapshot;
            int actualPos = 0;
            int spaceCount = tabSize;
            int end = line.End;
            for (int i = line.Start; i <= end; ++i)
            {
                char c = i == end ? ' ' : snapshot[i];

                if (actualPos > 0 && (actualPos % tabSize) == 0 &&
                    snapshot.Length > i)
                {
                    var span = new SnapshotSpan(snapshot, i, 1);
                    double left = View.TextViewLines.GetMarkerGeometry(span).Bounds.Left;
                    locations[actualPos] = left;
                }

                if (c == '\t')
                    actualPos = ((actualPos / tabSize) + 1) * tabSize;
                else if (c == ' ')
                    actualPos += 1;
                else
                    break;
            }

            if (actualPos > 0 && (actualPos % tabSize) != 0) locations[actualPos] = double.MaxValue;

            return locations;
        }

        /// <summary>
        /// Adds a guideline at the specified location.
        /// </summary>
        /// <param name="line">The line to add the guide for.</param>
        /// <param name="left">The horizontal location to add the
        /// guide.</param>
        private void AddGuide(ITextViewLine lineFirst, ITextViewLine lineLast, double left)
        {
            if (left == 0 || left > View.ViewportWidth) return;

            var top = Math.Min(lineFirst.Top, lineLast.Top);
            var bottom = Math.Max(lineFirst.Bottom, lineLast.Bottom);

            var guide = new Line()
            {
                X1 = left,
                Y1 = top,
                X2 = left,
                Y2 = bottom,
                Stroke = GuideBrush,
                StrokeThickness = 1.0,
                StrokeDashOffset = top,
                SnapsToDevicePixels = true
            };
            
            if (Theme.LineFormat.LineStyle == LineStyle.Thick)
                guide.StrokeThickness = 3.0;
            else if (Theme.LineFormat.LineStyle == LineStyle.Dotted)
                guide.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            else if (Theme.LineFormat.LineStyle == LineStyle.Dashed)
                guide.StrokeDashArray = new DoubleCollection { 3.0, 3.0 };

            SnapshotSpan span;
            if (lineFirst.Start < lineLast.Start)
                span = new SnapshotSpan(lineFirst.Start, lineLast.End);
            else
                span = new SnapshotSpan(lineLast.Start, lineFirst.End);

            Layer.AddAdornment(span, VersionNumber, guide);
        }
    }
}
