using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace IndentGuide
{
    /// <summary>
    /// Provides settings storage and update notifications.
    /// </summary>
    [Guid(Guids.IIndentGuideGuid)]
    [ComVisible(true)]
    public interface IIndentGuide
    {
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
        /// The loaded themes.
        /// </summary>
        IDictionary<string, IndentTheme> Themes { get; }
        /// <summary>
        /// The default theme.
        /// </summary>
        IndentTheme DefaultTheme { get; }

        /// <summary>
        /// Raised when the collection of themes changes.
        /// </summary>
        event EventHandler ThemesChanged;
        /// <summary>
        /// Raised when the global visibility changes.
        /// </summary>
        event EventHandler VisibleChanged;
    }

    /// <summary>
    /// The service interface.
    /// </summary>
    [Guid(Guids.SIndentGuideGuid)]
    public interface SIndentGuide
    { }

    /// <summary>
    /// Implementation of the service supporting Indent Guides.
    /// </summary>
    class IndentGuideService : SIndentGuide, IIndentGuide
    {
        public IndentGuideService(IndentGuidePackage package)
        {
            _Package = package;
            _Themes = new Dictionary<string, IndentTheme>();
            try
            {
                Upgrade();
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.Assert(false);
#endif
                Trace.WriteLine("Error upgrading previous settings.");
                Trace.WriteLine(" - " + ex.ToString());
                Reset();
            }
            Load();
        }

        #region IIndentGuide Members

        private readonly IndentGuidePackage _Package;
        public IndentGuidePackage Package
        {
            get { return _Package; }
        }

        private bool _Visible = true;
        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (_Visible != value)
                {
                    _Visible = value;

                    var evt = VisibleChanged;
                    if (evt != null) evt(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler VisibleChanged;

        private readonly Dictionary<string, IndentTheme> _Themes;
        public IDictionary<string, IndentTheme> Themes { get { return _Themes; } }
        public IndentTheme DefaultTheme { get; set; }

        public void OnThemesChanged()
        {
            Save();
            var evt = ThemesChanged;
            if (evt != null) evt(this, EventArgs.Empty);
        }

        public event EventHandler ThemesChanged;

        private const string SUBKEY_NAME = "IndentGuide";


        public void Save()
        {
            RegistryKey reg = null;
            try
            {
                using (var root = Package.UserRegistryRoot)
                    reg = root.CreateSubKey(SUBKEY_NAME);

                reg.SetValue("Version", CURRENT_VERSION);
                reg.SetValue("Visible", Visible ? 1 : 0);

                foreach (var key in reg.GetSubKeyNames())
                    reg.DeleteSubKeyTree(key);

                if (DefaultTheme != null)
                    DefaultTheme.Save(reg);

                foreach (var theme in Themes.Values)
                    theme.Save(reg);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("IndentGuide::Save: {0}", ex));
            }
            finally
            {
                if (reg != null) reg.Close();
            }
        }

        public void Save(IVsSettingsWriter writer)
        {
            var sb = new StringBuilder();
            if (DefaultTheme != null)
            {
                sb.Append(DefaultTheme.Save(writer));
                sb.Append(";");
            }

            foreach (var theme in Themes.Values)
            {
                sb.Append(theme.Save(writer));
                sb.Append(";");
            }

            writer.WriteSettingLong("Version", CURRENT_VERSION);
            writer.WriteSettingString("Themes", sb.ToString());
            writer.WriteSettingLong("Visible", Visible ? 1 : 0);
        }

        public void Load()
        {
            Themes.Clear();
            RegistryKey reg = null;
            try
            {
                using (var root = Package.UserRegistryRoot)
                    reg = root.OpenSubKey(SUBKEY_NAME);

                if (reg != null)
                {
                    foreach (var themeName in reg.GetSubKeyNames())
                    {
                        var theme = IndentTheme.Load(reg, themeName);
                        if (theme.IsDefault)
                            DefaultTheme = theme;
                        else
                            Themes[theme.ContentType] = theme;
                    }

                    Visible = (int)reg.GetValue("Visible", 1) != 0;
                }
                else
                {
                    Visible = true;
                }

                if (DefaultTheme == null)
                    DefaultTheme = new IndentTheme();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("IndentGuide::LoadSettingsFromStorage: {0}", ex));
            }
            finally
            {
                if (reg != null) reg.Close();
            }

            OnThemesChanged();
        }

        public void Load(IVsSettingsReader reader)
        {
            string themeKeysString;
            reader.ReadSettingString("Themes", out themeKeysString);

            foreach (var key in themeKeysString.Split(';'))
            {
                if (string.IsNullOrWhiteSpace(key)) continue;

                try
                {
                    var theme = IndentTheme.Load(reader, key);
                    if (theme.IsDefault)
                        DefaultTheme = theme;
                    else
                        Themes[theme.ContentType] = theme;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("IndentGuide::LoadSettingsFromXML: {0}", ex));
                }
            }

            int temp;
            reader.ReadSettingBoolean("Visible", out temp);
            Visible = (temp != 0);

            OnThemesChanged();
        }

        public void Reset()
        {
            using (var root = Package.UserRegistryRoot)
                root.DeleteSubKeyTree(SUBKEY_NAME, false);

            Load();
        }

        #endregion

        // Default version is 10.9.0.0, also known as 11 (beta 1).
        // This was the version prior to the version field being added.
        private const int DEFAULT_VERSION = 0x000A0900;

        private static readonly int CURRENT_VERSION = GetCurrentVersion();

        public int Version { get { return CURRENT_VERSION; } }

        private static int GetCurrentVersion()
        {
            var assembly = typeof(IndentGuideService).Assembly;
            var attribs = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyFileVersionAttribute), false);
            if (attribs.Length == 0) return DEFAULT_VERSION;

            var attrib = (System.Reflection.AssemblyFileVersionAttribute)attribs[0];

            try
            {
                int version = attrib.Version.Split('.')
                    .Select(p => int.Parse(p))
                    .Take(3)
                    .Aggregate(0, (acc, i) => acc << 8 | i);
#if DEBUG
                Trace.WriteLine(string.Format("IndentGuideService.CURRENT_VERSION == {0:X}", version));
#endif
                return version;
            }
            catch(Exception ex)
            {
                Trace.WriteLine(string.Format("IndentGuide::GetCurrentVersion: {0}", ex));
                return DEFAULT_VERSION;
            }
        }

        private void Upgrade()
        {
            using (var root = Package.UserRegistryRoot)
            {
                int version;
                using (var reg = root.OpenSubKey(SUBKEY_NAME, false))
                    version = (reg == null) ? 0 : (int)reg.GetValue("Version", DEFAULT_VERSION);

                // v11 (beta 2) (10.9.1) and later don't require an upgrade
                if (version >= 0x000A0901)
                    return;

                bool writeDefaults = true;
                // v9 and later can be upgraded.
                if (version == DEFAULT_VERSION)
                {
                    using (var reg = root.CreateSubKey(SUBKEY_NAME))
                    {
                        foreach (var keyName in reg.GetSubKeyNames())
                        {
                            if (keyName != IndentTheme.DefaultThemeName)
                                writeDefaults = false;

                            using (var key = reg.OpenSubKey(keyName))
                            {
                                if (key == null) continue;
                                var name = key.GetValue("Name") as string;
                                if (string.IsNullOrEmpty(name)) continue;

                                using (var newKey = reg.CreateSubKey(name))
                                {
                                    newKey.SetValue("VisibleAligned", 1);
                                    newKey.SetValue("VisibleUnaligned", 0);

                                    // Upgrade the old EmptyLineMode enumeration
                                    var elm = key.GetValue("EmptyLineMode") as string ?? "SameAsAboveLogical";
                                    switch (elm)
                                    {
                                        case "NoGuides":
                                            newKey.SetValue("TopToBottom", 1);
                                            newKey.SetValue("VisibleEmpty", 0);
                                            newKey.SetValue("VisibleEmptyAtEnd", 0);
                                            break;
                                        case "SameAsLineAboveActual":
                                            newKey.SetValue("TopToBottom", 1);
                                            newKey.SetValue("VisibleEmpty", 1);
                                            newKey.SetValue("VisibleEmptyAtEnd", 0);
                                            break;
                                        case "SameAsLineAboveLogical":
                                            newKey.SetValue("TopToBottom", 1);
                                            newKey.SetValue("VisibleEmpty", 1);
                                            newKey.SetValue("VisibleEmptyAtEnd", 1);
                                            break;
                                        case "SameAsLineBelowActual":
                                            newKey.SetValue("TopToBottom", 0);
                                            newKey.SetValue("VisibleEmpty", 1);
                                            newKey.SetValue("VisibleEmptyAtEnd", 0);
                                            break;
                                        case "SameAsLineBelowLogical":
                                            newKey.SetValue("TopToBottom", 0);
                                            newKey.SetValue("VisibleEmpty", 1);
                                            newKey.SetValue("VisibleEmptyAtEnd", 1);
                                            break;
                                    }

                                    // Upgrade the old VisibleAtText setting (default to false)
                                    var ate = string.Equals("True", key.GetValue("VisibleAtText") as string, StringComparison.InvariantCultureIgnoreCase);
                                    newKey.SetValue("VisibleAtTextEnd", ate ? 1 : 0);

                                    // Copy the default color/style to Default, Unaligned and Caret themes.
                                    // Change the Caret theme color to Red, or Teal if it was already red.
                                    using (var subKey1 = newKey.CreateSubKey(IndentTheme.FormatIndexToString(IndentTheme.DefaultFormatIndex)))
                                    using (var subKey2 = newKey.CreateSubKey(IndentTheme.FormatIndexToString(IndentTheme.UnalignedFormatIndex)))
                                    using (var subKey3 = newKey.CreateSubKey(IndentTheme.FormatIndexToString(IndentTheme.CaretFormatIndex)))
                                    {
                                        var visible = !string.Equals("False", key.GetValue("Visible") as string, StringComparison.InvariantCultureIgnoreCase);
                                        var color = key.GetValue("LineColor") as string ?? "Teal";
                                        var style = key.GetValue("LineStyle") as string ?? "Dotted";
                                        subKey1.SetValue("Visible", visible ? 1 : 0);
                                        subKey2.SetValue("Visible", visible ? 1 : 0);
                                        subKey3.SetValue("Visible", visible ? 1 : 0);
                                        subKey1.SetValue("LineColor", color);
                                        subKey2.SetValue("LineColor", color);
                                        subKey3.SetValue("LineColor", (color == "Red") ? "Teal" : "Red");
                                        subKey1.SetValue("LineStyle", style);
                                        subKey2.SetValue("LineStyle", style);
                                        subKey3.SetValue("LineStyle", style);
                                    }

                                    // Copy the existing indent overrides.
                                    foreach (var subkeyName in key.GetSubKeyNames())
                                    {
                                        int formatIndex;
                                        if (!int.TryParse(subkeyName, out formatIndex))
                                            continue;

                                        using (var subKey1 = key.OpenSubKey(subkeyName))
                                        using (var subKey2 = newKey.CreateSubKey(subkeyName))
                                        {
                                            var visible = !string.Equals("False", subKey1.GetValue("Visible") as string, StringComparison.InvariantCultureIgnoreCase);
                                            var color = subKey1.GetValue("LineColor") as string ?? "Teal";
                                            var style = subKey1.GetValue("LineStyle") as string ?? "Dotted";
                                            subKey2.SetValue("Visible", visible ? 1 : 0);
                                            subKey2.SetValue("LineColor", color);
                                            subKey2.SetValue("LineStyle", style);
                                        }
                                    }
                                }
                            }

                            reg.DeleteSubKeyTree(keyName);
                        }
                    }
                }

                if (writeDefaults)
                {
                    // TODO: Add default settings and themes.
                    //using (var reg = root.CreateSubKey(SUBKEY_NAME))
                    //{ }
                }
            }
        }
    }
}
