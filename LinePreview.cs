using System;
using System.Drawing;
using System.Windows.Forms;

namespace IndentGuide
{
    public partial class LinePreview : UserControl
    {
        public LinePreview()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw | ControlStyles.Opaque, true);

            InitializeComponent();
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

        private Pen LinePen = null;
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);

            int x = ClientRectangle.Width / 2 + ClientRectangle.Left;
            int y1 = ClientRectangle.Top;
            int y2 = ClientRectangle.Bottom;

            if (LinePen == null)
            {
                LinePen = new Pen(ForeColor, (float)Style.GetStrokeThickness());

                var pattern = Style.GetDashPattern();
                if (pattern == null)
                {
                    LinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    LinePen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
                    LinePen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
                }
                else
                {
                    LinePen.DashPattern = pattern;
                }
            }

            e.Graphics.DrawLine(LinePen, x, y1, x, y2);
        }
    }
}
