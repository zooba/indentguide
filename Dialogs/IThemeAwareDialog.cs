using System;
using System.Windows.Forms;

namespace IndentGuide {
    public class ThemeEventArgs : EventArgs {
        public ThemeEventArgs(IndentTheme theme)
            : base() {
            Theme = theme;
        }

        public IndentTheme Theme { get; set; }
    }

    public interface IThemeAwareDialog {
        IndentTheme ActiveTheme { set; }
        IIndentGuide Service { set; }

        Control.ControlCollection Controls { get; }

        void Activate();
        void Apply();

        void Update(IndentTheme active, IndentTheme previous);

        event EventHandler<ThemeEventArgs> ThemeChanged;
    }
}
