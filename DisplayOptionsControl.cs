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
            var theme = ActiveTheme;
            if (theme == null) return;

            var overrides = lstOverrides.Items.OfType<OverrideInfo>().ToDictionary(oi => oi.Index);
            
            OverrideInfo defOI;
            if (!overrides.TryGetValue(int.MinValue, out defOI))
                defOI = new OverrideInfo { Format = new LineFormat() };
            theme.DefaultLineFormat = defOI.Format;


            theme.NumberedOverride.Clear();
            foreach (var oi in overrides.Values)
            {
                if (oi.Index > 0 && oi.Format != null && oi.Format != theme.DefaultLineFormat)
                    theme.NumberedOverride[oi.Index] = oi.Format;
            }
        }

        public void Update(IndentTheme active, IndentTheme previous)
        {
            if (active == null)
            {
                lstOverrides.Items.Clear();
            }
            else
            {
                linePreview.ForeColor = active.DefaultLineFormat.LineColor;
                linePreview.Style = active.DefaultLineFormat.LineStyle;

                if (previous != active)
                {
                    lstOverrides.BeginUpdate();
                    lstOverrides.Items.Clear();
                    lstOverrides.Items.Add(new OverrideInfo {
                        Index = int.MinValue,
                        Text = ResourceLoader.LoadString("DefaultOverrideName"),
                        Format = active.DefaultLineFormat
                    });
                    for (int key = 1; key <= 30; ++key)
                    {
                        LineFormat format;
                        var name = string.Format(CultureInfo.CurrentCulture, "#{0}", key);
                        lstOverrides.Items.Add(new OverrideInfo {
                            Index = key,
                            Text = name,
                            Format = active.NumberedOverride.TryGetValue(key, out format) ? format : null
                        });
                    }
                    lstOverrides.EndUpdate();
                    if (lstOverrides.Items.Count > 0)
                        lstOverrides.SelectedIndex = 0;
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

        class OverrideInfo
        {
            public string Text;
            public int Index;
            public string Pattern;
            public LineFormat Format;
        }

        private void lstOverrides_SelectedIndexChanged(object sender, EventArgs e)
        {
            var oi = lstOverrides.SelectedItem as OverrideInfo;
            if (oi == null)
            {
                gridLineStyle.SelectedObject = null;
                linePreview.ForeColor = Color.Transparent;
                linePreview.Style = LineStyle.Solid;
            }
            else
            {
                if (oi.Format == null)
                    oi.Format = ActiveTheme.DefaultLineFormat.Clone();

                gridLineStyle.SelectedObject = oi.Format;
                linePreview.ForeColor = oi.Format.LineColor;
                linePreview.Style = oi.Format.LineStyle;
            }
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
