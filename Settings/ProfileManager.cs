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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using IndentGuide.Utils;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace IndentGuide {
    [Guid("6443B4D2-311B-41B6-AEC3-1DC34DF670FA")]
    partial class ProfileManager : Component, IProfileManager {
        public override ISite Site {
            get {
                return base.Site;
            }
            set {
                base.Site = value;
            }
        }

        private IIndentGuide Service {
            get {
                return Site != null ? Site.GetService(typeof(SIndentGuide)) as IIndentGuide : null;
            }
        }

        private bool ValidateService(IIndentGuide service) {
            if (service == null) {
                Errors.Log(string.Format("service is null{0}{1}",
                    Environment.NewLine,
                    new StackTrace(1, true)
                ));
                return false;
            }
            return true;
        }

        public void LoadSettingsFromStorage() {
            var service = Service;
            if (ValidateService(service)) {
                service.Load();
            }
        }

        public void LoadSettingsFromXml(IVsSettingsReader reader) {
            var service = Service;
            if (ValidateService(service)) {
                service.Load(reader);
            }
        }

        public void ResetSettings() {
            var service = Service;
            if (ValidateService(service)) {
                service.Reset();
            }
        }

        public void SaveSettingsToStorage() {
            var service = Service;
            if (ValidateService(service)) {
                service.Save();
            }
        }

        public void SaveSettingsToXml(IVsSettingsWriter writer) {
            var service = Service;
            if (ValidateService(service)) {
                service.Save(writer);
            }
        }
    }
}
