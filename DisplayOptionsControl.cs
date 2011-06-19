using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Globalization;

namespace IndentGuide
{
    public partial class DisplayOptionsControl : UserControl, IThemeAwareDialog
    {
        public DisplayOptionsControl()
        {
            InitializeComponent();
        }

        #region IThemeAwareDialog Members

        private IndentTheme _ActiveTheme = null;
        public IndentTheme ActiveTheme
        {
            get { return _ActiveTheme; }
            set
            {
                if (_ActiveTheme != value && _ActiveTheme != null)
                    Apply();
                _ActiveTheme = value;
            }
        }
        public IIndentGuide Service { get; set; }

        public void Activate()
        { }

        public void Apply()
        {
            if (ActiveTheme == null) return;

            ActiveTheme.NumberedOverride.Clear();
            foreach (var oi in lstOverrides.Items.OfType<OverrideInfo>())
            {
                if (oi.Index > 0)
                {
                    ActiveTheme.NumberedOverride[oi.Index] = oi.Format;
                }
            }
        }

        public void Update(IndentTheme active, IndentTheme previous)
        {
            if (active == null)
            {
                gridLineStyle.SelectedObject = null;
                linePreview.ForeColor = Color.Teal;
                linePreview.Style = LineStyle.Solid;
            }
            else
            {
                gridLineStyle.SelectedObject = active.DefaultLineFormat;
                linePreview.ForeColor = active.DefaultLineFormat.LineColor;
                linePreview.Style = active.DefaultLineFormat.LineStyle;

                if (previous != active)
                {
                    lstOverrides.BeginUpdate();
                    lstOverrides.Items.Clear();
                    foreach (var kv in active.NumberedOverride)
                    {
                        lstOverrides.Items.Add(new OverrideInfo
                        {
                            Index = kv.Key,
                            Text = string.Format(CultureInfo.CurrentCulture, "#{0}", kv.Key),
                            Format = kv.Value
                        });
                    }
                    lstOverrides.EndUpdate();
                    if (lstOverrides.Items.Count > 0)
                        lstOverrides.SelectedIndex = 0;
                }

                var oi = lstOverrides.SelectedItem as OverrideInfo;
                if (oi == null)
                {
                    gridLineOverride.SelectedObject = null;
                    lineOverridePreview.ForeColor = linePreview.ForeColor;
                    lineOverridePreview.Style = linePreview.Style;

                    chkLineOverrideIndex.Enabled = false;
                    chkLineOverrideText.Enabled = false;
                    txtLineFormatIndex.Enabled = false;
                    txtLineFormatText.Enabled = false;

                    btnRemoveOverride.Enabled = false;
                }
                else
                {
                    gridLineOverride.SelectedObject = oi.Format;
                    lineOverridePreview.ForeColor = oi.Format.LineColor;
                    lineOverridePreview.Style = oi.Format.LineStyle;

                    chkLineOverrideIndex.Enabled = true;
                    chkLineOverrideText.Enabled = false;// true; // NOT IMPLEMENTED
                    txtLineFormatIndex.Enabled = true;
                    txtLineFormatText.Enabled = false;// true; // NOT IMPLEMENTED

                    btnRemoveOverride.Enabled = true;

                    if (oi.Index > 0)
                    {
                        chkLineOverrideIndex.Checked = true;
                        txtLineFormatIndex.Value = oi.Index;
                    }
                    else
                    {
                        chkLineOverrideText.Checked = true;
                        txtLineFormatText.Text = oi.Pattern;
                    }
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

        private void gridLineStyle_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            OnThemeChanged(ActiveTheme);
            Update(ActiveTheme, ActiveTheme);
        }

        class OverrideInfo : IComparable
        {
            public string Text;
            public int Index;
            public string Pattern;
            public LineFormat Format;

            public int CompareTo(object obj)
            {
                var oi = obj as OverrideInfo;
                if (oi == null) return -1;
                return Text.CompareTo(oi.Text);
            }
        }

        private void txtLineFormatIndex_ValueChanged(object sender, EventArgs e)
        {
            var oi = lstOverrides.SelectedItem as OverrideInfo;
            if (oi == null) return;

            oi.Index = (int)txtLineFormatIndex.Value;
        }

        private void lstOverrides_SelectedIndexChanged(object sender, EventArgs e)
        {
            Update(ActiveTheme, ActiveTheme);
        }

        private void btnAddOverride_Click(object sender, EventArgs e)
        {
            var newOi = new OverrideInfo();

            newOi.Format = ActiveTheme.DefaultLineFormat.Clone();
            newOi.Index = 1;
            foreach (var oi in lstOverrides.Items.OfType<OverrideInfo>())
            {
                if (oi.Index >= newOi.Index) newOi.Index = oi.Index + 1;
            }
            newOi.Text = string.Format(CultureInfo.CurrentCulture, "#{0}", newOi.Index);
            lstOverrides.Items.Add(newOi);
            lstOverrides.SelectedItem = newOi;

            OnThemeChanged(ActiveTheme);
            Update(ActiveTheme, ActiveTheme);
        }

        private void btnRemoveOverride_Click(object sender, EventArgs e)
        {
            if (lstOverrides.SelectedIndex >= 0)
                lstOverrides.Items.RemoveAt(lstOverrides.SelectedIndex);

            OnThemeChanged(ActiveTheme);
            Update(ActiveTheme, ActiveTheme);
        }

        private void lstOverrides_Format(object sender, ListControlConvertEventArgs e)
        {
            var oi = e.ListItem as OverrideInfo;
            if (oi == null) e.Value = "(null)";
            e.Value = oi.Text;
        }

    }

    [Guid("05491866-4ED1-44FE-BDFF-FB14246BDABB")]
    public sealed class DisplayOptions : GenericOptions<DisplayOptionsControl> { }
}
