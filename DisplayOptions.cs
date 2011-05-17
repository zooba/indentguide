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

        private IDictionary<string, IndentTheme> Themes;

        public DisplayOptions()
        {
            Themes = new Dictionary<string, IndentTheme>();
            var provider = ServiceProvider.GlobalProvider;
            Service = (IndentGuideService)provider.GetService(typeof(SIndentGuide));
            TextManagerService = (IVsTextManager)provider.GetService(typeof(SVsTextManager));
            
            var componentModel = (IComponentModel)provider.GetService(typeof(SComponentModel));
            EditorAdapters = (IVsEditorAdaptersFactoryService)componentModel
                .GetService<IVsEditorAdaptersFactoryService>();

            Service.Themes = Themes;
            Service.DefaultTheme = new IndentTheme(true);

            Upgrade();
            LoadSettingsFromStorage();
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

        internal RegistryKey GetRegistryRoot(bool writable)
        {
            using (var key = Service.Package.UserRegistryRoot)
                return key.OpenSubKey("IndentGuide", writable);
        }

        public override void LoadSettingsFromStorage()
        {
            Themes.Clear();
            RegistryKey reg = null;
            try
            {
                reg = GetRegistryRoot(false);
                foreach (var themeName in reg.GetSubKeyNames())
                {
                    var theme = IndentTheme.Load(reg, themeName);
                    if (theme.IsDefault) Service.DefaultTheme = theme;
                    Themes[theme.Name] = theme;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("LoadSettingsFromStorage: {0}", ex), "IndentGuide");
            }
            finally
            {
                if (reg != null) reg.Close();
            }
            Service.OnThemesChanged();
        }

        public override void LoadSettingsFromXml(Microsoft.VisualStudio.Shell.Interop.IVsSettingsReader reader)
        {
            string themeKeysString;
            reader.ReadSettingString("Themes", out themeKeysString);

            using (var reg = GetRegistryRoot(true))
            {
                foreach (var key in themeKeysString.Split(';'))
                {
                    if (string.IsNullOrWhiteSpace(key)) continue;

                    try
                    {
                        var theme = IndentTheme.Load(reader, key);
                        theme.Save(reg);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(string.Format("LoadSettingsFromXML: {0}", ex), "IndentGuide");
                    }
                }
            }

            LoadSettingsFromStorage();
        }

        public override void SaveSettingsToStorage()
        {
            RegistryKey reg = null;
            try
            {
                reg = GetRegistryRoot(true);
                foreach (var theme in Themes.Values)
                {
                    theme.Save(reg);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("SaveSettingsToStorage: {0}", ex), "IndentGuide");
            }
            finally
            {
                if (reg != null) reg.Close();
            }
        }

        public override void SaveSettingsToXml(Microsoft.VisualStudio.Shell.Interop.IVsSettingsWriter writer)
        {
            LoadSettingsFromStorage();
            var sb = new StringBuilder();
            foreach (var theme in Themes.Values)
            {
                sb.Append(theme.Save(writer));
                sb.Append(";");
            }
            writer.WriteSettingString("Themes", sb.ToString());
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
                using (var reg = GetRegistryRoot(true))
                {
                    foreach (var theme in deletedThemes)
                    {
                        try { theme.Delete(reg); }
                        catch { }
                        Service.Themes.Remove(theme.Name);
                    }
                }
                Service.OnThemesChanged();
                deletedThemes.Clear();
            }

            base.OnApply(e);
        }

        public override void ResetSettings()
        {
            RegistryKey reg = null;
            try
            {
                reg = GetRegistryRoot(true);
                foreach (var theme in Service.Themes.Values)
                {
                    try { theme.Delete(reg); }
                    catch { }
                }
                Service.Themes.Clear();
                var defaultTheme = new IndentTheme(true);
                Service.Themes[defaultTheme.Name] = defaultTheme;
                defaultTheme.Save(reg);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("ResetSettings: {0}", ex), "IndentGuide");
            }
            finally
            {
                if (reg != null) reg.Close();
            }
            Service.OnThemesChanged();
        }

        #region Upgrade settings from v8.2
        
        private void Upgrade()
        {
            try
            {
                var reg = GetRegistryRoot(false);
                if (reg != null)
                {
                    reg.Close();
                    return;
                }

                using (var vsRoot = Service.Package.UserRegistryRoot)
                {
                    using (var newKey = vsRoot.CreateSubKey("IndentGuide"))
                    using (var key = vsRoot.OpenSubKey(SettingsRegistryPath))
                    {
                        var theme = new IndentTheme(true);
                        if (key != null)
                        {
                            theme.Name = (string)key.GetValue("Name", IndentTheme.DefaultThemeName);
                            theme.EmptyLineMode = (EmptyLineMode)TypeDescriptor.GetConverter(typeof(EmptyLineMode))
                                .ConvertFromInvariantString((string)key.GetValue("EmptyLineMode"));

                            theme.LineFormat.LineColor = (Color)TypeDescriptor.GetConverter(typeof(Color))
                                .ConvertFromInvariantString((string)key.GetValue("LineColor"));
                            theme.LineFormat.LineStyle = (LineStyle)TypeDescriptor.GetConverter(typeof(LineStyle))
                                .ConvertFromInvariantString((string)key.GetValue("LineStyle"));
                            theme.LineFormat.Visible = bool.Parse((string)key.GetValue("Visible"));
                        }

                        theme.Save(newKey);
                    }

                    vsRoot.DeleteSubKeyTree(SettingsRegistryPath, false);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("Upgrade: {0}", ex), "IndentGuide");
            }
        }

        #endregion
    }
}
