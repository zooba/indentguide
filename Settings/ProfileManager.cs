using System;
using System.ComponentModel;
using System.Diagnostics;
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
                        if (theme.IsDefault)
                            service.DefaultTheme = theme;
                        else
                            service.Themes[theme.ContentType] = theme;
                    }

                    service.Visible = (int)reg.GetValue("Visible", 1) != 0;
                    service.CaretHandler = (string)reg.GetValue("CaretHandler");
                } else {
                    service.Visible = true;
                    service.CaretHandler = null;
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
                    if (theme.IsDefault)
                        service.DefaultTheme = theme;
                    else
                        service.Themes[theme.ContentType] = theme;
                } catch (Exception ex) {
                    Trace.WriteLine(string.Format("IndentGuide::LoadSettingsFromXML: {0}", ex));
                }
            }

            int tempInt;
            reader.ReadSettingLong("Visible", out tempInt);
            service.Visible = (tempInt != 0);
            string tempString;
            reader.ReadSettingString("CaretHandler", out tempString);
            service.CaretHandler = string.IsNullOrEmpty(tempString) ? null : tempString;
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
                if (string.IsNullOrEmpty(service.CaretHandler)) {
                    reg.DeleteValue("CaretHandler", false);
                } else {
                    reg.SetValue("CaretHandler", service.CaretHandler);
                }

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
            if (!string.IsNullOrEmpty(service.CaretHandler)) {
                writer.WriteSettingString("CaretHandler", service.CaretHandler);
            }
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
