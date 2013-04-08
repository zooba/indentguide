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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace IndentGuide {
    [Guid("6443B4D2-311B-41B6-AEC3-1DC34DF670FA")]
    partial class ProfileManager : Component, IProfileManager {
        private const string SUBKEY_NAME = "IndentGuide";

        private IIndentGuide Service {
            get {
                if (Site != null) {
                    return (IIndentGuide)Site.GetService(typeof(SIndentGuide));
                } else {
                    return (IIndentGuide)ServiceProvider.GlobalProvider.GetService(typeof(SIndentGuide));
                }
            }
        }

        internal List<string> LoadRegisteredCaretHandlers(IIndentGuide service) {
            RegistryKey reg = null;
            try {
                reg = service.Package.UserRegistryRoot.OpenSubKey(SUBKEY_NAME);
                return ((reg.GetValue("CaretHandlers") as string) ?? "")
                    .Split(';')
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .ToList();
            } finally {
                reg.Close();
            }
        }

        public void LoadSettingsFromStorage() {
            LoadSettingsFromStorage(Service);
        }

        public void LoadSettingsFromStorage(IIndentGuide service) {
            lock (PreservedLock) {
                if (Preserved != null) {
                    return;
                }
            }
            service.Themes.Clear();
            service.DefaultTheme = new IndentTheme(); 
            RegistryKey reg = null;
            try {
                reg = service.Package.UserRegistryRoot.OpenSubKey(SUBKEY_NAME);

                if (reg != null) {
                    foreach (var themeName in reg.GetSubKeyNames()) {
                        var theme = IndentTheme.Load(reg, themeName);
                        if (theme.IsDefault) {
                            service.DefaultTheme = theme;
                        } else {
                            service.Themes[theme.ContentType] = theme;
                        }
                    }

                    service.Visible = (int)reg.GetValue("Visible", 1) != 0;
                } else {
                    service.Visible = true;
                }
            } catch (Exception ex) {
                Trace.WriteLine(string.Format("IndentGuide::LoadSettingsFromStorage: {0}", ex));
            } finally {
                if (reg != null) reg.Close();
            }
        }

        public void LoadSettingsFromXml(IVsSettingsReader reader) {
            var service = Service;
            service.Themes.Clear();
            service.DefaultTheme = new IndentTheme();

            string themeKeysString;
            reader.ReadSettingString("Themes", out themeKeysString);

            foreach (var key in themeKeysString.Split(';')) {
                if (string.IsNullOrWhiteSpace(key)) continue;

                try {
                    var theme = IndentTheme.Load(reader, key);
                    if (theme.IsDefault) {
                        service.DefaultTheme = theme;
                    } else {
                        service.Themes[theme.ContentType] = theme;
                    }
                } catch (Exception ex) {
                    Trace.WriteLine(string.Format("IndentGuide::LoadSettingsFromXML: {0}", ex));
                }
            }

            int tempInt;
            reader.ReadSettingLong("Visible", out tempInt);
            service.Visible = (tempInt != 0);
        }

        public void ResetSettings() {
            ResetSettings(Service);
        }

        public void ResetSettings(IIndentGuide service) {
            service.Package.UserRegistryRoot.DeleteSubKeyTree(SUBKEY_NAME, false);
        }

        public void SaveSettingsToStorage() {
            SaveSettingsToStorage(Service);
        }

        public void SaveSettingsToStorage(IIndentGuide service) {
            lock (PreservedLock) {
                if (Preserved != null) {
                    return;
                }
            }

            RegistryKey reg = null;
            try {
                reg = service.Package.UserRegistryRoot.CreateSubKey(SUBKEY_NAME);

                reg.SetValue("Version", service.Version);
                reg.SetValue("Visible", service.Visible ? 1 : 0);

                foreach (var key in reg.GetSubKeyNames()) {
                    reg.DeleteSubKeyTree(key);
                }

                if (service.DefaultTheme != null) {
                    service.DefaultTheme.Save(reg);
                }

                foreach (var theme in service.Themes.Values) {
                    theme.Save(reg);
                }
            } catch (Exception ex) {
                Trace.WriteLine(string.Format("IndentGuide::ProfileManager::SaveSettingsToStorage: {0}", ex));
            } finally {
                if (reg != null) reg.Close();
            }
        }

        public void SaveSettingsToXml(Microsoft.VisualStudio.Shell.Interop.IVsSettingsWriter writer) {
            var service = Service;

            var sb = new StringBuilder();
            if (service.DefaultTheme != null) {
                sb.Append(service.DefaultTheme.Save(writer));
                sb.Append(";");
            }

            foreach (var theme in service.Themes.Values) {
                sb.Append(theme.Save(writer));
                sb.Append(";");
            }

            writer.WriteSettingLong("Version", service.Version);
            writer.WriteSettingString("Themes", sb.ToString());
            writer.WriteSettingLong("Visible", service.Visible ? 1 : 0);
        }

        public void SaveVisibleSettingToStorage() {
            SaveVisibleSettingToStorage(Service);
        }

        public void SaveVisibleSettingToStorage(IIndentGuide service) {
            using (var reg = service.Package.UserRegistryRoot.OpenSubKey(SUBKEY_NAME, true)) {
                if (reg != null) {
                    // Key already exists, so just update this setting.
                    reg.SetValue("Visible", service.Visible ? 1 : 0);
                    return;
                }
            }

            // Key doesn't exist, so save all settings.
            SaveSettingsToStorage(service);
        }


        static readonly object PreservedLock = new object();
        static TemporarySettingStore Preserved = null;
        static int PreservedCount = 0;

        public bool PreserveSettings() {
            lock (PreservedLock) {
                if (Preserved == null) {
                    var preserved = new TemporarySettingStore();
                    SaveSettingsToXml(preserved);
                    Preserved = preserved;
                    PreservedCount += 1;
                    return true;
                } else {
                    PreservedCount += 1;
                    return false;
                }
            }
        }

        public bool AcceptSettings() {
            lock (PreservedLock) {
                if (PreservedCount == 0) {
                    return false;
                } else if (PreservedCount == 1) {
                    Preserved = null;
                    PreservedCount = 0;
                    var service = Service as IndentGuideService;
                    if (service != null) {
                        service.OnThemesChanged();
                    }
                    return true;
                } else {
                    PreservedCount -= 1;
                    return false;
                }
            }
        }

        public bool RollbackSettings() {
            lock (PreservedLock) {
                if (PreservedCount == 0) {
                    return false;
                } else if (Preserved != null) {
                    LoadSettingsFromXml(Preserved);
                    Preserved = null;
                    PreservedCount -= 1;
                    var service = Service as IndentGuideService;
                    if (service != null) {
                        service.OnThemesChanged();
                    }
                    return true;
                } else {
                    PreservedCount -= 1;
                    return false;
                }
            }
        }
    }
}
