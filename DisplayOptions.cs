using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.Win32;

namespace IndentGuide
{
    [ComVisible(true)]
    public sealed class DisplayOptions : DialogPage
    {
        private IndentGuideService Service;
        private IVsTextManager TextManagerService;
        internal IVsEditorAdaptersFactoryService EditorAdapters;

        public DisplayOptions()
        {
            var provider = ServiceProvider.GlobalProvider;
            Service = (IndentGuideService)provider.GetService(typeof(SIndentGuide));
            TextManagerService = (IVsTextManager)provider.GetService(typeof(SVsTextManager));
            
            var componentModel = (IComponentModel)provider.GetService(typeof(SComponentModel));
            EditorAdapters = (IVsEditorAdaptersFactoryService)componentModel
                .GetService<IVsEditorAdaptersFactoryService>();
        }

        private DisplayOptionsControl _Window = null;
        protected override System.Windows.Forms.IWin32Window Window
        {
            get
            {
                if (_Window == null)
                {
                    var newWindow = new DisplayOptionsControl(this);
                    System.Threading.Interlocked.CompareExchange(ref _Window, newWindow, null);
                }
                return _Window;
            }
        }

        public override void LoadSettingsFromStorage()
        {
            Service.Load();
        }

        public override void LoadSettingsFromXml(Microsoft.VisualStudio.Shell.Interop.IVsSettingsReader reader)
        {
            Service.Load(reader);
        }

        public override void SaveSettingsToStorage()
        {
            Service.Save();
        }

        public override void SaveSettingsToXml(Microsoft.VisualStudio.Shell.Interop.IVsSettingsWriter writer)
        {
            Service.Save(writer);
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);
            var doc = (DisplayOptionsControl)Window;
            doc.LocalThemes = Service.Themes.Values.OrderBy(t => t).Select(t => t.Clone()).ToList();

            try
            {
                IVsTextView view = null;
                IWpfTextView wpfView = null;
                TextManagerService.GetActiveView(0, null, out view);
                wpfView = EditorAdapters.GetWpfTextView(view);
                doc.CurrentContentType = wpfView.TextDataModel.ContentType.DisplayName;
            }
            catch
            {
                doc.CurrentContentType = null;
            }
        }

        protected override void OnApply(DialogPage.PageApplyEventArgs e)
        {
            var doc = (DisplayOptionsControl)Window;
            doc.SaveIfRequired();
            
            var changedThemes = doc.ChangedThemes;
            var deletedThemes = doc.DeletedThemes;
            if (changedThemes.Any())
            {
                foreach (var theme in changedThemes)
                {
                    Service.Themes[theme.Name] = theme;
                    if (theme.IsDefault) Service.DefaultTheme = theme;
                }
                if (!deletedThemes.Any()) Service.OnThemesChanged();
                changedThemes.Clear();
            }
            if (deletedThemes.Any())
            {
                foreach (var theme in deletedThemes)
                {
                    Service.Themes.Remove(theme.Name);
                }
                Service.OnThemesChanged();
                deletedThemes.Clear();
            }

            base.OnApply(e);
        }

        public override void ResetSettings()
        {
            Service.Reset();
        }
    }
}
