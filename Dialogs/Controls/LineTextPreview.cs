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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace IndentGuide {
    public partial class LineTextPreview : UserControl {
        private DocumentAnalyzer Analysis;
        
        public LineTextPreview() {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw | ControlStyles.Opaque, true);

            InitializeComponent();

            _IndentSize = 4;
            _Theme = null;
            Analysis = null;
        }

        private bool _Checked;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Checked {
            get { return _Checked; }
            set {
                if (_Checked != value) {
                    _Checked = value;
                    Invalidate();
                }
            }
        }

        private bool _VisibleWhitespace;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool VisibleWhitespace {
            get { return _VisibleWhitespace; }
            set {
                if (_VisibleWhitespace != value) {
                    _VisibleWhitespace = value;
                    Invalidate();
                }
            }
        }

        private int _IndentSize;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int IndentSize {
            get { return _IndentSize; }
            set {
                if (_IndentSize != value) {
                    _IndentSize = value;
                    Refresh();
                }
            }
        }

        private Color _HighlightBackColor;
        public Color HighlightBackColor {
            get { return _HighlightBackColor; }
            set {
                if (_HighlightBackColor != value) {
                    _HighlightBackColor = value;
                    Invalidate();
                }
            }
        }

        private bool ShouldSerializeHighlightBackColor() {
            return !HighlightForeColor.IsSystemColor || HighlightBackColor != SystemColors.Highlight;
        }
        public void ResetHighlightBackColor() {
            HighlightBackColor = SystemColors.Highlight;
        }

        private Color _HighlightForeColor;
        public Color HighlightForeColor {
            get { return _HighlightForeColor; }
            set {
                if (_HighlightForeColor != value) {
                    _HighlightForeColor = value;
                    Invalidate();
                }
            }
        }

        private bool ShouldSerializeHighlightForeColor() {
            return !HighlightForeColor.IsSystemColor || HighlightForeColor != SystemColors.HighlightText;
        }
        public void ResetHighlightForeColor() {
            HighlightForeColor = SystemColors.HighlightText;
        }


        private IndentTheme _Theme;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IndentTheme Theme {
            get { return _Theme; }
            set { _Theme = value; Refresh(); }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text {
            get { return base.Text; }
            set { base.Text = (value ?? ""); }
        }

        protected override void OnTextChanged(EventArgs e) {
            base.OnTextChanged(e);
            Refresh();
        }

        public override void Refresh() {
            base.Refresh();

            if (Theme != null && Theme.Behavior != null && IsHandleCreated) {
                var snapshot = new FakeSnapshot(Text);
                Analysis = new DocumentAnalyzer(snapshot, Theme.Behavior, IndentSize, IndentSize);
                Analysis.Reset().ContinueWith(t => { BeginInvoke((Action)Invalidate); });
            }
        }

        private Pen GetLinePen(int formatIndex, out bool isGlowing) {
            LineFormat format;

            if (!Theme.LineFormats.TryGetValue(formatIndex, out format))
                format = Theme.DefaultLineFormat;

            isGlowing = format.LineStyle.HasFlag(LineStyle.Glow);

            if (!format.Visible) return null;

            var pen = new Pen(format.LineColor, (float)format.LineStyle.GetStrokeThickness());

            pen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;

            var pattern = format.LineStyle.GetDashPattern();
            if (pattern == null) {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            } else {
                pen.DashPattern = pattern;
            }
            return pen;
        }

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.Clear(!Checked ? BackColor : HighlightBackColor);

            Color foreColor = !Checked ? ForeColor : HighlightForeColor;

            double spaceLeft, spaceWidth;
            using (var sf = new StringFormat()) {
                RectangleF rect = ClientRectangle;
                rect.Width *= 2.0f;

                sf.SetMeasurableCharacterRanges(new[] { new CharacterRange(0, 16) });

                var testString = VisibleWhitespace ? "················{" : "                {";
                var rgns = e.Graphics.MeasureCharacterRanges(testString, Font, rect, sf);
                var bounds = rgns[0].GetBounds(e.Graphics);
                spaceLeft = bounds.Left;
                spaceWidth = bounds.Width / 16;

#if DEBUG
                sf.Alignment = StringAlignment.Far;
                rect.Width /= 2;
                using (var brush = new SolidBrush(foreColor))
                    e.Graphics.DrawString(string.Format("{0}, {1}", spaceLeft, spaceWidth), Font, brush, rect, sf);
#endif
            }

            try {
                if (Analysis == null || Analysis.Snapshot == null) {
                    return;
                }

                using (var brush = new SolidBrush(foreColor)) {
                    foreach (var line in Analysis.Snapshot.Lines) {
                        var text = line.GetText();
                        if (VisibleWhitespace) {
                            text = text.Replace(' ', '·');
                        }
                        e.Graphics.DrawString(text, Font, brush, 0, line.LineNumber * Font.Height);
                    }
                }

                if (Theme == null || Analysis.Lines == null) {
                    return;
                }

                foreach (var line in Analysis.Lines) {
                    float top = line.FirstLine * Font.Height;
                    float bottom = (line.LastLine + 1) * Font.Height;
                    float left = (float)Math.Floor(line.Indent * spaceWidth + spaceLeft);

                    bool glow;
                    using (var pen = GetLinePen(line.FormatIndex, out glow)) {
                        if (pen != null) {
                            if (glow) {
                                using (var transparentBrush = new SolidBrush(Color.FromArgb(24, pen.Color))) {
                                    for (int i = 1; i < LineStyle.Thick.GetStrokeThickness(); ++i) {
                                        e.Graphics.FillRectangle(transparentBrush, left - i, top, i + i + 1, bottom - top);
                                    }
                                }
                            }
                            e.Graphics.DrawLine(pen, left, top, left, bottom);
                        }
                    }
                }
            } catch (Exception ex) {
                Trace.WriteLine("LineTextPreview::OnPaint Error");
                Trace.WriteLine(" - Exception: " + ex.ToString());
                using (var brush = new SolidBrush(foreColor)) {
                    e.Graphics.DrawString(ex.ToString(), Font, brush, 0.0f, 0.0f);
                }
            }
        }
    }
}
