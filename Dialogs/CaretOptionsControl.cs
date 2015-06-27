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
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace IndentGuide {
    public partial class CaretOptionsControl : UserControl, IThemeAwareDialog {
        public CaretOptionsControl() {
            InitializeComponent();

            var service = ServiceProvider.GlobalProvider.GetService(typeof(SIndentGuide)) as IIndentGuide2;
            if (service != null) {
                try {
                    lstNames.Items.AddRange(service.CaretHandlerNames.Where(md => md != null).ToArray());
                } catch (Exception ex) {
                    Trace.TraceError("CaretOptionsControl(): {0}", ex);
                }
                
            }
            if (lstNames.Items.Count == 0) {
                lstNames.Enabled = false;
            }
            webDocumentation.Document.OpenNew(true);
        }

        #region IThemeAwareDialog Members

        public IndentTheme ActiveTheme { get; set; }
        public IIndentGuide Service { get; set; }

        public void Activate() { }

        public void Apply() { }

        public void Close() { }

        public void Update(IndentTheme active, IndentTheme previous) {
            if (active != null) {
                SelectItem(active.CaretHandler);
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

        private void SelectItem(string name) {
            try {
                lstNames.Enabled = false;
                lstNames.SelectedIndex = -1;

                for (int i = 0; i < lstNames.Items.Count; ++i) {
                    var item = lstNames.Items[i] as CaretHandlerInfo;
                    if (item != null && item.TypeName.Equals(name, StringComparison.Ordinal)) {
                        lstNames.SelectedIndex = i;
                        return;
                    }
                }

                var metadata = CaretHandlerBase.MetadataFromName(name);
                for (int i = 0; i < lstNames.Items.Count; ++i) {
                    var item = lstNames.Items[i] as CaretHandlerInfo;
                    if (item != null) {
                        var metadata2 = CaretHandlerBase.MetadataFromName(item.TypeName);
                        if (metadata == metadata2) {
                            lstNames.SelectedIndex = i;
                            return;
                        }
                    }
                }

                if (lstNames.Items.Count > 0) {
                    lstNames.SelectedIndex = 0;
                }
            } finally {
                lstNames.Enabled = true;
            }
        }

        private void lstNames_SelectedIndexChanged(object sender, EventArgs e) {
            var item = lstNames.SelectedItem as CaretHandlerInfo;
            if (item == null) {
                webDocumentation.DocumentText = ResourceLoader.LoadString("NoDocumentationHtml");
            } else {
                webDocumentation.DocumentText = item.Documentation;
                var theme = ActiveTheme;
                if (theme != null && lstNames.Enabled) {
                    theme.CaretHandler = item.TypeName;
                    OnThemeChanged(theme);
                }
            }
        }

        private void lstNames_Format(object sender, ListControlConvertEventArgs e) {
            var item = e.ListItem as CaretHandlerInfo;
            if (item != null) {
                e.Value = item.DisplayName;
            } else {
                e.Value = "(null)";
            }
        }
    }

    [Guid("2DF6B764-DD1A-4B8D-86D9-630B8C2E9EEE")]
    public sealed class CaretOptions : GenericOptions<CaretOptionsControl> { }
}
