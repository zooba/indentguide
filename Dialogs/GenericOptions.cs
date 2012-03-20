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

        public override void LoadSettingsFromStorage() {
            ProfileManager.LoadSettingsFromStorage();
        }

        public override void LoadSettingsFromXml(Microsoft.VisualStudio.Shell.Interop.IVsSettingsReader reader) {
            ProfileManager.LoadSettingsFromXml(reader);
        }

        public override void SaveSettingsToStorage() {
            ProfileManager.SaveSettingsToStorage();
        }

        public override void SaveSettingsToXml(Microsoft.VisualStudio.Shell.Interop.IVsSettingsWriter writer) {
            ProfileManager.SaveSettingsToXml(writer);
        }

        public override void ResetSettings() {
            ProfileManager.ResetSettings();
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

            } else if (ShouldSave) {
                ProfileManager.AcceptSettings();
            } else {
                ProfileManager.RollbackSettings();
            }
            IsActivated = false;

            Wrapper.Close();
            base.OnClosed(e);
        }
    }
}
