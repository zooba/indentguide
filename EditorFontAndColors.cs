using System;
using System.Diagnostics;
using System.Drawing;

namespace IndentGuide
{
    public class EditorFontAndColors
    {
        public string FontFamily { get; private set; }
        public float FontSize { get; private set; }
        public bool FontBold { get; private set; }
        public Color ForeColor { get; private set; }
        public Color BackColor { get; private set; }

        public EditorFontAndColors()
        {
            FontFamily = "Consolas";
            FontSize = 10.0f;
            FontBold = false;
            ForeColor = Color.Black;
            BackColor = Color.White;

            try
            {
                var dte = (EnvDTE.DTE)IndentGuidePackage.GetGlobalService(typeof(EnvDTE.DTE));
                var props = dte.Properties["FontsAndColors", "TextEditor"];

                var fac = (EnvDTE.FontsAndColorsItems)props.Item("FontsAndColorsItems").Object;

                var colors = (EnvDTE.ColorableItems)fac.Item("Plain Text");

                FontFamily = props.Item("FontFamily").Value.ToString();
                FontSize = (float)(short)props.Item("FontSize").Value;
                FontBold = colors.Bold;
                ForeColor = ColorTranslator.FromOle((int)colors.Foreground);
                BackColor = ColorTranslator.FromOle((int)colors.Background);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error loading text editor font and colors");
                Trace.WriteLine(ex.ToString());
            }
        }
    }
}
