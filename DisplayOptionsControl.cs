using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IndentGuide
{
    public partial class DisplayOptionsControl : UserControl, IThemeAwareDialog
    {
        public DisplayOptionsControl()
        {
            InitializeComponent();
        }

        #region IThemeAwareDialog Members

        public IndentTheme ActiveTheme {get;set;}
        public IIndentGuide Service { get; set; }

        public void Activate()
        { }

        public void Apply()
        { }

        public void Cancel()
        { }

        public void Update(IndentTheme active)
        {
            if (active == null)
            {
                btnResetLineFormat.Enabled = false;
                gridLineStyle.SelectedObject = null;
                linePreview.ForeColor = Color.Teal;
                linePreview.Style = LineStyle.Solid;
                chkLineLogical.Checked = true;
                chkLineAbove.Checked = true;
            }
            else
            {
                LineFormat activeFormat = active.DefaultLineFormat;
                int i = txtLineFormatIndex.SelectedIndex;
                if (i > 0)
                {
                    if (!active.NumberedOverride.TryGetValue(i, out activeFormat))
                    {
                        activeFormat = active.DefaultLineFormat.Clone();
                        active.NumberedOverride[i] = activeFormat;
                    }
                    btnResetLineFormat.Enabled = !active.DefaultLineFormat.Equals(activeFormat);
                }
                else
                {
                    btnResetLineFormat.Enabled = !active.DefaultLineFormat.Equals(new LineFormat());
                }

                gridLineStyle.SelectedObject = activeFormat;
                linePreview.ForeColor = activeFormat.LineColor;
                linePreview.Style = activeFormat.LineStyle;
                switch (active.EmptyLineMode)
                {
                case EmptyLineMode.NoGuides:
                    break;
                case EmptyLineMode.SameAsLineAboveActual:
                    chkLineActual.Checked = true;
                    chkLineAbove.Checked = true;
                    break;
                case EmptyLineMode.SameAsLineAboveLogical:
                    chkLineLogical.Checked = true;
                    chkLineAbove.Checked = true;
                    break;
                case EmptyLineMode.SameAsLineBelowActual:
                    chkLineActual.Checked = true;
                    chkLineBelow.Checked = true;
                    break;
                case EmptyLineMode.SameAsLineBelowLogical:
                    chkLineLogical.Checked = true;
                    chkLineBelow.Checked = true;
                    break;
                }
            }
        }

        private void OnThemeChanged(IndentTheme theme)
        {
            var evt = ThemeChanged;
            if (evt != null) evt(this, new ThemeEventArgs(theme));
        }

        public event EventHandler<ThemeEventArgs> ThemeChanged;

        #endregion
        
        private void DisplayOptionsControl_Load(object sender, EventArgs e)
        {
            var items = ResourceLoader.LoadString(txtLineFormatIndex.Name + "_Items").Split(';');
            txtLineFormatIndex.Items.Clear();
            txtLineFormatIndex.Items.AddRange(items);
            txtLineFormatIndex.SelectedIndex = 0;
        }

        private void gridLineStyle_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            OnThemeChanged(ActiveTheme);

            Update(ActiveTheme);
        }

        private void chkEmptyLineBehaviour_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveTheme == null) return;

            OnThemeChanged(ActiveTheme);
            
            if (chkLineActual.Checked)
            {
                ActiveTheme.EmptyLineMode = chkLineAbove.Checked
                    ? EmptyLineMode.SameAsLineAboveActual
                    : EmptyLineMode.SameAsLineBelowActual;
            }
            else
            {
                ActiveTheme.EmptyLineMode = chkLineAbove.Checked
                    ? EmptyLineMode.SameAsLineAboveLogical
                    : EmptyLineMode.SameAsLineBelowLogical;
            }
        }

        private int txtLineFormatIndex_PreviousIndex = -1;
        private void txtLineFormatIndex_SelectedItemChanged(object sender, EventArgs e)
        {
            LineFormat prevFormat;
            if (ActiveTheme != null &&
                txtLineFormatIndex_PreviousIndex > 0 &&
                ActiveTheme.NumberedOverride.TryGetValue(txtLineFormatIndex_PreviousIndex, out prevFormat) &&
                prevFormat.Equals(ActiveTheme.DefaultLineFormat))
            {
                ActiveTheme.NumberedOverride.Remove(txtLineFormatIndex_PreviousIndex);
            }

            txtLineFormatIndex_PreviousIndex = txtLineFormatIndex.SelectedIndex;

            Update(ActiveTheme);
        }

        private void btnResetLineFormat_Click(object sender, EventArgs e)
        {
            if (ActiveTheme == null) return;

            int i = txtLineFormatIndex.SelectedIndex;
            if (i > 0)
            {
                ActiveTheme.NumberedOverride.Remove(i);
            }
            else
            {
                ActiveTheme.Apply();
                ActiveTheme.DefaultLineFormat = new LineFormat();
            }

            OnThemeChanged(ActiveTheme);
            
            Update(ActiveTheme);
        }
    }

    [Guid("05491866-4ED1-44FE-BDFF-FB14246BDABB")]
    public sealed class DisplayOptions : GenericOptions<DisplayOptionsControl> { }
}
