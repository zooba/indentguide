using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
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
        IDictionary<System.Drawing.Color, Brush> GuideBrushCache;
        IndentTheme Theme;
        bool GlobalVisible;

        DocumentAnalyzer Analysis;
        Dictionary<int, double> CachedLefts;

        /// <summary>
        /// Instantiates a new indent guide manager for a view.
        /// </summary>
        /// <param name="view">The text view to provide guides for.</param>
        /// <param name="service">The Indent Guide service.</param>
        public IndentGuideView(IWpfTextView view, IIndentGuide service)
        {
            CachedLefts = new Dictionary<int, double>();
            GuideBrushCache = new Dictionary<System.Drawing.Color, Brush>();

            View = view;
            View.LayoutChanged += View_LayoutChanged;

            Layer = view.GetAdornmentLayer("IndentGuide");

            if (!service.Themes.TryGetValue(View.TextDataModel.ContentType.DisplayName, out Theme))
                Theme = service.DefaultTheme;
            Debug.Assert(Theme != null, "No themes loaded");
            service.ThemesChanged += new EventHandler(Service_ThemesChanged);

            Analysis = new DocumentAnalyzer(view.TextSnapshot, Theme.EmptyLineMode,
                View.Options.GetOptionValue(DefaultOptions.IndentSizeOptionId));
            Analysis.Reset();

            GlobalVisible = service.Visible;
            service.VisibleChanged += new EventHandler(Service_VisibleChanged);
        }

        /// <summary>
        /// Raised when the global visibility property is updated.
        /// </summary>
        void Service_VisibleChanged(object sender, EventArgs e)
        {
            GlobalVisible = ((IIndentGuide)sender).Visible;
            InvalidateLines();
            UpdateAdornments();
        }

        /// <summary>
        /// Raised when the theme is updated.
        /// </summary>
        void Service_ThemesChanged(object sender, EventArgs e)
        {
            var service = (IIndentGuide)sender;
            if (!service.Themes.TryGetValue(View.TextDataModel.ContentType.DisplayName, out Theme))
                Theme = service.DefaultTheme;

            Analysis = new DocumentAnalyzer(Analysis.Snapshot, Theme.EmptyLineMode,
                View.Options.GetOptionValue(DefaultOptions.TabSizeOptionId));
            GuideBrushCache.Clear();

            InvalidateLines();
            UpdateAdornments();
        }

        /// <summary>
        /// Raised when the display changes.
        /// </summary>
        void View_LayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            Analysis.Update();
            UpdateAdornments();
        }

        /// <summary>
        /// Removes all lines from the line cache.
        /// </summary>
        void InvalidateLines()
        {
            if (Layer != null) Layer.RemoveAllAdornments();
            Analysis.Reset();
            CachedLefts.Clear();
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

            Layer.RemoveAllAdornments();

            if (!GlobalVisible)
                return;

            double spaceWidth = 0.0;
            {
                var suitable = View.TextViewLines.FirstOrDefault(l => 
                    View.TextSnapshot.GetLineFromPosition(l.Start.Position).GetText().StartsWith(" "));
                if (suitable != null)
                {
                    spaceWidth = View.TextViewLines.GetMarkerGeometry(new SnapshotSpan(suitable.Start, 1)).Bounds.Width;
                }
            }
            if (spaceWidth <= 0.0) return;

            foreach (var line in Analysis.Lines)
            {
                if (line.Type == LineSpanType.AtText && !Theme.VisibleAtText)
                    continue;
                if (line.Type == LineSpanType.EmptyLineAtText &&
                    !Theme.VisibleAtText &&
                    (Theme.EmptyLineMode == EmptyLineMode.SameAsLineAboveActual ||
                     Theme.EmptyLineMode == EmptyLineMode.SameAsLineBelowActual))
                    continue;

                int linePos = line.Indent;
                var firstLine = View.TextSnapshot.GetLineFromLineNumber(line.FirstLine);
                var lastLine = View.TextSnapshot.GetLineFromLineNumber(line.LastLine);
                var first = firstLine.Start + (firstLine.Length >= linePos ? linePos : 0);
                var last = lastLine.Start + (lastLine.Length >= linePos ? linePos : 0);
                if (first > View.TextViewLines.LastVisibleLine.Start) continue;
                if (last < View.TextViewLines.FirstVisibleLine.Start) continue;
                var firstView = View.GetTextViewLineContainingBufferPosition(first);
                var lastView = View.GetTextViewLineContainingBufferPosition(last);

                double top = (firstView.VisibilityState != VisibilityState.Unattached) ?
                    firstView.Top :
                    View.TextViewLines.FirstVisibleLine.Top;
                double bottom = (lastView.VisibilityState != VisibilityState.Unattached) ?
                    lastView.Bottom :
                    View.TextViewLines.LastVisibleLine.Bottom;
                double left = line.Indent * spaceWidth + 
                    ((firstView.VisibilityState == VisibilityState.FullyVisible) ?
                    firstView.TextLeft : View.TextViewLines.FirstVisibleLine.TextLeft);

                int formatIndex = line.Indent / Analysis.IndentSize;
                if (line.Indent % Analysis.IndentSize != 0)
                    formatIndex = 0;

                var guide = AddGuide(top, bottom, left, formatIndex);
                line.Adornment = guide;

                if (guide != null)
                {
                    Layer.AddAdornment(AdornmentPositioningBehavior.TextRelative,
                        new SnapshotSpan(first, last), line, guide, null);
                }
            }
        }

        /// <summary>
        /// Adds a guideline at the specified location.
        /// </summary>
        /// <param name="firstLine">The line to start the guide at.</param>
        /// <param name="lastLine">The line to end the guide at.</param>
        /// <param name="indent">The indent number.</param>
        /// <param name="formatIndex">The format index for this guide.
        /// </param>
        /// <returns>The added line.</returns>
        private Line AddGuide(double top, double bottom, double left, int formatIndex)
        {
            if (left == 0 || left > View.ViewportWidth) return null;

            LineFormat format;
            Brush brush;
            if (!Theme.NumberedOverride.TryGetValue(formatIndex, out format))
                format = Theme.DefaultLineFormat;
            
            if (!format.Visible) return null;

            if (!GuideBrushCache.TryGetValue(format.LineColor, out brush))
            {
                brush = new SolidColorBrush(format.LineColor.ToSWMC());
                if (brush.CanFreeze) brush.Freeze();
                GuideBrushCache[format.LineColor] = brush;
            }

            var guide = new Line()
            {
                X1 = left,
                Y1 = top,
                X2 = left,
                Y2 = bottom,
                Stroke = brush,
                StrokeThickness = 1.0,
                StrokeDashOffset = top,
                SnapsToDevicePixels = true
            };

            if (format.LineStyle == LineStyle.Thick)
                guide.StrokeThickness = 3.0;
            else if (format.LineStyle == LineStyle.Dotted)
                guide.StrokeDashArray = new DoubleCollection { 1.0, 2.0 };
            else if (format.LineStyle == LineStyle.Dashed)
                guide.StrokeDashArray = new DoubleCollection { 3.0, 3.0 };

            return guide;
        }
    }
}
