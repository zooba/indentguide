using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IndentGuide
{
    public class ThemeEventArgs : EventArgs
    {
        public ThemeEventArgs(IndentTheme theme)
            : base()
        {
            Theme = theme;
        }

        public IndentTheme Theme { get; set; }
    }

    public interface IThemeAwareDialog
    {
        IndentTheme ActiveTheme { set; }
        IIndentGuide Service { set; }
        
        Control.ControlCollection Controls { get; }

        void Activate();
        void Apply();
        void Cancel();

        void Update(IndentTheme active);

        event EventHandler<ThemeEventArgs> ThemeChanged;
    }
}
