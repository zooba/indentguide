using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Editor;
using System;
using Microsoft.VisualStudio.Text.Classification;

namespace IndentGuide
{
    public class IndentGuide
    {
        IAdornmentLayer Layer;
        IWpfTextView View;
        Brush GuideBrush;

        private static readonly Color LineColor = Colors.LightGray;

        public IndentGuide(IWpfTextView view, IEditorFormatMapService formatMapService)
        {
            View = view;
            View.LayoutChanged += OnLayoutChanged;

            Layer = view.GetAdornmentLayer("IndentGuide");

            var formatMap = formatMapService.GetEditorFormatMap(view);
            formatMap.FormatMappingChanged += new EventHandler<FormatItemsEventArgs>(OnFormatMappingChanged);

            var visibleWhitespace = formatMap.GetProperties("Visible Whitespace");
            GuideBrush = (Brush)visibleWhitespace[EditorFormatDefinition.ForegroundBrushId];
        }

        void OnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            if (e.ChangedItems.Contains("Visible Whitespace"))
            {
                var formatMap = (IEditorFormatMap)sender;
                var visibleWhitespace = formatMap.GetProperties("Visible Whitespace");
                GuideBrush = (SolidColorBrush)visibleWhitespace[EditorFormatDefinition.ForegroundBrushId];
            }
        }

        /// <summary>
        /// On layout change add the adornment to any reformatted lines
        /// </summary>
        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            int tabSize = View.Options.GetOptionValue(DefaultOptions.TabSizeOptionId);

            foreach (ITextViewLine line in e.NewOrReformattedLines)
            {
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
                        var marker = View.TextViewLines.GetMarkerGeometry(span);
                        var guide = new System.Windows.Shapes.Line()
                        {
                            X1 = marker.Bounds.Left,
                            Y1 = marker.Bounds.Top,
                            X2 = marker.Bounds.Left,
                            Y2 = marker.Bounds.Bottom,
                            Stroke = GuideBrush,
                            StrokeDashArray = new DoubleCollection { 10.0, 5.0 },
                            StrokeThickness = 0.25,
                            SnapsToDevicePixels = true
                        };

                        Layer.AddAdornment(span, null, guide);
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
}
