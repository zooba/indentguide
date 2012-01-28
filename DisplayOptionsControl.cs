using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IndentGuide {
    public partial class DisplayOptionsControl : UserControl, IThemeAwareDialog {
        class OverrideInfo {
            public string Text;
            public int Index;
            public string Pattern;
        }

        public DisplayOptionsControl() {
            InitializeComponent();

            gridLineMode.SelectableType = typeof(LineBehavior);

            lstOverrides.BeginUpdate();
            lstOverrides.Items.Clear();
            lstOverrides.Items.Add(new OverrideInfo {
                Index = IndentTheme.DefaultFormatIndex,
                Text = ResourceLoader.LoadString("DefaultFormatName")
            });
            lstOverrides.Items.Add(new OverrideInfo {
                Index = IndentTheme.UnalignedFormatIndex,
                Text = ResourceLoader.LoadString("UnalignedFormatName")
            });
            lstOverrides.Items.Add(new OverrideInfo {
                Index = IndentTheme.CaretFormatIndex,
                Text = ResourceLoader.LoadString("CaretFormatName")
            });
            for (int key = 1; key <= 30; ++key) {
                var name = string.Format(CultureInfo.CurrentCulture, "#{0}", key);
                lstOverrides.Items.Add(new OverrideInfo {
                    Index = key,
                    Text = name
                });
            }
            lstOverrides.EndUpdate();
        }

        #region IThemeAwareDialog Members

        public IndentTheme ActiveTheme { get; set; }
        public IIndentGuide Service { get; set; }

        public void Activate() {
            var fac = new EditorFontAndColors();

            lineTextPreview.Font = new Font(fac.FontFamily, fac.FontSize);
            lineTextPreview.ForeColor = fac.ForeColor;
            lineTextPreview.BackColor = fac.BackColor;
            linePreview.BackColor = fac.BackColor;
        }

        public void Apply() { }

        public void Update(IndentTheme active, IndentTheme previous) {
            if (active != null) {
                if (previous != active) {
                    lstOverrides.SelectedItem = null;    // ensure a change event occurs
                    lstOverrides.SelectedIndex = 0;
                }

                gridLineMode.SelectedObject = active.Behavior;
                lineTextPreview.Theme = active;
            }
        }

        private void OnThemeChanged(IndentTheme theme) {
            var evt = ThemeChanged;
            if (evt != null) evt(this, new ThemeEventArgs(theme));
        }

        public event EventHandler<ThemeEventArgs> ThemeChanged;

        #endregion

        private void gridLineStyle_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
            var format = gridLineStyle.SelectedObject as LineFormat;
            if (format != null) {
                linePreview.ForeColor = format.LineColor;
                linePreview.Style = format.LineStyle;
            }

            lineTextPreview.Invalidate();

            OnThemeChanged(ActiveTheme);
            Update(ActiveTheme, ActiveTheme);
        }

        private void gridLineMode_PropertyValueChanged(object s, EventArgs e) {
            lineTextPreview.Invalidate();

            OnThemeChanged(ActiveTheme);
            Update(ActiveTheme, ActiveTheme);
        }

        private void lstOverrides_SelectedIndexChanged(object sender, EventArgs e) {
            if (lstOverrides.SelectedItem == null) return;
            var oi = lstOverrides.SelectedItem as OverrideInfo;
            Debug.Assert(oi != null);
            if (oi == null) return;

            ActiveTheme.Apply();
            LineFormat format;
            if (oi.Pattern == null) {
                if (!ActiveTheme.LineFormats.TryGetValue(oi.Index, out format))
                    ActiveTheme.LineFormats[oi.Index] = format = ActiveTheme.DefaultLineFormat.Clone();
            } else {
                // TODO: Pattern based formatting
                format = ActiveTheme.DefaultLineFormat.Clone();
            }

            gridLineStyle.SelectedObject = format;
            linePreview.ForeColor = format.LineColor;
            linePreview.Style = format.LineStyle;
        }

        private void lstOverrides_Format(object sender, ListControlConvertEventArgs e) {
            var oi = e.ListItem as OverrideInfo;
            Debug.Assert(oi != null);
            if (oi == null) return;

            e.Value = oi.Text;
        }

    }

    [Guid("05491866-4ED1-44FE-BDFF-FB14246BDABB")]
    public sealed class DisplayOptions : GenericOptions<DisplayOptionsControl> { }
}
