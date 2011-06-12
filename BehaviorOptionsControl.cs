using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;

namespace IndentGuide
{
    public partial class BehaviorOptionsControl : UserControl, IThemeAwareDialog
    {
        public BehaviorOptionsControl()
        {
            InitializeComponent();
        }

        #region IThemeAwareDialog Members

        public IndentTheme ActiveTheme { get; set; }
        public IIndentGuide Service { get; set; }

        public void Activate()
        { }

        public void Apply()
        { }

        public void Cancel()
        { }

        public void Update(IndentTheme active, IndentTheme previous)
        {
            if (active == null)
            {
                tableLineBehavior.Enabled = false;
                tableEndOfLineBehavior.Enabled = false;
                return;
            }

            tableLineBehavior.Enabled = true;
            tableEndOfLineBehavior.Enabled = true;

            SuppressThemeChange = true;

            switch (active.EmptyLineMode)
            {
                case EmptyLineMode.NoGuides:
                    chkNoGuides.Checked = true;
                    break;
                case EmptyLineMode.SameAsLineAboveActual:
                    chkSameAsAboveActual.Checked = true;
                    break;
                case EmptyLineMode.SameAsLineAboveLogical:
                    chkSameAsAboveLogical.Checked = true;
                    break;
                case EmptyLineMode.SameAsLineBelowActual:
                    chkSameAsBelowActual.Checked = true;
                    break;
                case EmptyLineMode.SameAsLineBelowLogical:
                    chkSameAsBelowLogical.Checked = true;
                    break;
                default:
                    break;
            }
            chkEndOfLineVisible.Checked = active.VisibleAtText;
            chkEndOfLineHidden.Checked = !active.VisibleAtText;

            lineTextPreview.ShowAtText = active.VisibleAtText;
            lineTextPreview.EmptyLineMode = active.EmptyLineMode;
            lineTextPreview.Invalidate();

            SuppressThemeChange = false;
        }

        private bool SuppressThemeChange = false;

        private void OnThemeChanged(IndentTheme theme)
        {
            if (SuppressThemeChange) return;
            var evt = ThemeChanged;
            if (evt != null) evt(this, new ThemeEventArgs(theme));
        }

        public event EventHandler<ThemeEventArgs> ThemeChanged;

        #endregion

        private void BehaviorOptionsControl_Load(object sender, EventArgs e)
        {
            var conv = new EnumResourceTypeConverter<EmptyLineMode>();
            chkNoGuides.Text = conv.ConvertToString(null, CultureInfo.CurrentCulture,
                EmptyLineMode.NoGuides);
            chkSameAsAboveActual.Text = conv.ConvertToString(null, CultureInfo.CurrentCulture,
                EmptyLineMode.SameAsLineAboveActual);
            chkSameAsAboveLogical.Text = conv.ConvertToString(null, CultureInfo.CurrentCulture,
                EmptyLineMode.SameAsLineAboveLogical);
            chkSameAsBelowActual.Text = conv.ConvertToString(null, CultureInfo.CurrentCulture,
                EmptyLineMode.SameAsLineBelowActual);
            chkSameAsBelowLogical.Text = conv.ConvertToString(null, CultureInfo.CurrentCulture,
                EmptyLineMode.SameAsLineBelowLogical);
        }

        private void chkLineBehavior_CheckedChanged(object sender, EventArgs e)
        {
            var lineMode = chkNoGuides.Checked ? EmptyLineMode.NoGuides :
                chkSameAsAboveActual.Checked ? EmptyLineMode.SameAsLineAboveActual :
                chkSameAsAboveLogical.Checked ? EmptyLineMode.SameAsLineAboveLogical :
                chkSameAsBelowActual.Checked ? EmptyLineMode.SameAsLineBelowActual :
                chkSameAsBelowLogical.Checked ? EmptyLineMode.SameAsLineBelowLogical :
                EmptyLineMode.NoGuides;

            if (ActiveTheme != null) ActiveTheme.EmptyLineMode = lineMode;
            lineTextPreview.EmptyLineMode = lineMode;
            lineTextPreview.Invalidate();
            OnThemeChanged(ActiveTheme);
        }

        private void chkEndOfLine_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveTheme != null) ActiveTheme.VisibleAtText = chkEndOfLineVisible.Checked;
            lineTextPreview.ShowAtText = chkEndOfLineVisible.Checked;
            lineTextPreview.Invalidate();
            OnThemeChanged(ActiveTheme);
        }
    }

    [Guid("E41AFF16-7C74-4610-A513-61ACFCD6C60D")]
    public sealed class BehaviorOptions : GenericOptions<BehaviorOptionsControl> { }
}
