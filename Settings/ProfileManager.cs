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

        public void LoadSettingsFromStorage() {
            var service = (IIndentGuide)Site.GetService(typeof(SIndentGuide));
            LoadSettingsFromStorage(service);
        }

        public void LoadSettingsFromStorage(IIndentGuide service) {
            service.Themes.Clear();
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

                if (service.DefaultTheme == null)
                    service.DefaultTheme = new IndentTheme();
            } catch (Exception ex) {
                Trace.WriteLine(string.Format("IndentGuide::LoadSettingsFromStorage: {0}", ex));
            } finally {
                if (reg != null) reg.Close();
            }
        }

        public void LoadSettingsFromXml(IVsSettingsReader reader) {
            var service = (IIndentGuide)Site.GetService(typeof(SIndentGuide));

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
            reader.ReadSettingBoolean("Visible", out tempInt);
            service.Visible = (tempInt != 0);
            string tempString;
            reader.ReadSettingString("CaretHandler", out tempString);
            service.CaretHandler = string.IsNullOrEmpty(tempString) ? null : tempString;
        }

        public void ResetSettings() {
            var service = (IIndentGuide)Site.GetService(typeof(SIndentGuide));
            ResetSettings(service);
        }

        public void ResetSettings(IIndentGuide service) {
            service.Package.UserRegistryRoot.DeleteSubKeyTree(SUBKEY_NAME, false);
        }

        public void SaveSettingsToStorage() {
            var service = (IIndentGuide)Site.GetService(typeof(SIndentGuide));
            SaveSettingsToStorage(service);
        }

        public void SaveSettingsToStorage(IIndentGuide service) {
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
            var service = (IIndentGuide)Site.GetService(typeof(SIndentGuide));

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
            var service = (IIndentGuide)Site.GetService(typeof(SIndentGuide));
            SaveVisibleSettingToStorage(service);
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
    }
}
