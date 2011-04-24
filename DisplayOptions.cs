using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Composition;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace IndentGuide
{
    [ComVisible(true)]
    public sealed class DisplayOptions : DialogPage
    {
        private IIndentGuide Service;

        public DisplayOptions()
        {
            Visible = true;
            Line = LineStyle.Dotted;
            
            Service = (IIndentGuide)ServiceProvider.GlobalProvider.GetService(typeof(SIndentGuide));
            Service.VisibleChanged += new EventHandler(Service_VisibleChanged);
            Service.LineStyleChanged += new EventHandler(Service_LineStyleChanged);
        }

        void Service_LineStyleChanged(object sender, EventArgs e)
        {
            Line = ((IIndentGuide)sender).LineStyle;
        }

        void Service_VisibleChanged(object sender, EventArgs e)
        {
            Visible = ((IIndentGuide)sender).Visible;
        }

        protected override void OnApply(DialogPage.PageApplyEventArgs e)
        {
            base.OnApply(e);
            Service.Visible = Visible;
            Service.LineStyle = Line;
        }

        public override void ResetSettings()
        {
            base.ResetSettings();
            Visible = true;
            Line = LineStyle.Dotted;
        }

        [Description("True to display indentation guides in all text editors; otherwise, False.")]
        [Category("Appearance")]
        public bool Visible { get; set; }

        [Description("The style of guides to display.")]
        [Category("Appearance")]
        public LineStyle Line { get; set; }
    }
}
