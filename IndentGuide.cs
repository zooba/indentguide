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
        bool GlobalVisible;

        struct GuidePositions
        {
            public GuidePositions(bool original, List<int> tabs)
            {
                Original = original;
                Tabs = tabs;
            }
            public GuidePositions Clone()
            {
                return new GuidePositions(false, Tabs);
            }
            public bool Original;
            public List<int> Tabs;
        }
        Dictionary<int, GuidePositions> ActiveLines;
        Dictionary<int, double> CachedLefts;

        /// <summary>
        /// Instantiates a new indent guide manager for a view.
        /// </summary>
        /// <param name="view">The text view to provide guides for.</param>
        /// <param name="service">The Indent Guide service.</param>
        public IndentGuideView(IWpfTextView view, IIndentGuide service, string themeName)
        {
            ActiveLines = new Dictionary<int, GuidePositions>();
            CachedLefts = new Dictionary<int, double>();

            View = view;
            View.LayoutChanged += View_LayoutChanged;

            Layer = view.GetAdornmentLayer("IndentGuide");

            if (!service.Themes.TryGetValue(themeName, out Theme))
                Theme = service.DefaultTheme;
            service.ThemesChanged += new EventHandler(Service_ThemesChanged);

            GlobalVisible = service.Visible;
            service.VisibleChanged += new EventHandler(Service_VisibleChanged);

            GuideBrush = new SolidColorBrush(Theme.LineFormat.LineColor.ToSWMC());
            if (GuideBrush.CanFreeze) GuideBrush.Freeze();
        }

        /// <summary>
        /// Raised when the global visibility property is updated.
        /// </summary>
        void Service_VisibleChanged(object sender, EventArgs e)
        {
            GlobalVisible = ((IIndentGuide)sender).Visible;
            ActiveLines.Clear();
            CachedLefts.Clear();
            UpdateAdornments();
        }

        /// <summary>
        /// Raised when the theme is updated.
        /// </summary>
        void Service_ThemesChanged(object sender, EventArgs e)
        {
            var service = (IIndentGuide)sender;
            if (!service.Themes.TryGetValue(Theme.Name, out Theme))
                Theme = service.DefaultTheme;
            
            GuideBrush = new SolidColorBrush(Theme.LineFormat.LineColor.ToSWMC());
            if (GuideBrush.CanFreeze) GuideBrush.Freeze();

            ActiveLines.Clear();
            CachedLefts.Clear();
            UpdateAdornments();
        }

        /// <summary>
        /// Raised when the display changes.
        /// </summary>
        void View_LayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            foreach (var line in e.NewOrReformattedLines)
            {
                int lineNumber = line.Snapshot.GetLineNumberFromPosition(line.Start.Position);
                ActiveLines.Remove(lineNumber);
            }
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

            if (!Theme.LineFormat.Visible || !GlobalVisible)
            {
                Layer.RemoveAllAdornments();
                return;
            }

            int tabSize = View.Options.GetOptionValue(DefaultOptions.TabSizeOptionId);
            var lines = View.TextViewLines.Cast<IWpfTextViewLine>().ToList();
            var emptyLines = new List<int>();
            var newLines = new List<int>();

            foreach (var line in lines)
            {
                int lineNumber = line.Snapshot.GetLineNumberFromPosition(line.Start.Position);
                if (ActiveLines.ContainsKey(lineNumber)) continue;

                if (line.IsEmpty())
                {
                    emptyLines.Add(lineNumber);
                }
                else
                {
                    newLines.Add(lineNumber);
                    ActiveLines[lineNumber] = new GuidePositions(true, GetIndentLocations(tabSize, line));
                }
            }

            if (Theme.EmptyLineMode != EmptyLineMode.NoGuides)
            {
                foreach (var line_i in emptyLines)
                {
                    int source_i = line_i;
                    if (Theme.EmptyLineMode == EmptyLineMode.SameAsLineAboveActual ||
                        Theme.EmptyLineMode == EmptyLineMode.SameAsLineAboveLogical)
                    {
                        while (!ActiveLines.ContainsKey(source_i) && source_i > 0)
                            source_i -= 1;
                    }
                    else
                    {
                        while (!ActiveLines.ContainsKey(source_i) && source_i < View.TextSnapshot.LineCount)
                            source_i += 1;
                    }
                    
                    if (ActiveLines.ContainsKey(source_i))
                    {
                        ActiveLines[line_i] = ActiveLines[source_i].Clone();
                        newLines.Add(line_i);
                    }
                }
            }

            foreach (var line_i in newLines)
            {
                Layer.RemoveAdornmentsByTag(line_i);
                var gp = ActiveLines[line_i];
                if (!gp.Tabs.Any()) continue;

                var pos = View.TextSnapshot.GetLineFromLineNumber(line_i).Start;
                var line = View.TextViewLines.GetTextViewLineContainingBufferPosition(pos);
                var tabsRepeat = new List<int>(gp.Tabs);

                if (gp.Original ||
                    Theme.EmptyLineMode == EmptyLineMode.SameAsLineAboveActual ||
                    Theme.EmptyLineMode == EmptyLineMode.SameAsLineBelowActual)
                {
                    tabsRepeat.Remove(gp.Tabs.Max());
                }

                while (tabsRepeat.Any())
                {
                    foreach (var tab in tabsRepeat.ToList())
                    {
                        AddGuide(line, tab, tab / tabSize, line_i);
                        tabsRepeat.Remove(tab);
                    }
                }
            }
        }

        private List<int> GetIndentLocations(int tabSize, IWpfTextViewLine line)
        {
            var locations = new List<int>();
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
                    if (!CachedLefts.ContainsKey(actualPos))
                    {
                        var span = new SnapshotSpan(snapshot, i, 1);
                        double left = View.TextViewLines.GetMarkerGeometry(span).Bounds.Left;
                        CachedLefts[actualPos] = left;
                    }
                    locations.Add(actualPos);
                }

                if (c == '\t')
                    actualPos = ((actualPos / tabSize) + 1) * tabSize;
                else if (c == ' ')
                    actualPos += 1;
                else
                    break;
            }

            if (actualPos > 0 && (actualPos % tabSize) != 0) locations.Add(actualPos);

            return locations;
        }

        /// <summary>
        /// Adds a guideline at the specified location.
        /// </summary>
        /// <param name="line">The line to add the guide for.</param>
        /// <param name="indent">The indent number.</param>
        /// <param name="format">The format index for this guide.
        /// </param>
        /// <param name="tag">The tag to associate with the created
        /// adornment.</param>
        private void AddGuide(ITextViewLine line, int indent, int format, object tag)
        {
            double left;
            if (!CachedLefts.TryGetValue(indent, out left)) return;

            if (left == 0 || left > View.ViewportWidth) return;

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

            if (Theme.LineFormat.LineStyle == LineStyle.Thick)
                guide.StrokeThickness = 3.0;
            else if (Theme.LineFormat.LineStyle == LineStyle.Dotted)
                guide.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            else if (Theme.LineFormat.LineStyle == LineStyle.Dashed)
                guide.StrokeDashArray = new DoubleCollection { 3.0, 3.0 };

            SnapshotSpan span;
            span = new SnapshotSpan(line.Start, line.End);

            Layer.AddAdornment(span, tag, guide);
        }
    }
}
