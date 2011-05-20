using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IndentGuide
{
    public partial class BehaviorOptionsControl : UserControl, IThemeAwareDialog
    {
        public BehaviorOptionsControl()
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

        public void Update(IndentTheme active, IndentTheme previous)
        {
            
        }

        private void OnThemeChanged(IndentTheme theme)
        {
            var evt = ThemeChanged;
            if (evt != null) evt(this, new ThemeEventArgs(theme));
        }
        
        public event EventHandler<ThemeEventArgs> ThemeChanged;

        #endregion
    }

    [Guid("E41AFF16-7C74-4610-A513-61ACFCD6C60D")]
    public sealed class BehaviorOptions : GenericOptions<BehaviorOptionsControl> { }
}
