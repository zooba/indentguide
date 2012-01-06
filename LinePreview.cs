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
                if (Style.HasFlag(LineStyle.Thick))
                    LinePen = new Pen(ForeColor, 3.0f);
                else
                    LinePen = new Pen(ForeColor, 1.0f);

                if (Style.HasFlag(LineStyle.Solid))
                {
                    LinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    LinePen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
                    LinePen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
                }
                else if (Style.HasFlag(LineStyle.Dotted))
                {
                    LinePen.DashPattern = new float[] { 1.0f, 2.0f, 1.0f, 2.0f };
                }
                else if (Style.HasFlag(LineStyle.Dashed))
                {
                    LinePen.DashPattern = new float[] { 3.0f, 3.0f, 3.0f, 3.0f };
                }
            }

            e.Graphics.DrawLine(LinePen, x, y1, x, y2);
        }
    }
}
