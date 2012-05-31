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
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

namespace IndentGuide {
    public partial class CustomBehaviorOptionsControl : UserControl, IThemeAwareDialog {
        public CustomBehaviorOptionsControl() {
            InitializeComponent();

            gridLineMode.SelectableType = typeof(LineBehavior);
        }

        #region IThemeAwareDialog Members

        public IndentTheme ActiveTheme { get; set; }
        public IIndentGuide Service { get; set; }

        public void Activate() {
            var fac = new EditorFontAndColors();

            lineTextPreview.Font = new Font(fac.FontFamily, fac.FontSize, fac.FontBold ? FontStyle.Bold : FontStyle.Regular);
            lineTextPreview.ForeColor = fac.ForeColor;
            lineTextPreview.BackColor = fac.BackColor;
        }

        public void Apply() { }

        public void Update(IndentTheme active, IndentTheme previous) {
            if (active != null) {
                gridLineMode.SelectedObject = active.Behavior;
                lineTextPreview.Theme = active;
                lineTextPreview.Invalidate();
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

        private void gridLineMode_PropertyValueChanged(object s, EventArgs e) {
            lineTextPreview.Invalidate();

            OnThemeChanged(ActiveTheme);
            Update(ActiveTheme, ActiveTheme);
        }
    }

    [Guid("16020738-BDB9-4165-A7C1-65B51D1EE134")]
    public sealed class CustomBehaviorOptions : GenericOptions<CustomBehaviorOptionsControl> { }
}
