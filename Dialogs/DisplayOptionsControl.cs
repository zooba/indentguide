/* ****************************************************************************
 * Copyright 2012 Steve Dower
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
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IndentGuide {
    public partial class DisplayOptionsControl : UserControl, IThemeAwareDialog {
        class OverrideInfo {
            public string Text;
            public int Index;
            public string Pattern;
        }

        public DisplayOptionsControl() {
            InitializeComponent();

            lstOverrides.BeginUpdate();
            lstOverrides.Items.Clear();
            lstOverrides.Items.Add(new OverrideInfo {
                Index = IndentTheme.DefaultFormatIndex,
                Text = ResourceLoader.LoadString("DefaultFormatName")
            });
            lstOverrides.Items.Add(new OverrideInfo {
                Index = IndentTheme.UnalignedFormatIndex,
                Text = ResourceLoader.LoadString("UnalignedFormatName")
            });
            for (int key = 1; key <= 30; ++key) {
                var name = string.Format(CultureInfo.CurrentCulture, "#{0}", key);
                lstOverrides.Items.Add(new OverrideInfo {
                    Index = key,
                    Text = name
                });
            }
            lstOverrides.EndUpdate();
        }

        #region IThemeAwareDialog Members

        public IndentTheme ActiveTheme { get; set; }
        public IIndentGuide Service { get; set; }

        public void Activate() {
            var fac = new EditorFontAndColors();

            linePreview.BackColor = fac.BackColor;
            linePreviewHighlight.BackColor = fac.BackColor;
        }

        public void Apply() { }

        public void Update(IndentTheme active, IndentTheme previous) {
            if (active != null) {
                int previousIndex = lstOverrides.SelectedIndex;
                lstOverrides.SelectedItem = null;    // ensure a change event occurs
                if (0 <= previousIndex && previousIndex < lstOverrides.Items.Count) {
                    lstOverrides.SelectedIndex = previousIndex;
                } else {
                    lstOverrides.SelectedIndex = 0;
                }
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

        private void gridLineStyle_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
            var format = gridLineStyle.SelectedObject as LineFormat;
            if (format != null) {
                linePreview.ForeColor = format.LineColor;
                linePreview.GlowColor = format.LineColor;
                linePreview.Style = format.LineStyle;
                linePreviewHighlight.ForeColor = format.HighlightStyle.HasFlag(LineStyle.Glow) ? format.LineColor : format.HighlightColor;
                linePreviewHighlight.GlowColor = format.HighlightColor;
                linePreviewHighlight.Style = format.HighlightStyle;
            }

            OnThemeChanged(ActiveTheme);
        }

        private void lstOverrides_SelectedIndexChanged(object sender, EventArgs e) {
            if (lstOverrides.SelectedItem == null) return;
            var oi = lstOverrides.SelectedItem as OverrideInfo;
            Debug.Assert(oi != null);
            if (oi == null) return;

            LineFormat format;
            if (oi.Pattern == null) {
                if (!ActiveTheme.LineFormats.TryGetValue(oi.Index, out format)) {
                    ActiveTheme.LineFormats[oi.Index] = format = ActiveTheme.DefaultLineFormat.Clone();
                }
            } else {
                // TODO: Pattern based formatting
                format = ActiveTheme.DefaultLineFormat.Clone();
            }

            gridLineStyle.SelectedObject = format;
            linePreview.ForeColor = format.LineColor;
            linePreview.GlowColor = format.LineColor;
            linePreview.Style = format.LineStyle;
            linePreviewHighlight.ForeColor = format.HighlightStyle.HasFlag(LineStyle.Glow) ? format.LineColor : format.HighlightColor;
            linePreviewHighlight.GlowColor = format.HighlightColor;
            linePreviewHighlight.Style = format.HighlightStyle;
        }

        private void lstOverrides_Format(object sender, ListControlConvertEventArgs e) {
            var oi = e.ListItem as OverrideInfo;
            Debug.Assert(oi != null);
            if (oi == null) return;

            e.Value = oi.Text;
        }
    }

    [Guid("05491866-4ED1-44FE-BDFF-FB14246BDABB")]
    public sealed class DisplayOptions : GenericOptions<DisplayOptionsControl> { }
}
