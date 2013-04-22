/* ****************************************************************************
 * Copyright 2012 Steve Dower
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy 
 * of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 * ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        /// <summary>
        /// Instantiates a new indent guide manager for a view.
        /// </summary>
        /// <param name="view">The text view to provide guides for.</param>
        /// <param name="service">The Indent Guide service.</param>
        public IndentGuideView(IWpfTextView view, IIndentGuide service) {
            GuideBrushCache = new Dictionary<System.Drawing.Color, Brush>();

            View = view;
            View.Caret.PositionChanged += Caret_PositionChanged;
            View.LayoutChanged += View_LayoutChanged;
            View.Options.OptionChanged += View_OptionChanged;

            Layer = view.GetAdornmentLayer("IndentGuide");

            if (!service.Themes.TryGetValue(View.TextDataModel.ContentType.DisplayName, out Theme)) {
                Theme = service.DefaultTheme;
            }
            Debug.Assert(Theme != null, "No themes loaded");
            if (Theme == null) {
                Theme = new IndentTheme();
            }
            service.ThemesChanged += new EventHandler(Service_ThemesChanged);

            Analysis = new DocumentAnalyzer(view.TextSnapshot, Theme.Behavior,
                View.Options.GetOptionValue(DefaultOptions.IndentSizeOptionId),
                View.Options.GetOptionValue(DefaultOptions.TabSizeOptionId));

            GlobalVisible = service.Visible;
            service.VisibleChanged += new EventHandler(Service_VisibleChanged);
        }

        /// <summary>
        /// Raised when the global visibility property is updated.
        /// </summary>
        void Service_VisibleChanged(object sender, EventArgs e) {
            GlobalVisible = ((IIndentGuide)sender).Visible;
            UpdateAdornments(Analysis.Reset());
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

                UpdateAdornments(Analysis.Reset());
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

            UpdateAdornments(Analysis.Reset());
        }

        /// <summary>
        /// Raised when the display changes.
        /// </summary>
        void View_LayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
            UpdateAdornments(Analysis.Update());
        }

        /// <summary>
        /// Schedules an update when the provided task completes.
        /// </summary>
        void UpdateAdornments(Task task) {
            if (task != null) {
#if DEBUG
                Trace.TraceInformation("Update non-null");
#endif
                task.ContinueWith(UpdateAdornmentsCallback, TaskContinuationOptions.OnlyOnRanToCompletion);
            } else {
#if DEBUG
                Trace.TraceInformation("Update null");
#endif
                UpdateAdornments();
            }
        }

        void UpdateAdornmentsCallback(Task task) {
            UpdateAdornments();
        }

        private IEnumerable<LineSpan> GetPageWidthLines() {
            return Theme.PageWidthMarkers.Keys
                .Select(i => new LineSpan(int.MinValue, int.MaxValue, i, LineSpanType.PageWidthMarker));
        }

        /// <summary>
        /// Recreates all adornments.
        /// </summary>
        void UpdateAdornments() {
            Debug.Assert(View != null);
            Debug.Assert(Layer != null);
            Debug.Assert(View.TextViewLines != null);
            if (View == null || Layer == null || View.TextViewLines == null) return;
            if (Analysis == null) return;

            var analysisLines = Analysis.Lines;
            if (Analysis.Snapshot != View.TextSnapshot) {
                var task = Analysis.Update();
                if (task != null) {
                    UpdateAdornments(task);
                }
                return;
            } else if (analysisLines == null) {
                UpdateAdornments(Analysis.Reset());
                return;
            }

            if (!View.VisualElement.Dispatcher.CheckAccess()) {
                View.VisualElement.Dispatcher.InvokeAsync(UpdateAdornments);
                return;
            }

            // Re-check snapshot in case we had to reinvoke on the UI thread
            // and raced.
            if (Analysis.Snapshot != View.TextSnapshot) {
                var task = Analysis.Update();
                if (task != null) {
                    UpdateAdornments(task);
                    return;
                }
            }

            Layer.RemoveAllAdornments();

            if (!GlobalVisible) {
                return;
            }

            double spaceWidth = View.TextViewLines.Select(line => line.VirtualSpaceWidth).FirstOrDefault();
            if (spaceWidth <= 0.0) return;
            double horizontalOffset = View.TextViewLines.FirstVisibleLine.TextLeft;

            var caret = CaretHandlerBase.FromName(Theme.CaretHandler, View.Caret.Position.VirtualBufferPosition, Analysis.TabSize);

            foreach (var line in analysisLines.Concat(GetPageWidthLines())) {
                double top, bottom, left;

                if (line.Type == LineSpanType.PageWidthMarker) {
                    line.Highlight = (Analysis.LongestLine > line.Indent);

                    top = View.ViewportTop;
                    bottom = View.ViewportBottom;
                    left = (line.Indent - 1) * spaceWidth + horizontalOffset;
                } else {
                    caret.AddLine(line, willUpdateImmediately: true);

                    top = View.TextViewLines.FirstVisibleLine.Top;
                    bottom = View.TextViewLines.LastVisibleLine.Bottom;
                    left = line.Indent * spaceWidth + horizontalOffset;
                }

                if (line.FirstLine >= 0 && line.LastLine < int.MaxValue) {
                    ITextSnapshotLine firstLine, lastLine;
                    try {
                        firstLine = View.TextSnapshot.GetLineFromLineNumber(line.FirstLine);
                        lastLine = View.TextSnapshot.GetLineFromLineNumber(line.LastLine);
                    } catch (Exception ex) {
                        Trace.TraceError("In GetLineFromLineNumber:\n{0}", ex);
                        continue;
                    }

                    var viewModel = View.TextViewModel;
                    if ((viewModel == null ||
                        !viewModel.IsPointInVisualBuffer(firstLine.End, PositionAffinity.Successor) ||
                        !viewModel.IsPointInVisualBuffer(lastLine.Start - (line.LastLine == 0 ? 0 : 1), PositionAffinity.Predecessor)) ||
                        firstLine.Start > View.TextViewLines.LastVisibleLine.Start ||
                        lastLine.Start < View.TextViewLines.FirstVisibleLine.Start) {
                        continue;
                    }

                    IWpfTextViewLine firstView, lastView;
                    try {
                        firstView = View.GetTextViewLineContainingBufferPosition(firstLine.Start);
                        lastView = View.GetTextViewLineContainingBufferPosition(lastLine.End);
                    } catch (Exception ex) {
                        Trace.TraceError("UpdateAdornments GetTextViewLineContainingBufferPosition failed\n{0}", ex);
                        continue;
                    }

                    if (firstView.VisibilityState != VisibilityState.Unattached) {
                        top = firstView.Top;
                    }
                    if (lastView.VisibilityState != VisibilityState.Unattached) {
                        bottom = lastView.Bottom;
                    }
                    if (firstView.VisibilityState == VisibilityState.FullyVisible) {
                        left = line.Indent * spaceWidth + firstView.TextLeft;
                    }

                    line.Span = new SnapshotSpan(firstLine.Start, lastLine.End);
                }

                line.Adornment = CreateGuide(top, bottom, left);
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
        /// <returns>The added line.</returns>
        private Line CreateGuide(double top, double bottom, double left) {
            if (left == 0 || left > View.ViewportWidth) return null;

            var guide = new Line() {
#if DEBUG
                X1 = left - 1,
                X2 = left + 1,
#else
                X1 = left,
                X2 = left,
#endif
                Y1 = top,
                Y2 = bottom,
                StrokeThickness = 1.0,
                StrokeDashOffset = top,
                SnapsToDevicePixels = true,
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
            if (lineSpan.Type == LineSpanType.PageWidthMarker) {
                if (!Theme.PageWidthMarkers.TryGetValue(lineSpan.Indent, out format)) {
                    format = Theme.DefaultLineFormat;
                }
            } else if (!Theme.LineFormats.TryGetValue(lineSpan.FormatIndex, out format)) {
                format = Theme.DefaultLineFormat;
            }

            if (!format.Visible) {
                guide.Visibility = System.Windows.Visibility.Hidden;
                return;
            }

            var lineStyle = lineSpan.Highlight ? format.HighlightStyle : format.LineStyle;
            var lineColor = (lineSpan.Highlight && !lineStyle.HasFlag(LineStyle.Glow)) ? 
                format.HighlightColor : format.LineColor;

            Brush brush;
            if (!GuideBrushCache.TryGetValue(lineColor, out brush)) {
                    brush = new SolidColorBrush(lineColor.ToSWMC());
                    if (brush.CanFreeze) brush.Freeze();
                    GuideBrushCache[lineColor] = brush;
            }

            guide.Visibility = System.Windows.Visibility.Visible;
            guide.Stroke = brush;
            guide.StrokeThickness = lineStyle.GetStrokeThickness();
            guide.StrokeDashArray = lineStyle.GetStrokeDashArray();

            if (lineStyle.HasFlag(LineStyle.Glow)) {
                guide.Effect = new System.Windows.Media.Effects.DropShadowEffect {
                    Color = (lineSpan.Highlight ? format.HighlightColor : format.LineColor).ToSWMC(),
                    BlurRadius = LineStyle.Thick.GetStrokeThickness(),
                    Opacity = 1.0,
                    ShadowDepth = 0.0,
                    RenderingBias = System.Windows.Media.Effects.RenderingBias.Performance
                };
            } else {
                guide.Effect = null;
            }
        }

        /// <summary>
        /// Raised when the caret position changes.
        /// </summary>
        void Caret_PositionChanged(object sender, CaretPositionChangedEventArgs e) {
            var analysisLines = Analysis.Lines;
            if (analysisLines == null) return;
            var caret = CaretHandlerBase.FromName(Theme.CaretHandler, e.NewPosition.VirtualBufferPosition, Analysis.TabSize);

            foreach (var line in analysisLines) {
                int linePos = line.Indent;
                if (!Analysis.Behavior.VisibleUnaligned && (linePos % Analysis.IndentSize) != 0)
                    continue;

                int formatIndex = line.Indent / Analysis.IndentSize;

                if (line.Indent % Analysis.IndentSize != 0) {
                    formatIndex = IndentTheme.UnalignedFormatIndex;
                }

                caret.AddLine(line, willUpdateImmediately: false);
            }

            foreach (var line in caret.GetModified()) {
                UpdateGuide(line);
            }
        }
    }
}
