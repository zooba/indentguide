using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

namespace IndentGuide {
    public partial class CustomBehaviorOptionsControl : UserControl, IThemeAwareDialog {
        public CustomBehaviorOptionsControl() {
            InitializeComponent();

            gridLineMode.SelectableType = typeof(LineBehavior);
        }

        #region IThemeAwareDialog Members

        public IndentTheme ActiveTheme { get; set; }
        public IIndentGuide Service { get; set; }

        public void Activate() {
            var fac = new EditorFontAndColors();

            lineTextPreview.Font = new Font(fac.FontFamily, fac.FontSize, fac.FontBold ? FontStyle.Bold : FontStyle.Regular);
            lineTextPreview.ForeColor = fac.ForeColor;
            lineTextPreview.BackColor = fac.BackColor;
        }

        public void Apply() { }

        public void Update(IndentTheme active, IndentTheme previous) {
            if (active != null) {
                gridLineMode.SelectedObject = active.Behavior;
                lineTextPreview.Theme = active;
                lineTextPreview.Invalidate();
            }
        }

        private void OnThemeChanged(IndentTheme theme) {
            if (theme != null) {
                var evt = ThemeChanged;
                if (evt != null) evt(this, new ThemeEventArgs(theme));
            }
        }

        public event EventHandler<ThemeEventArgs> ThemeChanged;

        #endregion

        private void gridLineMode_PropertyValueChanged(object s, EventArgs e) {
            lineTextPreview.Invalidate();

            OnThemeChanged(ActiveTheme);
            Update(ActiveTheme, ActiveTheme);
        }
    }

    [Guid("16020738-BDB9-4165-A7C1-65B51D1EE134")]
    public sealed class CustomBehaviorOptions : GenericOptions<CustomBehaviorOptionsControl> { }
}
