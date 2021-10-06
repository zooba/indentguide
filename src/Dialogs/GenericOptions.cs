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
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace IndentGuide {
    public class GenericOptions<T> : DialogPage where T : Control, IThemeAwareDialog, new() {
        private IndentGuideService _service;
        private readonly Lazy<ThemeOptionsControl> Wrapper;
        private bool IsActivated;
        private bool ShouldSave;

        public GenericOptions() {
            IsActivated = false;
            Wrapper = new Lazy<ThemeOptionsControl>(() =>
            {
                return new ThemeOptionsControl(Service, new T());
            });
        }

        internal IndentGuideService Service
        {
            get
            {
                var s = _service;
                if (s != null)
                    return s;
                s = ThreadHelper.JoinableTaskFactory.Run(ServiceProvider.GetGlobalServiceAsync<SIndentGuide, IndentGuideService>);
                s = Interlocked.CompareExchange(ref _service, s, null) ?? s;
                return s;
            }
        }


        protected override IWin32Window Window {
            get { return Wrapper.Value; }
        }

        protected override void OnActivate(CancelEventArgs e) {
            if (IsActivated == false) {
                ((IndentGuideService)Service).PreserveSettings();
                IsActivated = true;
                ShouldSave = false;
            }
            base.OnActivate(e);
            Wrapper.Value.ActivateAsync().FileAndForget("stevedower/indentguide/WrapperActivateAsync");
        }

        protected override void OnApply(DialogPage.PageApplyEventArgs e) {
            Wrapper.Value.Apply();
            base.OnApply(e);
            ShouldSave = true;
        }

        protected override void OnClosed(EventArgs e) {
            if (!IsActivated) {
                // Do nothing
            } else {
                if (Service is IndentGuideService service) {
                    if (ShouldSave) {
                        service.AcceptSettings();
                    } else {
                        service.RollbackSettings();
                    }
                }
            }
            // Settings are saved automatically by the final accept/rollback.
            IsActivated = false;

            Wrapper.Value.Close();
            base.OnClosed(e);
        }
    }
}
