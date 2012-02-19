using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace IndentGuide {
    /// <summary>
    /// Manages indent guides for a particular text view.
    /// </summary>
    public class IndentGuideView {
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
        public IndentGuideView(IWpfTextView view, IIndentGuide service) {
            CachedLefts = new Dictionary<int, double>();
            GuideBrushCache = new Dictionary<System.Drawing.Color, Brush>();

            View = view;
            View.Caret.PositionChanged += Caret_PositionChanged;
            View.LayoutChanged += View_LayoutChanged;
            View.Options.OptionChanged += View_OptionChanged;

            Layer = view.GetAdornmentLayer("IndentGuide");

            if (!service.Themes.TryGetValue(View.TextDataModel.ContentType.DisplayName, out Theme))
                Theme = service.DefaultTheme;
            Debug.Assert(Theme != null, "No themes loaded");
            service.ThemesChanged += new EventHandler(Service_ThemesChanged);

            Analysis = new DocumentAnalyzer(view.TextSnapshot, Theme.Behavior,
                View.Options.GetOptionValue(DefaultOptions.IndentSizeOptionId),
                View.Options.GetOptionValue(DefaultOptions.TabSizeOptionId));
            Analysis.Reset();

            GlobalVisible = service.Visible;
            service.VisibleChanged += new EventHandler(Service_VisibleChanged);
        }

        /// <summary>
        /// Raised when the global visibility property is updated.
        /// </summary>
        void Service_VisibleChanged(object sender, EventArgs e) {
            GlobalVisible = ((IIndentGuide)sender).Visible;
            InvalidateLines();
            UpdateAdornments();
        }

        /// <summary>
        /// Raised when a view option changes.
        /// </summary>
        void View_OptionChanged(object sender, EditorOptionChangedEventArgs e) {
            if (e.OptionId == DefaultOptions.IndentSizeOptionId.Name) {
                Analysis = new DocumentAnalyzer(Analysis.Snapshot, Theme.Behavior,
                    View.Options.GetOptionValue(DefaultOptions.IndentSizeOptionId),
                    View.Options.GetOptionValue(DefaultOptions.TabSizeOptionId));
                GuideBrushCache.Clear();

                InvalidateLines();
                UpdateAdornments();
            }
        }

        /// <summary>
        /// Raised when the theme is updated.
        /// </summary>
        void Service_ThemesChanged(object sender, EventArgs e) {
            var service = (IIndentGuide)sender;
            if (!service.Themes.TryGetValue(View.TextDataModel.ContentType.DisplayName, out Theme))
                Theme = service.DefaultTheme;

            Analysis = new DocumentAnalyzer(Analysis.Snapshot, Theme.Behavior,
                View.Options.GetOptionValue(DefaultOptions.IndentSizeOptionId),
                View.Options.GetOptionValue(DefaultOptions.TabSizeOptionId));
            GuideBrushCache.Clear();

            InvalidateLines();
            UpdateAdornments();
        }

        /// <summary>
        /// Raised when the display changes.
        /// </summary>
        void View_LayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
            Analysis.Update();
            UpdateAdornments();
        }

        /// <summary>
        /// Removes all lines from the line cache.
        /// </summary>
        void InvalidateLines() {
            if (Layer != null) Layer.RemoveAllAdornments();
            Analysis.Reset();
            CachedLefts.Clear();
        }

        /// <summary>
        /// Recreates all adornments.
        /// </summary>
        void UpdateAdornments() {
            Debug.Assert(View != null);
            Debug.Assert(Layer != null);
            Debug.Assert(View.TextViewLines != null);
            if (View == null || Layer == null || View.TextViewLines == null) return;

            Layer.RemoveAllAdornments();

            if (!GlobalVisible)
                return;

            double spaceWidth = View.TextViewLines.Select(line => line.VirtualSpaceWidth).FirstOrDefault();
            if (spaceWidth <= 0.0) return;

            CaretHandlerBase caret;
            caret = new CaretNearestLeft(View.Caret.Position.VirtualBufferPosition, Analysis.TabSize, 2);

            foreach (var line in Analysis.Lines) {
                int linePos = line.Indent;
                if (!Analysis.Behavior.VisibleUnaligned && (linePos % Analysis.IndentSize) != 0)
                    continue;
                var firstLine = View.TextSnapshot.GetLineFromLineNumber(line.FirstLine);
                var lastLine = View.TextSnapshot.GetLineFromLineNumber(line.LastLine);

                int formatIndex = line.Indent / Analysis.IndentSize;

                if (line.Indent % Analysis.IndentSize != 0)
                    formatIndex = IndentTheme.UnalignedFormatIndex;

                var viewModel = View.TextViewModel;
                if ((viewModel == null ||
                    !viewModel.IsPointInVisualBuffer(firstLine.Start, PositionAffinity.Successor) ||
                    !viewModel.IsPointInVisualBuffer(lastLine.End, PositionAffinity.Predecessor)) ||
                    firstLine.Start > View.TextViewLines.LastVisibleLine.Start ||
                    lastLine.Start < View.TextViewLines.FirstVisibleLine.Start) {
                    continue;
                }

                caret.AddLine(line, willUpdateImmediately: true);

                IWpfTextViewLine firstView, lastView;
                try {
                    firstView = View.GetTextViewLineContainingBufferPosition(firstLine.Start);
                    lastView = View.GetTextViewLineContainingBufferPosition(lastLine.End);
                } catch (Exception ex) {
                    Trace.WriteLine("UpdateAdornments GetTextViewLineContainingBufferPosition failed");
                    Trace.WriteLine(" - Exception: " + ex.ToString());
                    continue;
                }

                double top = (firstView.VisibilityState != VisibilityState.Unattached) ?
                    firstView.Top :
                    View.TextViewLines.FirstVisibleLine.Top;
                double bottom = (lastView.VisibilityState != VisibilityState.Unattached) ?
                    lastView.Bottom :
                    View.TextViewLines.LastVisibleLine.Bottom;
                double left = line.Indent * spaceWidth +
                    ((firstView.VisibilityState == VisibilityState.FullyVisible) ?
                    firstView.TextLeft : View.TextViewLines.FirstVisibleLine.TextLeft);

                line.Adornment = CreateGuide(top, bottom, left);
                line.FormatIndex = formatIndex;
                line.Span = new SnapshotSpan(firstLine.Start, lastLine.End);
                UpdateGuide(line);

                Layer.AddAdornment(line);
            }

            foreach (var line in caret.GetModified()) {
                UpdateGuide(line);
            }
        }

        /// <summary>
        /// Adds a guideline at the specified location.
        /// </summary>
        /// <param name="firstLine">The line to start the guide at.</param>
        /// <param name="lastLine">The line to end the guide at.</param>
        /// <param name="indent">The indent number.</param>
        /// </param>
        /// <returns>The added line.</returns>
        private Line CreateGuide(double top, double bottom, double left) {
            if (left == 0 || left > View.ViewportWidth) return null;

            var guide = new Line() {
                X1 = left,
                Y1 = top,
                X2 = left,
                Y2 = bottom,
                StrokeThickness = 1.0,
                StrokeDashOffset = top,
                SnapsToDevicePixels = true
            };

            return guide;
        }

        /// <summary>
        /// Updates the line <paramref name="guide"/> with a new format.
        /// </summary>
        /// <param name="guide">The <see cref="Line"/> to update.</param>
        /// <param name="formatIndex">The new format index.</param>
        void UpdateGuide(LineSpan lineSpan) {
            if (lineSpan == null) return;
            var guide = lineSpan.Adornment as Line;
            if (guide == null) return;


            LineFormat format;
            if (!Theme.LineFormats.TryGetValue(lineSpan.FormatIndex, out format))
                format = Theme.DefaultLineFormat;

            if (!format.Visible) {
                guide.Visibility = System.Windows.Visibility.Hidden;
                return;
            }

            var lineColor = lineSpan.Highlight ? format.HighlightColor : format.LineColor;
            var lineStyle = lineSpan.Highlight ? format.HighlightStyle : format.LineStyle;

            Brush brush;
            if (!GuideBrushCache.TryGetValue(lineColor, out brush)) {
                brush = new SolidColorBrush(lineColor.ToSWMC());
                if (brush.CanFreeze) brush.Freeze();
                GuideBrushCache[format.LineColor] = brush;
            }

            guide.Visibility = System.Windows.Visibility.Visible;
            guide.Stroke = brush;
            guide.StrokeThickness = lineStyle.GetStrokeThickness();
            guide.StrokeDashArray = lineStyle.GetStrokeDashArray();
        }

        /// <summary>
        /// Raised when the caret position changes.
        /// </summary>
        void Caret_PositionChanged(object sender, CaretPositionChangedEventArgs e) {
            int tabSize = e.TextView.Options.GetOptionValue(DefaultOptions.TabSizeOptionId);
            
            CaretHandlerBase caret;
            caret = new CaretNearestLeft(e.NewPosition.VirtualBufferPosition, tabSize, 2);

            foreach (var line in Analysis.Lines) {
                int linePos = line.Indent;
                if (!Analysis.Behavior.VisibleUnaligned && (linePos % Analysis.IndentSize) != 0)
                    continue;
                var firstLine = View.TextSnapshot.GetLineFromLineNumber(line.FirstLine);
                var lastLine = View.TextSnapshot.GetLineFromLineNumber(line.LastLine);

                int formatIndex = line.Indent / Analysis.IndentSize;

                if (line.Indent % Analysis.IndentSize != 0)
                    formatIndex = IndentTheme.UnalignedFormatIndex;

                caret.AddLine(line, willUpdateImmediately: false);
            }

            foreach (var line in caret.GetModified()) {
                UpdateGuide(line);
            }
        }
    }
}
