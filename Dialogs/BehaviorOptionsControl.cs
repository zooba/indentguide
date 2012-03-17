using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IndentGuide {
    public partial class BehaviorOptionsControl : UserControl, IThemeAwareDialog {
        public BehaviorOptionsControl() {
            InitializeComponent();

            gridLineMode.SelectableType = typeof(LineBehavior);
        }

        #region IThemeAwareDialog Members

        public IndentTheme ActiveTheme { get; set; }
        public IIndentGuide Service { get; set; }

        public void Activate() {
            var fac = new EditorFontAndColors();

            lineTextPreview.Font = new Font(fac.FontFamily, fac.FontSize);
            lineTextPreview.ForeColor = fac.ForeColor;
            lineTextPreview.BackColor = fac.BackColor;
        }

        public void Apply() { }

        public void Update(IndentTheme active, IndentTheme previous) {
            if (active != null) {
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

        private void gridLineMode_PropertyValueChanged(object s, EventArgs e) {
            lineTextPreview.Invalidate();

            OnThemeChanged(ActiveTheme);
            Update(ActiveTheme, ActiveTheme);
        }
    }

    [Guid("D6E472BA-194A-46BD-B817-9107BC0DF1A1")]
    public sealed class BehaviorOptions : GenericOptions<BehaviorOptionsControl> { }
}
