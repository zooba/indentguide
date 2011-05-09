using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Linq;
using System.Xml.Linq;

namespace IndentGuide
{
    [ComVisible(true)]
    public sealed class DisplayOptions : DialogPage
    {
        private IndentGuideService Service;

        private IDictionary<string, IndentTheme> Themes;

        public DisplayOptions()
        {
            Upgrade();

            Themes = new Dictionary<string, IndentTheme>();
            Service = (IndentGuideService)ServiceProvider.GlobalProvider.GetService(typeof(SIndentGuide));

            Service.Themes = Themes;
            Service.DefaultTheme = new IndentTheme(true);
        }

        private DisplayOptionsControl _Window = null;
        protected override System.Windows.Forms.IWin32Window Window
        {
            get
            {
                if (_Window == null)
                {
                    var newWindow = new DisplayOptionsControl();
                    System.Threading.Interlocked.CompareExchange(ref _Window, newWindow, null);
                }
                return _Window;
            }
        }

        private RegistryKey RegistryRoot
        {
            get
            {
                var vsRoot = VSRegistry.RegistryRoot(Microsoft.VisualStudio.Shell.Interop.__VsLocalRegistryType.RegType_UserSettings);
                return vsRoot.OpenSubKey("IndentGuide");
            }
        }

        private RegistryKey RegistryRootWritable
        {
            get
            {
                var vsRoot = VSRegistry.RegistryRoot(Microsoft.VisualStudio.Shell.Interop.__VsLocalRegistryType.RegType_UserSettings, true);
                return vsRoot.OpenSubKey("IndentGuide", true);
            }
        }

        public override void LoadSettingsFromStorage()
        {
            var reg = RegistryRoot;
            Themes.Clear();
            foreach (var themeName in reg.GetSubKeyNames())
            {
                var theme = IndentTheme.Load(reg, themeName);
                if (theme.IsDefault) Service.DefaultTheme = theme;
                Themes[theme.Name] = theme;
            }
            Service.OnThemesChanged();
        }

        public override void LoadSettingsFromXml(Microsoft.VisualStudio.Shell.Interop.IVsSettingsReader reader)
        {
            string xml;
            reader.ReadSettingXmlAsString("IndentGuide", out xml);
            var root = XElement.Parse(xml);
            Themes.Clear();
            foreach (var theme in root.Elements("Theme").Select(x => IndentTheme.Load(x)))
            {
                Themes[theme.Name] = theme;
            }
            Service.OnThemesChanged();
        }

        public override void SaveSettingsToStorage()
        {
            var reg = RegistryRootWritable;
            foreach (var theme in Themes.Values)
            {
                theme.Save(reg);
            }
        }

        public override void SaveSettingsToXml(Microsoft.VisualStudio.Shell.Interop.IVsSettingsWriter writer)
        {
            var root = new XElement("IndentGuide",
                Themes.Values.Select(t => t.ToXElement()));
            string xml = root.CreateReader().ReadOuterXml();
            writer.WriteSettingXmlFromString(xml);
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);
            ((DisplayOptionsControl)Window).LocalThemes = 
                Service.Themes.Values.OrderBy(t => t).Select(t => t.Clone()).ToList();
        }

        protected override void OnApply(DialogPage.PageApplyEventArgs e)
        {
            var changedThemes = ((DisplayOptionsControl)Window).ChangedThemes;
            var deletedThemes = ((DisplayOptionsControl)Window).DeletedThemes;
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
                var reg = RegistryRootWritable;
                foreach (var theme in deletedThemes)
                {
                    try { theme.Delete(reg); }
                    catch { }
                    Service.Themes.Remove(theme.Name);
                }
                Service.OnThemesChanged();
                deletedThemes.Clear();
            }

            base.OnApply(e);
        }

        public override void ResetSettings()
        {
            base.ResetSettings();
            
        }

        #region Upgrade settings from v8.2
        
        private void Upgrade()
        {
            if (RegistryRoot != null) return;

            var vsRoot = VSRegistry.RegistryRoot(Microsoft.VisualStudio.Shell.Interop.__VsLocalRegistryType.RegType_UserSettings, true);

            using (var newKey = vsRoot.CreateSubKey("IndentGuide"))
            using (var key = vsRoot.OpenSubKey(SettingsRegistryPath))
            {
                var theme = new IndentTheme(true);
                theme.Name = (string)key.GetValue("Name", IndentTheme.DefaultThemeName);
                theme.EmptyLineMode = (EmptyLineMode)TypeDescriptor.GetConverter(typeof(EmptyLineMode))
                    .ConvertFromInvariantString((string)key.GetValue("EmptyLineMode"));

                theme.LineFormat.LineColor = (Color)TypeDescriptor.GetConverter(typeof(Color))
                    .ConvertFromInvariantString((string)key.GetValue("LineColor"));
                theme.LineFormat.LineStyle = (LineStyle)TypeDescriptor.GetConverter(typeof(LineStyle))
                    .ConvertFromInvariantString((string)key.GetValue("LineStyle"));
                theme.LineFormat.Visible = bool.Parse((string)key.GetValue("Visible"));

                theme.Save(newKey);
            }

            vsRoot.DeleteSubKeyTree(SettingsRegistryPath, false);
        }

        #endregion
    }
}
