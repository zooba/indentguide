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

            if (!GlobalVisible)
            {
                Layer.RemoveAllAdornments();
                return;
            }

            var lines = View.TextViewLines;

            Layer.RemoveAllAdornments();
            int firstLineNumber = View.TextSnapshot.GetLineNumberFromPosition(View.TextViewLines.First().Start.Position);
            int lastLineNumber = View.TextSnapshot.GetLineNumberFromPosition(View.TextViewLines.Last().Start.Position);
            foreach (var line in Analysis.Lines)
            {
                if (line.Type == LineSpanType.AtText && !Theme.VisibleAtText)
                    continue;
                if (line.Type == LineSpanType.EmptyLineAtText && 
                    !Theme.VisibleAtText &&
                    (Theme.EmptyLineMode == EmptyLineMode.SameAsLineAboveActual ||
                     Theme.EmptyLineMode == EmptyLineMode.SameAsLineBelowActual))
                    continue;


                IWpfTextViewLine first = null, last = null;
                if (line.First > firstLineNumber && line.First <= lastLineNumber)
                    first = View.TextViewLines[line.First - firstLineNumber];
                else if (line.First <= firstLineNumber)
                    first = View.TextViewLines[0];

                if (line.Last >= firstLineNumber && line.Last < lastLineNumber)
                    last = View.TextViewLines[line.Last - firstLineNumber];
                else if (line.Last >= lastLineNumber)
                    last = View.TextViewLines[View.TextViewLines.Count - 1];

                if(first != null && last != null)
                    AddGuide(first, last, line.Indent, line.Indent / Analysis.IndentSize, line);
            }
        }

        /// <summary>
        /// Adds a guideline at the specified location.
        /// </summary>
        /// <param name="firstLine">The line to add the guide for.</param>
        /// <param name="indent">The indent number.</param>
        /// <param name="format">The format index for this guide.
        /// </param>
        /// <param name="tag">The tag to associate with the created
        /// adornment.</param>
        private void AddGuide(IWpfTextViewLine firstLine, IWpfTextViewLine lastLine, int indent, int formatIndex, object tag)
        {
            double left;
            if (!CachedLefts.TryGetValue(indent, out left))
            {
                left = firstLine.GetCharacterBounds(new VirtualSnapshotPoint(firstLine.Start.GetContainingLine(), indent)).Left;
                CachedLefts[indent] = left;
            }

            if (left == 0 || left > View.ViewportWidth) return;

            LineFormat format;
            Brush brush;
            if (!Theme.NumberedOverride.TryGetValue(formatIndex, out format))
                format = Theme.DefaultLineFormat;

            if (!format.Visible) return;

            if (!GuideBrushCache.TryGetValue(format.LineColor, out brush))
            {
                brush = new SolidColorBrush(format.LineColor.ToSWMC());
                if (brush.CanFreeze) brush.Freeze();
                GuideBrushCache[format.LineColor] = brush;
            }

            var guide = new Line()
            {
                X1 = left,
                Y1 = firstLine.Top,
                X2 = left,
                Y2 = lastLine.Bottom,
                Stroke = brush,
                StrokeThickness = 1.0,
                StrokeDashOffset = firstLine.Top,
                SnapsToDevicePixels = true
            };

            if (format.LineStyle == LineStyle.Thick)
                guide.StrokeThickness = 3.0;
            else if (format.LineStyle == LineStyle.Dotted)
                guide.StrokeDashArray = new DoubleCollection { 1.0, 2.0 };
            else if (format.LineStyle == LineStyle.Dashed)
                guide.StrokeDashArray = new DoubleCollection { 3.0, 3.0 };

            SnapshotSpan span;
            span = new SnapshotSpan(firstLine.Start, lastLine.End);

            Layer.AddAdornment(span, null, guide);
        }
    }
}
