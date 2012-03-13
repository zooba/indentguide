using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace IndentGuide {
    public class GenericOptions<T> : DialogPage where T : Control, IThemeAwareDialog, new() {
        private IIndentGuide Service;

        public GenericOptions() {
            var provider = ServiceProvider.GlobalProvider;
            Service = (IIndentGuide)provider.GetService(typeof(SIndentGuide));
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
            Service.Load();
        }

        public override void LoadSettingsFromXml(Microsoft.VisualStudio.Shell.Interop.IVsSettingsReader reader) {
            Service.Load(reader);
        }

        public override void SaveSettingsToStorage() {
            Service.Save();
        }

        public override void SaveSettingsToXml(Microsoft.VisualStudio.Shell.Interop.IVsSettingsWriter writer) {
            Service.Save(writer);
        }

        public override void ResetSettings() {
            Service.Reset();
        }

        protected override void OnActivate(CancelEventArgs e) {
            base.OnActivate(e);
            Wrapper.Activate();
        }

        protected override void OnApply(DialogPage.PageApplyEventArgs e) {
            Wrapper.Apply();
            base.OnApply(e);
        }

        protected override void OnClosed(EventArgs e) {
            Wrapper.Close();
            base.OnClosed(e);
        }
    }
}
