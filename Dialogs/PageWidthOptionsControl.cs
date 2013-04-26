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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IndentGuide {
    public partial class PageWidthOptionsControl : UserControl, IThemeAwareDialog {
        class LineFormatAndPosition {
            public LineFormatAndPosition(int position, LineFormat format) {
                Position = PreviousPosition = position;
                Format = format;
            }

            [Browsable(false)]
            public readonly LineFormat Format;

            [Browsable(false)]
            public int PreviousPosition { get; set; }

            [ResourceDescription("PageWidthPosition")]
            [ResourceCategory("Appearance")]
            public int Position { get; set; }

            [ResourceDescription("VisibilityDescription")]
            [ResourceCategory("Appearance")]
            public bool Visible { get { return Format.Visible; } set { Format.Visible = value; } }

            bool ShouldSerializeVisible() {
                return Format.Visible != Format.BaseFormat.Visible;
            }

            class LineStyleConverter : EnumResourceTypeConverter<LineStyle> { }

            [ResourceDisplayName("LineStyleDisplayName")]
            [ResourceDescription("LineStyleDescription")]
            [ResourceCategory("Appearance")]
            [TypeConverter(typeof(LineStyleConverter))]
            public LineStyle LineStyle { get { return Format.LineStyle; } set { Format.LineStyle = value; } }

            bool ShouldSerializeLineStyle() {
                return Format.LineStyle != Format.BaseFormat.LineStyle;
            }

            [ResourceDisplayName("LineColorDisplayName")]
            [ResourceDescription("LineColorDescription")]
            [ResourceCategory("Appearance")]
            [TypeConverter(typeof(ColorConverter))]
            public Color LineColor { get { return Format.LineColor; } set { Format.LineColor = value; } }

            bool ShouldSerializeLineColor() {
                return Format.LineColor != Format.BaseFormat.LineColor;
            }

            [ResourceDisplayName("HighlightStyleDisplayName")]
            [ResourceDescription("HighlightStyleDescription")]
            [ResourceCategory("Appearance")]
            [TypeConverter(typeof(LineStyleConverter))]
            public LineStyle HighlightStyle { get { return Format.HighlightStyle; } set { Format.HighlightStyle = value; } }

            bool ShouldSerializeHighlightStyle() {
                return Format.HighlightStyle != Format.BaseFormat.HighlightStyle;
            }

            [ResourceDisplayName("HighlightColorDisplayName")]
            [ResourceDescription("HighlightColorDescription")]
            [ResourceCategory("Appearance")]
            [TypeConverter(typeof(ColorConverter))]
            public Color HighlightColor { get { return Format.HighlightColor; } set { Format.HighlightColor = value; } }

            bool ShouldSerializeHighlightColor() {
                return Format.HighlightColor != Format.BaseFormat.HighlightColor;
            }
        }

        public PageWidthOptionsControl() {
            InitializeComponent();
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
                    foreach (var keyValue in active.PageWidthMarkers.OrderBy(kv => kv.Key)) {
                        lstLocations.Items.Add(new LineFormatAndPosition(keyValue.Key, keyValue.Value));
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
            var format = gridLineStyle.SelectedObject as LineFormatAndPosition;
            if (format != null) {
                if (format.Position != format.PreviousPosition) {
                    if (ActiveTheme != null) {
                        if (ActiveTheme.PageWidthMarkers.ContainsKey(format.Position)) {
                            throw new ArgumentException(ResourceLoader.LoadString("PageWidthExists"));
                        }
                        ActiveTheme.PageWidthMarkers.Remove(format.PreviousPosition);
                        format.PreviousPosition = format.Position;
                        ActiveTheme.PageWidthMarkers[format.Position] = format.Format;
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
            var format = lstLocations.SelectedItem as LineFormatAndPosition;
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
            var format = e.ListItem as LineFormatAndPosition;
            Debug.Assert(format != null);
            if (format == null) return;

            e.Value = format.Position.ToString(CultureInfo.CurrentUICulture);
        }

        private void btnAddLocation_Click(object sender, EventArgs e) {
            if (ActiveTheme == null) return;

            var format = new LineFormatAndPosition(80, ActiveTheme.DefaultLineFormat.Clone(ActiveTheme));
            var existing = lstLocations.SelectedItem as LineFormatAndPosition;
            if (existing != null) {
                format.Position = existing.Position + 10;
                while (ActiveTheme.PageWidthMarkers.ContainsKey(format.Position)) {
                    format.Position += 10;
                }
                format.PreviousPosition = format.Position;
            }
            ActiveTheme.PageWidthMarkers[format.Position] = format.Format;
            OnThemeChanged(ActiveTheme);
            lstLocations.Items.Add(format);
            lstLocations.SelectedItem = format;
        }

        private void btnRemoveLocation_Click(object sender, EventArgs e) {
            if (ActiveTheme == null) return;

            int index = lstLocations.SelectedIndex;
            var existing = lstLocations.SelectedItem as LineFormatAndPosition;
            if (existing != null) {
                ActiveTheme.PageWidthMarkers.Remove(existing.Position);
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
