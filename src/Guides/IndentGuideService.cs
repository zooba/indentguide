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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace IndentGuide {
    sealed class CaretHandlerInfo : ICaretHandlerInfo {
        public string DisplayName;
        public string Documentation;
        public string TypeName;
        public int SortOrder;

        string ICaretHandlerInfo.DisplayName { get { return DisplayName; } }
        string ICaretHandlerInfo.Documentation { get { return Documentation; } }
        string ICaretHandlerInfo.TypeName { get { return TypeName; } }
    }

    /// <summary>
    /// Implementation of the service supporting Indent Guides.
    /// </summary>
    class IndentGuideService : SIndentGuide, IIndentGuide2, IDisposable {
        private bool IsDisposed;
        private readonly IndentGuidePackage _Package;
        private bool _Visible;
        private readonly Stack<TemporarySettingStore> Preserved = new Stack<TemporarySettingStore>();
        
        private const string SUBKEY_NAME = "IndentGuide";
        private const string CARETHANDLERS_SUBKEY_NAME = "Caret Handlers";

        private const string DefaultCollection = "IndentGuide";
        private const string CaretHandlersCollection = DefaultCollection + "\\Caret Handlers";

        private readonly Dictionary<string, IndentTheme> _Themes = new Dictionary<string, IndentTheme>();

        public IndentGuideService(IndentGuidePackage package) {
            _Package = package;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~IndentGuideService() {
            Dispose(false);
        }

        protected void Dispose(bool isDisposing) {
            if (!IsDisposed && isDisposing) {
                IsDisposed = true;
            }
        }

        public int Version {
            get { return IndentGuidePackage.Version; }
        }

        public IndentGuidePackage Package {
            get { return _Package; }
        }

        public bool Visible {
            get { return _Visible; }
            set {
                if (_Visible != value) {
                    _Visible = value;

                    // Save the setting immediately.
                    SaveVisibleSettingToStorage();

                    var evt = VisibleChanged;
                    if (evt != null) evt(this, EventArgs.Empty);
                }
            }
        }

        public bool PreserveSettings() {
            lock (Preserved) {
                var preserved = new TemporarySettingStore();
                Save(preserved);
                Preserved.Push(preserved);
            }
            return true;
        }

        public bool AcceptSettings() {
            bool result = false;
            lock (Preserved) {
                Preserved.Pop();
                result = Preserved.Count == 0;
            }
            if (result) {
                OnThemesChanged();
            }
            return result;
        }

        public bool RollbackSettings() {
            bool result = false;
            lock (Preserved) {
                if (Preserved.Count > 0) {
                    Load(Preserved.Pop());
                    result = true;
                }
            }
            if (result) {
                OnThemesChanged();
            }
            return result;
        }

        private void SaveVisibleSettingToStorage() {
            using (var reg = Package.UserRegistryRoot.OpenSubKey(SUBKEY_NAME, true)) {
                if (reg != null) {
                    // Key already exists, so just update this setting.
                    reg.SetValue("Visible", Visible ? 1 : 0);
                    return;
                }
            }

            // Key doesn't exist, so save all settings.
            Save();
        }

        public event EventHandler VisibleChanged;

        [Obsolete("CaretHandler has been moved to IndentTheme")]
        public string CaretHandler {
            get { return null; }
            set { }
        }

        [Obsolete("CaretHandlerChanged has been moved to IndentTheme")]
        public event EventHandler CaretHandlerChanged { add { } remove { } }

        public IDictionary<string, IndentTheme> Themes { get { return _Themes; } }
        public IndentTheme DefaultTheme { get; set; }

        public void OnThemesChanged() {
            Save();
            var evt = ThemesChanged;
            if (evt != null) evt(this, EventArgs.Empty);
        }

        public event EventHandler ThemesChanged;

        public void Save() {
            lock (Preserved) {
                if (Preserved.Count > 0) {
                    return;
                }
            }

            using (var root = Package.UserRegistryRoot)
            using (var reg = root != null ? root.CreateSubKey(SUBKEY_NAME) : null) {
                if (reg != null) {
                    try {
                        SaveToRegistry(reg);
                    } catch (Exception ex) {
                        Trace.WriteLine(string.Format("IndentGuideService::Save: {0}", ex));
                    }
                }
            }
        }

        private void SaveToRegistry(RegistryKey reg) {
            Debug.Assert(reg != null, "reg cannot be null");

            lock (_Themes) {
                reg.SetValue("Version", Version);
                reg.SetValue("Visible", Visible ? 1 : 0);

                foreach (var key in reg.GetSubKeyNames()) {
                    if (CARETHANDLERS_SUBKEY_NAME.Equals(key, StringComparison.InvariantCulture)) {
                        continue;
                    }
                    reg.DeleteSubKeyTree(key);
                }

                if (DefaultTheme != null) {
                    DefaultTheme.Save(reg);
                }

                foreach (var theme in _Themes.Values) {
                    theme.Save(reg);
                }
            }
        }

        public void Save(IVsSettingsWriter writer) {
            ThreadHelper.ThrowIfNotOnUIThread();
            lock (_Themes)
            {
                var sb = new StringBuilder();
                if (DefaultTheme != null) {
                    sb.Append(DefaultTheme.Save(writer));
                    sb.Append(";");
                }

                foreach (var theme in _Themes.Values) {
                    sb.Append(theme.Save(writer));
                    sb.Append(";");
                }

                writer.WriteSettingLong("Version", Version);
                writer.WriteSettingString("Themes", sb.ToString());
                writer.WriteSettingLong("Visible", Visible ? 1 : 0);
            }
        }

        public void Load() {
            lock (Preserved) {
                if (Preserved.Count > 0) {
                    return;
                }
            }

            using (var root = Package.UserRegistryRoot)
            using (var reg = root != null ? root.OpenSubKey(SUBKEY_NAME) : null) {
                if (reg != null) {
                    try {
                        LoadFromRegistry(reg);
                    } catch (Exception ex) {
                        Trace.WriteLine(string.Format("IndentGuideService::Load: {0}", ex));
                    }
                } else {
                    // No settings, so just ensure we're visible
                    Visible = true;
                }
            }
        }

        private void LoadFromRegistry(RegistryKey reg) {
            Debug.Assert(reg != null, "reg cannot be null");

            lock (_Themes) {
                _Themes.Clear();
                DefaultTheme = new IndentTheme();
                foreach (var themeName in reg.GetSubKeyNames()) {
                    if (CARETHANDLERS_SUBKEY_NAME.Equals(themeName, StringComparison.InvariantCulture)) {
                        continue;
                    }
                    var theme = IndentTheme.Load(reg, themeName);
                    if (theme.IsDefault) {
                        DefaultTheme = theme;
                    } else {
                        _Themes[theme.ContentType] = theme;
                    }
                }

                Visible = (int)reg.GetValue("Visible", 1) != 0;
            }

            OnThemesChanged();
        }

        public void Load(IVsSettingsReader reader) {
            ThreadHelper.ThrowIfNotOnUIThread();
            lock (_Themes) {
                _Themes.Clear();
                DefaultTheme = new IndentTheme();

                string themeKeysString;
                reader.ReadSettingString("Themes", out themeKeysString);

                foreach (var key in themeKeysString.Split(';')) {
                    if (string.IsNullOrWhiteSpace(key)) continue;

                    try {
                        var theme = IndentTheme.Load(reader, key);
                        if (theme.IsDefault) {
                            DefaultTheme = theme;
                        } else {
                            _Themes[theme.ContentType] = theme;
                        }
                    } catch (Exception ex) {
                        Trace.WriteLine(string.Format("IndentGuide::LoadSettingsFromXML: {0}", ex));
                    }
                }

                int tempInt;
                reader.ReadSettingLong("Visible", out tempInt);
                Visible = (tempInt != 0);
            }

            OnThemesChanged();
        }

        public void Reset() {
            Package.UserRegistryRoot.DeleteSubKeyTree(SUBKEY_NAME, false);
            Load();
        }

        internal bool Upgrade() {
            var upgrade = new UpgradeManager();
            using (var root = Package.UserRegistryRoot) {
                return upgrade.Upgrade(this, root, SUBKEY_NAME);
            }
        }


        private List<string> LoadRegisteredCaretHandlers() {
            var result = new List<string>();
            result.Add(typeof(CaretNone).FullName);
            result.Add(typeof(CaretNearestLeft).FullName);
            result.Add(typeof(CaretNearestLeft2).FullName);
            result.Add(typeof(CaretAdjacent).FullName);
            result.Add(typeof(CaretAboveBelowEnds).FullName);

            using (var reg = Package.UserRegistryRoot.OpenSubKey(SUBKEY_NAME)) {
                if (reg != null) {
                    using (var subreg = reg.OpenSubKey(CARETHANDLERS_SUBKEY_NAME)) {
                        if (subreg != null) {
                            foreach (var name in subreg.GetValueNames()) {
                                result.Add((subreg.GetValue(name) as string) ?? name);
                            }
                        }
                    }
                }
            }
            return result;
        }

        private List<CaretHandlerInfo> _CaretHandlerNames;
        public IEnumerable<ICaretHandlerInfo> CaretHandlerNames {
            get {
                if (_CaretHandlerNames == null) {
                    _CaretHandlerNames = LoadRegisteredCaretHandlers()
                        .Select(n => {
                            var md = CaretHandlerBase.MetadataFromName(n);
                            return md == null ? null : new CaretHandlerInfo {
                                DisplayName = md.GetDisplayName(CultureInfo.CurrentUICulture),
                                Documentation = md.GetDocumentation(CultureInfo.CurrentUICulture),
                                TypeName = n,
                                SortOrder = md.GetSortOrder(CultureInfo.CurrentUICulture)
                            };
                        })
                        .Where(h => h != null)
                        .OrderBy(h => h.SortOrder)
                        .ThenBy(h => h.TypeName)
                        .ToList();
                }
                return _CaretHandlerNames;
            }
        }
    }
}
