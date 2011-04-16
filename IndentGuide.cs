using System;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.Collections.Generic;

namespace IndentGuide
{
    public class IndentGuide
    {
        IAdornmentLayer Layer;
        IWpfTextView View;
        Brush GuideBrush;

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

            foreach (ITextViewLine _line in e.NewOrReformattedLines)
            {
                var line = _line;
                var snapshot = line.Snapshot;
                if (line.Length == 0)
                {
                    int lineNumber = line.Snapshot.GetLineNumberFromPosition(line.Start.Position);
                    if (lineNumber > snapshot.LineCount) continue;

                    for (int i = lineNumber - 1;
                        i >= 0;
                        i -= 1)
                    {
                        var line2 = snapshot.GetLineFromLineNumber(i);
                        if (line2.Length > 0)
                        {
                            try
                            {
                                line = View.TextViewLines.GetTextViewLineContainingBufferPosition(line2.Start);
                            }
                            catch (ArgumentOutOfRangeException)
                            { }
                            break;
                        }
                    }
                    if (line == null) continue;
                }

                if (line.Length > 0)
                {
                    int actualPos = 0;
                    int spaceCount = tabSize;
                    int end = line.End;
                    for (int i = line.Start; i <= end; ++i)
                    {
                        char c = i == end ? ' ' : snapshot[i];

                        if (actualPos > 0 && (actualPos % tabSize) == 0 && char.IsWhiteSpace(c) &&
                            snapshot.Length > i)
                        {
                            double left = GetGuideLeft(snapshot, i);
                            AddGuide(_line, left);
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

        private double GetGuideLeft(ITextSnapshot snapshot, int i)
        {
            var span = new SnapshotSpan(snapshot, i, 1);
            var marker = View.TextViewLines.GetMarkerGeometry(span);
            return marker.Bounds.Left;
        }
        
        private void AddGuide(ITextViewLine line, double left)
        {
            var guide = new System.Windows.Shapes.Line()
            {
                X1 = left,
                Y1 = line.Top,
                X2 = left,
                Y2 = line.Bottom,
                Stroke = GuideBrush,
                StrokeThickness = 1.0,
                SnapsToDevicePixels = true
            };

            Layer.AddAdornment(new SnapshotSpan(line.Start, line.End), null, guide);
        }
    }
}
