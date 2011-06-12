using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IndentGuide
{
    public partial class LineTextPreview : UserControl
    {
        public LineTextPreview()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw | ControlStyles.Opaque, true);

            InitializeComponent();

            ShowAtText = false;
            TabSize = 4;
        }

        public bool ShowAtText { get; set; }
        public int TabSize { get; set; }
        public EmptyLineMode EmptyLineMode { get; set; }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = (value ?? ""); Invalidate(); }
        }

        private LineStyle _Style = LineStyle.Solid;
        public LineStyle Style
        {
            get
            {
                return _Style;
            }
            set
            {
                if (_Style != value)
                {
                    _Style = value;
                    LinePen = null;
                    Invalidate();
                }
            }
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            LinePen = null;
            Invalidate();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        private Pen _LinePen = null;
        private Pen LinePen
        {
            get
            {
                if (_LinePen == null)
                {
                    if (Style == LineStyle.Solid)
                    {
                        _LinePen = new Pen(ForeColor, 1.0f);
                        _LinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    }
                    else if (Style == LineStyle.Thick)
                    {
                        _LinePen = new Pen(ForeColor, 3.0f);
                        _LinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                        _LinePen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
                        _LinePen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
                    }
                    else if (Style == LineStyle.Dotted)
                    {
                        _LinePen = new Pen(ForeColor, 1.0f);
                        _LinePen.DashPattern = new float[] { 1.0f, 2.0f, 1.0f, 2.0f };
                    }
                    else if (Style == LineStyle.Dashed)
                    {
                        _LinePen = new Pen(ForeColor, 1.0f);
                        _LinePen.DashPattern = new float[] { 3.0f, 3.0f, 3.0f, 3.0f };
                    }
                }
                return _LinePen;
            }
            set { _LinePen = value; }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);

            float textY = 0.0f;
            RectangleF rect = ClientRectangle;
            rect.Width *= 2.0f;
            var sf = new StringFormat();

            try
            {
                var lines = Text.Replace("\\n", "\n").Replace("\r\n", "\n").Split('\n');
                var positions = new List<List<int>>();
                foreach (var line in lines)
                {
                    e.Graphics.DrawString(line, Font, Brushes.Black, 0.0f, textY);

                    float y1 = textY, y2 = textY + Font.Height;
                    int pos = 0;
                    var poss = new List<int>();
                    positions.Add(poss);
                    foreach (char c in line)
                    {
                        if (pos % TabSize == 0)
                        {
                            poss.Add(pos);
                            if (ShowAtText || char.IsWhiteSpace(c))
                            {
                                sf.SetMeasurableCharacterRanges(new[] { new CharacterRange(0, pos) });
                                var rgns = e.Graphics.MeasureCharacterRanges(line, Font, rect, sf);
                                var x = rgns[0].GetBounds(e.Graphics).Right;
                                if (x > 0.0f)
                                    e.Graphics.DrawLine(LinePen, x, y1, x, y2);
                            }
                        }

                        if (c == '\t')
                            pos = ((pos / TabSize) + 1) * TabSize;
                        else if (c == ' ')
                            pos += 1;
                        else
                            break;
                    }

                    textY = y2;
                }

                textY = 0.0f;
                for (int i = 0; i < lines.Length; ++i, textY += Font.Height)
                {
                    float y1 = textY, y2 = textY + Font.Height;
                    var line = lines[i];
                    if (!string.IsNullOrWhiteSpace(line)) continue;

                    var poss = positions[i];
                    if (EmptyLineMode == IndentGuide.EmptyLineMode.SameAsLineAboveActual && i > 0)
                    {
                        line = lines[i - 1];
                        poss = positions[i - 1];
                        if (!ShowAtText && poss.Count > 0) poss.RemoveAt(poss.Count - 1);
                    }
                    else if (EmptyLineMode == IndentGuide.EmptyLineMode.SameAsLineAboveLogical && i > 0)
                    {
                        line = lines[i - 1];
                        poss = positions[i - 1];
                    }
                    else if (EmptyLineMode == IndentGuide.EmptyLineMode.SameAsLineBelowActual && i < lines.Length)
                    {
                        line = lines[i + 1];
                        poss = positions[i + 1];
                        if (!ShowAtText && poss.Count > 0) poss.RemoveAt(poss.Count - 1);
                    }
                    else if (EmptyLineMode == IndentGuide.EmptyLineMode.SameAsLineBelowLogical && i < lines.Length)
                    {
                        line = lines[i + 1];
                        poss = positions[i + 1];
                    }
                    else
                    {
                        continue;
                    }

                    foreach (var pos in poss)
                    {
                        sf.SetMeasurableCharacterRanges(new[] { new CharacterRange(0, pos) });
                        var rgns = e.Graphics.MeasureCharacterRanges(line, Font, rect, sf);
                        var x = rgns[0].GetBounds(e.Graphics).Right;
                        if (x > 0.0f)
                            e.Graphics.DrawLine(LinePen, x, y1, x, y2);
                    }
                }
            }
            catch (Exception ex)
            {
                e.Graphics.DrawString(ex.ToString(), Font, Brushes.Black, 0.0f, 0.0f);
            }
        }
    }
}
