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
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

namespace IndentGuide {
    /// <summary>
    /// Provides settings storage and update notifications.
    /// </summary>
    [Guid(Guids.IIndentGuideGuid)]
    [ComVisible(true)]
    public interface IIndentGuide {
        /// <summary>
        /// The version identifier for the service.
        /// </summary>
        int Version { get; }

        /// <summary>
        /// The package that owns this service.
        /// </summary>
        IndentGuidePackage Package { get; }

        /// <summary>
        /// Save the current settings to the registry.
        /// </summary>
        void Save();

        /// <summary>
        /// Save the current settings to <paramref name="writer"/>.
        /// </summary>
        void Save(IVsSettingsWriter writer);

        /// <summary>
        /// Load settings from the registry.
        /// </summary>
        void Load();

        /// <summary>
        /// Load settings from <paramref name="reader"/>.
        /// </summary>
        void Load(IVsSettingsReader reader);

        /// <summary>
        /// Reset the settings to their default.
        /// </summary>
        void Reset();

        /// <summary>
        /// Whether guides are shown or not.
        /// </summary>
        bool Visible { get; set; }
        /// <summary>
        /// The name of the caret handler to use.
        /// </summary>
        string CaretHandler { get; set; }
        /// <summary>
        /// The loaded themes.
        /// </summary>
        IDictionary<string, IndentTheme> Themes { get; }
        /// <summary>
        /// The default theme.
        /// </summary>
        IndentTheme DefaultTheme { get; set; }

        /// <summary>
        /// Raised when the collection of themes changes.
        /// </summary>
        event EventHandler ThemesChanged;
        /// <summary>
        /// Raised when the global visibility changes.
        /// </summary>
        event EventHandler VisibleChanged;
        /// <summary>
        /// Raised when the caret handler changes.
        /// </summary>
        event EventHandler CaretHandlerChanged;
    }

    /// <summary>
    /// The service interface.
    /// </summary>
    [Guid(Guids.SIndentGuideGuid)]
    public interface SIndentGuide { }

    /// <summary>
    /// Implementation of the service supporting Indent Guides.
    /// </summary>
    class IndentGuideService : SIndentGuide, IIndentGuide {
        public IndentGuideService(IndentGuidePackage package) {
            _Themes = new Dictionary<string, IndentTheme>();
            _Package = package;
            Profile = new ProfileManager();

            try {
                Profile.Upgrade(this);
            } catch (Exception ex) {
                Debug.Assert(false, "Error upgrading previous settings", ex.ToString());
                Trace.TraceError("Error upgrading previous settings: {0}", ex);
                Reset();
            }
            Load();
        }

        #region IIndentGuide Members

        public int Version {
            get { return IndentGuidePackage.Version; }
        }

        private readonly IndentGuidePackage _Package;
        public IndentGuidePackage Package {
            get { return _Package; }
        }

        private bool _Visible;
        public bool Visible {
            get { return _Visible; }
            set {
                if (_Visible != value) {
                    _Visible = value;

                    // Save the setting immediately.
                    Profile.SaveVisibleSettingToStorage(this);

                    var evt = VisibleChanged;
                    if (evt != null) evt(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler VisibleChanged;

        private string _CaretHandler;
        public string CaretHandler {
            get { return _CaretHandler; }
            set {
                if (!String.Equals(_CaretHandler, value, StringComparison.Ordinal)) {
                    _CaretHandler = value;

                    var evt = CaretHandlerChanged;
                    if (evt != null) evt(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler CaretHandlerChanged;

        private readonly ProfileManager Profile;

        private Dictionary<string, IndentTheme> _Themes;
        public IDictionary<string, IndentTheme> Themes { get { return _Themes; } }
        public IndentTheme DefaultTheme { get; set; }

        public void OnThemesChanged() {
            Save();
            var evt = ThemesChanged;
            if (evt != null) evt(this, EventArgs.Empty);
        }

        public event EventHandler ThemesChanged;

        public void Save() {
            Profile.SaveSettingsToStorage(this);
        }

        public void Save(IVsSettingsWriter writer) {
            Profile.SaveSettingsToXml(writer);
        }

        public void Load() {
            Profile.LoadSettingsFromStorage(this);

            OnThemesChanged();
        }

        public void Load(IVsSettingsReader reader) {
            Profile.LoadSettingsFromXml(reader);

            OnThemesChanged();
        }

        public void Reset() {
            var profile = new ProfileManager();
            profile.ResetSettings();
            Load();
        }

        #endregion
    }
}
