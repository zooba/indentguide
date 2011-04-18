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
    public class IndentGuide
    {
        IAdornmentLayer Layer;
        IWpfTextView View;
        Brush GuideBrush;
        bool Visible;

        public IndentGuide(IWpfTextView view, IEditorFormatMapService formatMapService, IEditorOptions options)
        {
            View = view;
            View.LayoutChanged += OnLayoutChanged;

            Layer = view.GetAdornmentLayer("IndentGuide");

            var formatMap = formatMapService.GetEditorFormatMap(View);
            UpdateFormat(formatMap);
            formatMap.FormatMappingChanged += new EventHandler<FormatItemsEventArgs>(OnFormatMappingChanged);

            UpdateVisibility(options);
            options.OptionChanged += new EventHandler<EditorOptionChangedEventArgs>(OnOptionChanged);
        }


        /// <summary>
        /// On option change update visibility.
        /// </summary>
        void OnOptionChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (e.OptionId == IndentGuideVisibilityOption.OptionKey.Name)
            {
                UpdateVisibility(sender as IEditorOptions);
            }
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

        void UpdateVisibility(IEditorOptions options)
        {
            Debug.Assert(options != null);
            if (options == null) return;

            Visible = options.GetOptionValue(IndentGuideVisibilityOption.OptionKey);
            if (Visible)
            {
                UpdateAdornments();
            }
            else
            {
                if (Layer != null) Layer.RemoveAllAdornments();
            }
        }

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

        void UpdateAdornments()
        {
            Debug.Assert(View != null);
            Debug.Assert(Layer != null);
            if (View == null || Layer == null) return;
            if (View.Options == null || View.TextViewLines == null) return;

            if (!Visible)
            {
                Layer.RemoveAllAdornments();
                return;
            }

            int tabSize = View.Options.GetOptionValue(DefaultOptions.TabSizeOptionId);

            var lastGuidesAt = new List<double>();

            foreach (var line in View.TextViewLines)
            {
                if (line.Length == 0)
                {
                    foreach (var left in lastGuidesAt)
                    {
                        AddGuide(line, left);
                    }
                }
                else
                {
                    lastGuidesAt.Clear();
                    var snapshot = line.Snapshot;
                    int actualPos = 0;
                    int spaceCount = tabSize;
                    int end = line.End;
                    for (int i = line.Start; i <= end; ++i)
                    {
                        char c = i == end ? ' ' : snapshot[i];

                        if (actualPos > 0 && (actualPos % tabSize) == 0 && char.IsWhiteSpace(c) &&
                            snapshot.Length > i)
                        {
                            var span = new SnapshotSpan(snapshot, i, 1);
                            double left = View.TextViewLines.GetMarkerGeometry(span).Bounds.Left;
                            lastGuidesAt.Add(left);
                            AddGuide(line, left);
                        }

                        if (c == '\t')
                            actualPos = ((actualPos / tabSize) + 1) * tabSize;
                        else if (c == ' ')
                            actualPos += 1;
                        else
                            break;
                    }
                }
            }
        }

        private void AddGuide(ITextViewLine line, double left)
        {
            var guide = new Line()
            {
                X1 = left,
                Y1 = line.Top,
                X2 = left,
                Y2 = line.Bottom,
                Stroke = GuideBrush,
                StrokeThickness = 1.0,
                StrokeDashArray = new DoubleCollection { 1.0, 1.0 },
                StrokeDashOffset = line.Top,
                SnapsToDevicePixels = true
            };

            Layer.AddAdornment(new SnapshotSpan(line.Start, line.End), null, guide);
        }
    }
}
