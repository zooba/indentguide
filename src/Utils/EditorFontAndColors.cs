/* ****************************************************************************
 * Copyright 2015 Steve Dower
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy 
 * of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 * ***************************************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using Microsoft.VisualStudio.Shell;

namespace IndentGuide {
    public class EditorFontAndColors {
        public string FontFamily { get; private set; }
        public float FontSize { get; private set; }
        public bool FontBold { get; private set; }
        public Color ForeColor { get; private set; }
        public Color BackColor { get; private set; }
        public bool HighlightFontBold { get; private set; }
        public Color HighlightForeColor { get; private set; }
        public Color HighlightBackColor { get; private set; }

        public EditorFontAndColors() {
            ThreadHelper.ThrowIfNotOnUIThread();

            FontFamily = "Consolas";
            FontSize = 10.0f;
            FontBold = false;
            ForeColor = Color.Black;
            BackColor = Color.White;
            HighlightFontBold = false;
            HighlightForeColor = Color.White;
            HighlightBackColor = Color.Blue;

            try {
                var dte = (EnvDTE.DTE)IndentGuidePackage.GetGlobalService(typeof(EnvDTE.DTE));
                var props = dte.Properties["FontsAndColors", "TextEditor"];

                var fac = (EnvDTE.FontsAndColorsItems)props.Item("FontsAndColorsItems").Object;

                var colors = (EnvDTE.ColorableItems)fac.Item("Plain Text");

                FontFamily = props.Item("FontFamily").Value.ToString();
                FontSize = (float)(short)props.Item("FontSize").Value;
                FontBold = colors.Bold;
                ForeColor = ColorTranslator.FromOle((int)colors.Foreground);
                BackColor = ColorTranslator.FromOle((int)colors.Background);

                colors = (EnvDTE.ColorableItems)fac.Item("Selected Text");

                HighlightFontBold = colors.Bold;
                HighlightForeColor = ColorTranslator.FromOle((int)colors.Foreground);
                HighlightBackColor = ColorTranslator.FromOle((int)colors.Background);
            } catch (Exception ex) {
                Trace.WriteLine("Error loading text editor font and colors");
                Trace.WriteLine(ex.ToString());
            }
        }
    }
}
