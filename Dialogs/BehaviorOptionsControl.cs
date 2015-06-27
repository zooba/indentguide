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
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

namespace IndentGuide {
    public partial class BehaviorOptionsControl : UserControl, IThemeAwareDialog {
        private IList<LineTextPreview> Presets;
        private readonly IEnumerable<IndentTheme> PresetThemes = new[] {
            new IndentTheme {   // preset 1
                Behavior = new LineBehavior {
                    VisibleAligned = true,
                    VisibleUnaligned = false,
                    ExtendInwardsOnly = true,
                    VisibleAtTextEnd = false,
                    VisibleEmpty = true,
                    VisibleEmptyAtEnd = true
                }
            },
            new IndentTheme {   // preset 2
                Behavior = new LineBehavior {
                    VisibleAligned = false,
                    VisibleUnaligned = true,
                    ExtendInwardsOnly = false,
                    VisibleAtTextEnd = false,
                    VisibleEmpty = true,
                    VisibleEmptyAtEnd = true
                }
            },
            new IndentTheme {   // preset 3
                Behavior = new LineBehavior {
                    VisibleAligned = true,
                    VisibleUnaligned = false,
                    ExtendInwardsOnly = true,
                    VisibleAtTextEnd = false,
                    VisibleEmpty = false,
                    VisibleEmptyAtEnd = true
                }
            },
            new IndentTheme {   // preset 4
                Behavior = new LineBehavior {
                    VisibleAligned = true,
                    VisibleUnaligned = true,
                    ExtendInwardsOnly = false,
                    VisibleAtTextEnd = false,
                    VisibleEmpty = true,
                    VisibleEmptyAtEnd = true
                }
            },
            new IndentTheme {   // preset 5
                Behavior = new LineBehavior {
                    VisibleAligned = true,
                    VisibleUnaligned = true,
                    ExtendInwardsOnly = false,
                    VisibleAtTextEnd = true,
                    VisibleEmpty = true,
                    VisibleEmptyAtEnd = true
                }
            },
            new IndentTheme {   // preset 6
                Behavior = new LineBehavior {
                    VisibleAligned = true,
                    VisibleUnaligned = false,
                    ExtendInwardsOnly = false,
                    VisibleAtTextEnd = false,
                    VisibleEmpty = true,
                    VisibleEmptyAtEnd = true
                }
            }
        };

        public BehaviorOptionsControl() {
            InitializeComponent();

            Presets = new List<LineTextPreview> { preset1, preset2, preset3, preset4, preset5, preset6 };
        }

        #region IThemeAwareDialog Members

        public IndentTheme ActiveTheme { get; set; }
        public IIndentGuide Service { get; set; }

        public void Activate() {
            var fac = new EditorFontAndColors();

            foreach (var p in Presets.Zip(PresetThemes, (x, y) => new Tuple<LineTextPreview, IndentTheme>(x, y))) {
                p.Item1.Font = new Font(fac.FontFamily, 8.0f, fac.FontBold ? FontStyle.Bold : FontStyle.Regular);
                p.Item1.ForeColor = fac.ForeColor;
                p.Item1.BackColor = fac.BackColor;
                p.Item1.HighlightBackColor = fac.HighlightBackColor;
                p.Item1.HighlightForeColor = fac.HighlightForeColor;
                p.Item1.VisibleWhitespace = true;
                p.Item1.Theme = p.Item2;
            }
        }

        public void Apply() { }

        public void Close() { }

        public void Update(IndentTheme active, IndentTheme previous) {
            if (active != null) {
                foreach (var p in Presets) {
                    p.Theme.LineFormats.Clear();
                    foreach (var kv in active.LineFormats) {
                        p.Theme.LineFormats[kv.Key] = kv.Value;
                    }
                    p.Checked = p.Theme.Behavior.Equals(active.Behavior);
                    p.Invalidate();
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

        private void Preset_Click(object sender, EventArgs e) {
            var preset = sender as LineTextPreview;
            if (preset == null) {
                return;
            }

            preset.Checked = true;
            ActiveTheme.Behavior = preset.Theme.Behavior.Clone();
            OnThemeChanged(ActiveTheme);
            
            foreach (var p in Presets) {
                if (p != preset) {
                    p.Checked = false;
                }
            }
        }
    }

    [Guid("D6E472BA-194A-46BD-B817-9107BC0DF1A1")]
    public sealed class BehaviorOptions : GenericOptions<BehaviorOptionsControl> { }
}
