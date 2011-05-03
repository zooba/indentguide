using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace IndentGuide
{
    [ComVisible(true)]
    public sealed class DisplayOptions : DialogPage
    {
        private IIndentGuide Service;

        public DisplayOptions()
        {
            Visible = true;
            LineStyle = LineStyle.Dotted;
            LineColor = Color.Teal;
            EmptyLineMode = EmptyLineMode.SameAsLineAboveLogical;
            
            Service = (IIndentGuide)ServiceProvider.GlobalProvider.GetService(typeof(SIndentGuide));
            Service.VisibleChanged += new EventHandler(Service_VisibleChanged);
            Service.LineStyleChanged += new EventHandler(Service_LineStyleChanged);
            Service.LineColorChanged += new EventHandler(Service_LineColorChanged);
            Service.EmptyLineModeChanged += new EventHandler(Service_EmptyLineModeChanged);
        }

        void Service_VisibleChanged(object sender, EventArgs e)
        {
            Visible = ((IIndentGuide)sender).Visible;
        }

        void Service_LineStyleChanged(object sender, EventArgs e)
        {
            LineStyle = ((IIndentGuide)sender).LineStyle;
        }

        void Service_LineColorChanged(object sender, EventArgs e)
        {
            var color = ((IIndentGuide)sender).LineColor;
            LineColor = Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        void Service_EmptyLineModeChanged(object sender, EventArgs e)
        {
            EmptyLineMode = ((IIndentGuide)sender).EmptyLineMode;
        }

        private void UpdateService()
        {
            Service.BatchUpdate(
                Visible,
                LineStyle,
                System.Windows.Media.Color.FromArgb(LineColor.A, LineColor.R, LineColor.G, LineColor.B),
                EmptyLineMode);
        }

        public override void LoadSettingsFromStorage()
        {
            base.LoadSettingsFromStorage();
            UpdateService();
        }

        protected override void OnApply(DialogPage.PageApplyEventArgs e)
        {
            base.OnApply(e);
            UpdateService();
        }

        public override void ResetSettings()
        {
            base.ResetSettings();
            Visible = true;
            LineStyle = LineStyle.Dotted;
            LineColor = Color.Teal;
            EmptyLineMode = EmptyLineMode.SameAsLineAboveLogical;
            UpdateService();
        }

        [ResourceDescription("VisibilityDescription")]
        [ResourceCategory("Appearance")]
        public bool Visible { get; set; }

        [ResourceDisplayName("LineStyleDisplayName")]
        [ResourceDescription("LineStyleDescription")]
        [ResourceCategory("Appearance")]
        public LineStyle LineStyle { get; set; }

        [ResourceDisplayName("LineColorDisplayName")]
        [ResourceDescription("LineColorDescription")]
        [ResourceCategory("Appearance")]
        [TypeConverter(typeof(ColorConverter))]
        public Color LineColor { get; set; }

        private class EmptyLineModeTypeConverter : EnumResourceTypeConverter<EmptyLineMode>
        { }

        [ResourceDisplayName("EmptyLineModeDisplayName")]
        [ResourceDescription("EmptyLineModeDescription")]
        [ResourceCategory("Appearance")]
        [TypeConverter(typeof(EmptyLineModeTypeConverter))]
        public EmptyLineMode EmptyLineMode { get; set; }
    }
}
