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

        [ResourceDescription("VisibilityDescription")]
        [ResourceCategory("Appearance")]
        public bool Visible { get; set; }

        [ResourceDisplayName("LineStyleDisplayName")]
        [ResourceDescription("LineStyleDescription")]
        [ResourceCategory("Appearance")]
        public LineStyle Line { get; set; }

        private class EmptyLineModeTypeConverter : EnumResourceTypeConverter<EmptyLineMode>
        { }

        [ResourceDisplayName("EmptyLineModeDisplayName")]
        [ResourceDescription("EmptyLineModeDescription")]
        [ResourceCategory("Appearance")]
        [TypeConverter(typeof(EmptyLineModeTypeConverter))]
        public EmptyLineMode EmptyLineMode { get; set; }
    }
}
