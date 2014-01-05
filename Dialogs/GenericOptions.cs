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
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace IndentGuide {
    public class GenericOptions<T> : DialogPage where T : Control, IThemeAwareDialog, new() {
        private readonly ProfileManager ProfileManager;
        private bool IsActivated;
        private bool ShouldSave;

        public GenericOptions() {
            ProfileManager = new ProfileManager();
            IsActivated = false;
        }

        private T _Control = null;
        private T Control {
            get {
                if (_Control == null)
                    System.Threading.Interlocked.CompareExchange(ref _Control, new T(), null);

                return _Control;
            }
        }

        private ThemeOptionsControl _Wrapper = null;
        private ThemeOptionsControl Wrapper {
            get {
                if (_Wrapper == null)
                    System.Threading.Interlocked.CompareExchange(ref _Wrapper, new ThemeOptionsControl(Control), null);

                return _Wrapper;
            }
        }

        protected override System.Windows.Forms.IWin32Window Window {
            get { return Wrapper; }
        }

        protected override void OnActivate(CancelEventArgs e) {
            if (IsActivated == false) {
                ProfileManager.PreserveSettings();
                IsActivated = true;
                ShouldSave = false;
            }
            base.OnActivate(e);
            Wrapper.Activate();
        }

        protected override void OnApply(DialogPage.PageApplyEventArgs e) {
            Wrapper.Apply();
            base.OnApply(e);
            ShouldSave = true;
        }

        protected override void OnClosed(EventArgs e) {
            if (!IsActivated) {
                // Do nothing
            } else if (ShouldSave) {
                ProfileManager.AcceptSettings();
            } else {
                ProfileManager.RollbackSettings();
            }
            // Settings are saved automatically by the final accept/rollback.
            IsActivated = false;

            Wrapper.Close();
            base.OnClosed(e);
        }
    }
}
