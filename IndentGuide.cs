using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Editor;

namespace IndentGuide
{
    public class IndentGuide
    {
        [Import(typeof(IVsFontsAndColorsInformationService))]
        internal IVsFontsAndColorsInformationService FontsAndColors = null;

        IAdornmentLayer Layer;
        IWpfTextView View;
        Pen GuidePen;

        private static readonly Color LineColor = Colors.LightGray;

        public IndentGuide(IWpfTextView view)
        {
            View = view;
            View.LayoutChanged += OnLayoutChanged;

            Layer = view.GetAdornmentLayer("IndentGuide");

            Brush penBrush = new SolidColorBrush(Colors.LightGray);
            penBrush.Freeze();
            GuidePen = new Pen(penBrush, 1.0);
            GuidePen.Freeze();
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
                for (int i = line.Start; i < line.End; ++i)
                {
                    char c = snapshot[i];

                    if (actualPos > 0 && (actualPos % tabSize) == 0 && char.IsWhiteSpace(c))
                    {
                        var span = new SnapshotSpan(snapshot, i, 1);
                        var marker = View.TextViewLines.GetMarkerGeometry(span);
                        var geometry = new LineGeometry(
                            new Point(0.0, 0.0),
                            new Point(0.0, marker.Bounds.Height));
                        if (geometry.CanFreeze) geometry.Freeze();
                        var drawing = new GeometryDrawing(null, GuidePen, geometry);
                        drawing.Freeze();
                        var drawingImage = new DrawingImage(drawing);
                        drawingImage.Freeze();
                        var image = new Image { Source = drawingImage };

                        Canvas.SetLeft(image, marker.Bounds.Left);
                        Canvas.SetTop(image, marker.Bounds.Top);

                        Layer.AddAdornment(AdornmentPositioningBehavior.TextRelative,
                            span,
                            null,
                            image,
                            null);
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
