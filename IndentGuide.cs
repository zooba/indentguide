using System;
using System.Collections.Generic;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.ComponentModel.Composition;

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

            Visible = options.GetOptionValue<bool>("IndentGuideVisibility");
            options.OptionChanged += new EventHandler<EditorOptionChangedEventArgs>(OnOptionChanged);

            Layer = view.GetAdornmentLayer("IndentGuide");

            var formatMap = formatMapService.GetEditorFormatMap(view);
            formatMap.FormatMappingChanged += new EventHandler<FormatItemsEventArgs>(OnFormatMappingChanged);

            var format = formatMap.GetProperties("Indent Guides");
            GuideBrush = (Brush)format[EditorFormatDefinition.ForegroundBrushId];
        }

        void OnOptionChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (e.OptionId == "IndentGuideVisibility")
            {
                var options = sender as IEditorOptions;
                if (options == null) return;

                Visible = options.GetOptionValue<bool>("IndentGuideVisibility");
                if (Visible) UpdateAdornments();
                else Layer.RemoveAllAdornments();
            }
        }

        void OnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            if (e.ChangedItems.Contains("Indent Guides"))
            {
                var formatMap = (IEditorFormatMap)sender;
                var format = formatMap.GetProperties("Indent Guides");
                GuideBrush = (SolidColorBrush)format[EditorFormatDefinition.ForegroundBrushId];
            }
        }

        /// <summary>
        /// On layout change add the adornment to any reformatted lines
        /// </summary>
        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            UpdateAdornments();
        }

        void UpdateAdornments()
        {
            if (Layer == null || View == null) return;
            
            if (!Visible)
            {
                Layer.RemoveAllAdornments();
                return;
            }

            int tabSize = View.Options.GetOptionValue(DefaultOptions.TabSizeOptionId);

            var lastGuidesAt = new List<double>();

            foreach (var line in View.TextViewLines)
            {
                var snapshot = line.Snapshot;
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
                StrokeDashArray = new DoubleCollection { 1.0, 1.0 },
                StrokeDashOffset = line.Top,
                SnapsToDevicePixels = true
            };

            Layer.AddAdornment(new SnapshotSpan(line.Start, line.End), null, guide);
        }
    }
}
