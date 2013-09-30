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
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IndentGuide {
    public partial class PageWidthOptionsControl : UserControl, IThemeAwareDialog {
        public PageWidthOptionsControl() {
            InitializeComponent();
            if (Environment.OSVersion.Version >= new Version(6, 2)) {
                btnAddLocation.Text = "\uE109";
                btnRemoveLocation.Text = "\uE10A";
            }
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

        public void Close() { }

        public void Update(IndentTheme active, IndentTheme previous) {
            lstLocations.BeginUpdate();
            try {
                lstLocations.Items.Clear();
                if (active != null) {
                    foreach (var item in active.PageWidthMarkers.OrderBy(i => i.Position)) {
                        lstLocations.Items.Add(item);
                    }
                }
            } finally {
                lstLocations.EndUpdate();
                gridLineStyle.SelectedObject = null;
                lstLocations.SelectedIndex = (lstLocations.Items.Count > 0) ? 0 : -1;
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
            var format = gridLineStyle.SelectedObject as PageWidthMarkerFormat;
            if (format != null) {
                if (e.ChangedItem.PropertyDescriptor.Name == "Position") {
                    if (ActiveTheme != null) {
                        if (ActiveTheme.LineFormats.ContainsKey(format.GetFormatIndex())) {
                            format.Position = (int)e.OldValue;
                            MessageBox.Show(ResourceLoader.LoadString("PageWidthExists"), ResourceLoader.LoadString("Title"));
                            return;
                        }
                        ActiveTheme.LineFormats.Remove(format.FormatIndex.Value);
                        format.FormatIndex = format.GetFormatIndex();
                        ActiveTheme.LineFormats[format.FormatIndex.Value] = format;
                    }
                    lstLocations.FormattingEnabled = false;
                    lstLocations.FormattingEnabled = true;
                }

                linePreview.ForeColor = format.LineColor;
                linePreview.GlowColor = format.LineColor;
                linePreview.Style = format.LineStyle;
                linePreviewHighlight.ForeColor = format.HighlightStyle.HasFlag(LineStyle.Glow) ? format.LineColor : format.HighlightColor;
                linePreviewHighlight.GlowColor = format.HighlightColor;
                linePreviewHighlight.Style = format.HighlightStyle;
            }

            OnThemeChanged(ActiveTheme);
        }

        private void lstLocations_SelectedIndexChanged(object sender, EventArgs e) {
            var format = lstLocations.SelectedItem as PageWidthMarkerFormat;
            if (format == null) {
                gridLineStyle.SelectedObject = null;
                return;
            }

            gridLineStyle.SelectedObject = format;
            linePreview.ForeColor = format.LineColor;
            linePreview.GlowColor = format.LineColor;
            linePreview.Style = format.LineStyle;
            linePreviewHighlight.ForeColor = format.HighlightStyle.HasFlag(LineStyle.Glow) ? format.LineColor : format.HighlightColor;
            linePreviewHighlight.GlowColor = format.HighlightColor;
            linePreviewHighlight.Style = format.HighlightStyle;
        }

        private void lstLocations_Format(object sender, ListControlConvertEventArgs e) {
            var format = e.ListItem as PageWidthMarkerFormat;
            Debug.Assert(format != null);
            if (format == null) return;

            e.Value = format.Position.ToString(CultureInfo.CurrentUICulture);
        }

        private void btnAddLocation_Click(object sender, EventArgs e) {
            if (ActiveTheme == null) return;

            var existing = lstLocations.SelectedItem as PageWidthMarkerFormat;
            PageWidthMarkerFormat format;
            if (existing != null) {
                format = (PageWidthMarkerFormat)existing.Clone(ActiveTheme);
            } else {
                format = new PageWidthMarkerFormat(ActiveTheme);
            }
            
            if (existing != null) {
                format.Position = existing.Position + 10;
                while (ActiveTheme.LineFormats.ContainsKey(format.GetFormatIndex())) {
                    format.Position += 10;
                }
            }
            format.FormatIndex = format.GetFormatIndex();
            ActiveTheme.LineFormats[format.FormatIndex.Value] = format;
            OnThemeChanged(ActiveTheme);
            lstLocations.Items.Add(format);
            lstLocations.SelectedItem = format;
        }

        private void btnRemoveLocation_Click(object sender, EventArgs e) {
            if (ActiveTheme == null) return;

            int index = lstLocations.SelectedIndex;
            var existing = lstLocations.SelectedItem as PageWidthMarkerFormat;
            if (existing != null) {
                ActiveTheme.LineFormats.Remove(existing.FormatIndex.Value);
                OnThemeChanged(ActiveTheme);
                lstLocations.Items.Remove(existing);
            }
            if (lstLocations.Items.Count > index) {
                lstLocations.SelectedIndex = index;
            } else if (lstLocations.Items.Count > 0) {
                lstLocations.SelectedIndex = lstLocations.Items.Count - 1;
            } else {
                lstLocations.SelectedIndex = -1;
            }
        }
    }

    [Guid("B10D21A1-E1B6-4083-A939-6B6DFAF380F4")]
    public sealed class PageWidthOptions : GenericOptions<PageWidthOptionsControl> { }
}
