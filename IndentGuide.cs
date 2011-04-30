using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.ComponentModel.Composition;
using System.Windows.Shapes;
using System.Diagnostics;

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
        bool Visible;
        LineStyle LineStyle;
        EmptyLineMode EmptyLineMode;

        /// <summary>
        /// Instantiates a new indent guide manager for a view.
        /// </summary>
        /// <param name="view">The text view to provide guides for.</param>
        /// <param name="formatMapService">The format map provider.</param>
        /// <param name="service">The Indent Guide service.</param>
        public IndentGuideView(IWpfTextView view, IEditorFormatMapService formatMapService, IIndentGuide service)
        {
            View = view;
            View.LayoutChanged += OnLayoutChanged;

            Layer = view.GetAdornmentLayer("IndentGuide");

            var formatMap = formatMapService.GetEditorFormatMap(View);
            UpdateFormat(formatMap);
            formatMap.FormatMappingChanged += new EventHandler<FormatItemsEventArgs>(OnFormatMappingChanged);

            Visible = service.Visible;
            LineStyle = service.LineStyle;
            EmptyLineMode = service.EmptyLineMode;
            service.VisibleChanged += new EventHandler(OnVisibleChanged);
            service.LineStyleChanged += new EventHandler(OnLineStyleChanged);
            service.EmptyLineModeChanged += new EventHandler(OnEmptyLineModeChanged);
        }

        /// <summary>
        /// Updates the empty line mode and redraws all adornments.
        /// </summary>
        void OnEmptyLineModeChanged(object sender, EventArgs e)
        {
            EmptyLineMode = ((IIndentGuide)sender).EmptyLineMode;
            UpdateAdornments();
        }

        /// <summary>
        /// Updates the line style and redraws all adornments.
        /// </summary>
        void OnLineStyleChanged(object sender, EventArgs e)
        {
            LineStyle = ((IIndentGuide)sender).LineStyle;
            UpdateAdornments();
        }


        /// <summary>
        /// Updates visibility and redraws or removes all adornments.
        /// </summary>
        void OnVisibleChanged(object sender, EventArgs e)
        {
            UpdateVisibility(sender as IIndentGuide);
        }

        /// <summary>
        /// On format change update the line color.
        /// </summary>
        private void OnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            if (e.ChangedItems.Contains(IndentGuideFormat.Name))
            {
                UpdateFormat(sender as IEditorFormatMap);
            }
        }

        /// <summary>
        /// On layout change add the adornment to all visible lines.
        /// </summary>
        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            UpdateAdornments();
        }

        /// <summary>
        /// Either clears all adornments or draws adornments, depending
        /// on the current visibility value.
        /// </summary>
        void UpdateVisibility(IIndentGuide service)
        {
            Debug.Assert(service != null);
            if (service == null) return;

            Visible = service.Visible;
            if (Visible) UpdateAdornments();
            else if (Layer != null) Layer.RemoveAllAdornments();
        }

        /// <summary>
        /// Updates the color of all adornments.
        /// </summary>
        void UpdateFormat(IEditorFormatMap formatMap)
        {
            Debug.Assert(formatMap != null);
            if (formatMap == null) return;

            var format = formatMap.GetProperties(IndentGuideFormat.Name);
            GuideBrush = (SolidColorBrush)format[EditorFormatDefinition.ForegroundBrushId];
            if (Layer != null && Layer.Elements.Count > 0)
            {
                foreach (var line in Layer.Elements.Select(e => e.Adornment).OfType<Line>())
                {
                    line.Stroke = GuideBrush;
                }
            }
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

            if (!Visible) return;

            int tabSize = View.Options.GetOptionValue(DefaultOptions.TabSizeOptionId);

            var lastGuidesAt = new List<double>();
            
            var lines = View.TextViewLines.Cast<IWpfTextViewLine>().ToList();
            if (EmptyLineMode == IndentGuide.EmptyLineMode.SameAsLineBelowActual ||
                EmptyLineMode == IndentGuide.EmptyLineMode.SameAsLineBelowLogical)
            {
                lines.Reverse();
            }

            bool logical = (EmptyLineMode == IndentGuide.EmptyLineMode.SameAsLineAboveLogical ||
                EmptyLineMode == IndentGuide.EmptyLineMode.SameAsLineBelowLogical);

            foreach(var line in lines)
            {
                if (line.Length == 0)
                {
                    if (EmptyLineMode == IndentGuide.EmptyLineMode.NoGuides)
                    { }
                    else if (logical)
                    {
                        foreach (var left in lastGuidesAt)
                            AddGuide(line, left);
                    }
                    else
                    {
                        foreach (var left in lastGuidesAt.Take(lastGuidesAt.Count - 1))
                            AddGuide(line, left);
                    }
                }
                else
                {
                    lastGuidesAt = GetIndentLocations(tabSize, line);
                    foreach (var left in lastGuidesAt.Take(lastGuidesAt.Count - 1))
                        AddGuide(line, left);
                }
            }
        }

        private List<double> GetIndentLocations(int tabSize, IWpfTextViewLine line)
        {
            var locations = new List<double>();
            var snapshot = line.Snapshot;
            int actualPos = 0;
            int spaceCount = tabSize;
            int end = line.End;
            for (int i = line.Start; i <= end; ++i)
            {
                char c = i == end ? ' ' : snapshot[i];

                if (actualPos > 0 && (actualPos % tabSize) == 0 && //char.IsWhiteSpace(c) &&
                    snapshot.Length > i)
                {
                    var span = new SnapshotSpan(snapshot, i, 1);
                    double left = View.TextViewLines.GetMarkerGeometry(span).Bounds.Left;
                    locations.Add(left);
                }

                if (c == '\t')
                    actualPos = ((actualPos / tabSize) + 1) * tabSize;
                else if (c == ' ')
                    actualPos += 1;
                else
                    break;
            }

            if (actualPos > 0 && (actualPos % tabSize) != 0) locations.Add(0);

            return locations;
        }

        /// <summary>
        /// Adds a guideline at the specified location.
        /// </summary>
        /// <param name="line">The line to add the guide for.</param>
        /// <param name="left">The horizontal location to add the
        /// guide.</param>
        private void AddGuide(ITextViewLine line, double left)
        {
            if (left == 0) return;

            var guide = new Line()
            {
                X1 = left,
                Y1 = line.Top,
                X2 = left,
                Y2 = line.Bottom,
                Stroke = GuideBrush,
                StrokeThickness = 1.0,
                StrokeDashOffset = line.Top,
                SnapsToDevicePixels = true
            };

            if (LineStyle == LineStyle.Thick)
            {
                guide.StrokeThickness = 3.0;
            }
            else if (LineStyle == LineStyle.Dotted)
            {
                guide.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            }
            else if (LineStyle == LineStyle.Dashed)
            {
                guide.StrokeDashArray = new DoubleCollection { 3.0, 3.0 };
            }

            Layer.AddAdornment(new SnapshotSpan(line.Start, line.End), null, guide);
        }
    }
}
