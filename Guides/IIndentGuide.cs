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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;

namespace IndentGuide {
    /// <summary>
    /// Provides settings storage and update notifications.
    /// </summary>
    [Guid(Guids.IIndentGuideGuid)]
    [ComVisible(true)]
    public interface IIndentGuide {
        /// <summary>
        /// The version identifier for the 
        /// </summary>
        int Version { get; }

        /// <summary>
        /// The package that owns this 
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
        [Obsolete("CaretHandler has been moved to IndentTheme")]
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
        [Obsolete("CaretHandlerChanged has been moved to IndentTheme")]
        event EventHandler CaretHandlerChanged;
    }


    /// <summary>
    /// Provides a list of registered caret handlers.
    /// </summary>
    [Guid(Guids.IIndentGuide2Guid)]
    [ComVisible(true)]
    interface IIndentGuide2 : IIndentGuide {
        IEnumerable<ICaretHandlerInfo> CaretHandlerNames { get; }
    }

    /// <summary>
    /// Provides information about available caret handlers.
    /// </summary>
    [Guid(Guids.ICaretHandlerInfoGuid)]
    [ComVisible(true)]
    public interface ICaretHandlerInfo {
        string DisplayName { get; }
        string Documentation { get; }
        string TypeName { get; }
    }

    /// <summary>
    /// The service interface.
    /// </summary>
    [Guid(Guids.SIndentGuideGuid)]
    public interface SIndentGuide { }
}
