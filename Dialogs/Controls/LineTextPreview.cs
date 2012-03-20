using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace IndentGuide {
    public partial class LineTextPreview : UserControl {
        public LineTextPreview() {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw | ControlStyles.Opaque, true);

            InitializeComponent();

            IndentSize = 4;
            _Theme = null;
        }

        private bool _Checked;
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
        public bool VisibleWhitespace {
            get { return _VisibleWhitespace; }
            set {
                if (_VisibleWhitespace != value) {
                    _VisibleWhitespace = value;
                    Invalidate();
                }
            }
        }

        public int IndentSize { get; set; }

        private IndentTheme _Theme;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IndentTheme Theme {
            get { return _Theme; }
            set { _Theme = value; Invalidate(); }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text {
            get { return base.Text; }
            set { base.Text = (value ?? ""); Invalidate(); }
        }

        protected override void OnTextChanged(EventArgs e) {
            base.OnTextChanged(e);
            Invalidate();
        }

        private Pen GetLinePen(int formatIndex) {
            LineFormat format;

            if (!Theme.LineFormats.TryGetValue(formatIndex, out format))
                format = Theme.DefaultLineFormat;

            if (!format.Visible) return null;

            Pen pen = null;

            if (!Checked) {
                pen = new Pen(format.LineColor, (float)format.LineStyle.GetStrokeThickness());
            } else {
                pen = new Pen(format.LineColor.AsInverted(), (float)format.LineStyle.GetStrokeThickness());
            }

            var pattern = format.LineStyle.GetDashPattern();
            if (pattern == null) {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            } else {
                pen.DashPattern = pattern;
            }
            return pen;
        }

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.Clear(!Checked ? BackColor : BackColor.AsInverted());

            Color foreColor = !Checked ? ForeColor : ForeColor.AsInverted();

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
                var snapshot = new FakeSnapshot(Text);
                using (var brush = new SolidBrush(foreColor)) {
                    foreach (var line in snapshot.Lines) {
                        var text = line.GetText();
                        if (VisibleWhitespace) {
                            text = text.Replace(' ', '·');
                        }
                        e.Graphics.DrawString(text, Font, brush, 0, line.LineNumber * Font.Height);
                    }
                }

                if (Theme == null) return;

                var analysis = new DocumentAnalyzer(snapshot, Theme.Behavior, IndentSize, IndentSize);

                foreach (var line in analysis.Lines) {
                    int linePos = line.Indent;
                    if (!analysis.Behavior.VisibleUnaligned && (linePos % analysis.IndentSize) != 0)
                        continue;

                    float top = line.FirstLine * Font.Height;
                    float bottom = (line.LastLine + 1) * Font.Height;
                    float left = (float)Math.Floor(line.Indent * spaceWidth + spaceLeft);

                    int formatIndex = line.Indent / analysis.IndentSize;
                    if (line.Indent % analysis.IndentSize != 0)
                        formatIndex = IndentTheme.UnalignedFormatIndex;

                    var pen = GetLinePen(formatIndex);
                    if (pen != null) {
                        e.Graphics.DrawLine(pen, left, top, left, bottom);
                        pen.Dispose();
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
