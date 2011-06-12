using System;
using System.ComponentModel;
using System.Drawing;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Globalization;
using System.Text;

namespace IndentGuide
{
    /// <summary>
    /// The supported styles of guideline.
    /// </summary>
    public enum LineStyle
    {
        Solid,
        Thick,
        Dotted,
        Dashed
    }

    /// <summary>
    /// The supported modes for handling empty lines.
    /// </summary>
    public enum EmptyLineMode
    {
        NoGuides,
        SameAsLineAboveActual,
        SameAsLineAboveLogical,
        SameAsLineBelowActual,
        SameAsLineBelowLogical
    }

    /// <summary>
    /// The format of a particular type of guideline.
    /// </summary>
    public class LineFormat : IEquatable<LineFormat>
    {
        public LineFormat()
        {
            Visible = true;
            LineStyle = LineStyle.Dotted;
            LineColor = Color.Teal;
        }

        public LineFormat Clone()
        {
            return new LineFormat
            {
                Visible = Visible,
                LineStyle = LineStyle,
                LineColor = LineColor
            };
        }

        public void Reset()
        {
            Visible = true;
            LineStyle = LineStyle.Dotted;
            LineColor = Color.Teal;
        }

        [ResourceDescription("VisibilityDescription")]
        [ResourceCategory("Appearance")]
        public bool Visible { get; set; }

        [ResourceDisplayName("LineStyleDisplayName")]
        [ResourceDescription("LineStyleDescription")]
        [ResourceCategory("Appearance")]
        public LineStyle LineStyle { get; set; }

        [ResourceDisplayName("LineColorDisplayName")]
        [ResourceDescription("LineColorDescription")]
        [ResourceCategory("Appearance")]
        [TypeConverter(typeof(ColorConverter))]
        public Color LineColor { get; set; }

        public static LineFormat FromInvariantStrings(string lineStyle, string lineColor, string visible)
        {
            var inst = new LineFormat();
            try
            {
                inst.LineStyle = (LineStyle)TypeDescriptor.GetConverter(typeof(LineStyle))
                    .ConvertFromInvariantString(lineStyle);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("IndentGuide::Error parsing " + lineStyle.ToString() + " into LineStyle");
                Trace.WriteLine(" - Exception: " + ex.ToString());
            }

            try
            {
                inst.LineColor = (Color)TypeDescriptor.GetConverter(typeof(Color))
                    .ConvertFromInvariantString(lineColor);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("IndentGuide::Error parsing " + lineColor.ToString() + " into LineColor");
                Trace.WriteLine(" - Exception: " + ex.ToString());
            }

            try
            {
                inst.Visible = bool.Parse(visible);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("IndentGuide::Error parsing " + visible.ToString() + " into Visible");
                Trace.WriteLine(" - Exception: " + ex.ToString());
            }
            return inst;
        }

        public void ToInvariantStrings(out string lineStyle, out string lineColor, out string visible)
        {
            try
            {
                lineStyle = TypeDescriptor.GetConverter(typeof(LineStyle))
                    .ConvertToInvariantString(LineStyle);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("IndentGuide::Error converting LineStyle into string");
                Trace.WriteLine(" - Exception: " + ex.ToString());
                lineStyle = LineStyle.Dotted.ToString();
            }

            try
            {
                lineColor = TypeDescriptor.GetConverter(typeof(Color))
                    .ConvertToInvariantString(LineColor);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("IndentGuide::Error converting LineColor into string");
                Trace.WriteLine(" - Exception: " + ex.ToString());
                lineColor = Color.Teal.ToString();
            }

            try
            {
                visible = Visible.ToString();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("IndentGuide::Error converting Visible into string");
                Trace.WriteLine(" - Exception: " + ex.ToString());
                visible = true.ToString();
            }
        }

        #region IEquatable<LineFormat> Members

        public bool Equals(LineFormat other)
        {
            if (null == other) return false;
            return LineStyle.Equals(other.LineStyle) &&
                LineColor.Equals(other.LineColor) &&
                Visible.Equals(other.Visible);
        }

        #endregion
    }

    /// <summary>
    /// A theme for a particular language or document type.
    /// </summary>
    public class IndentTheme : IComparable<IndentTheme>
    {
        public static readonly string DefaultThemeName = ResourceLoader.LoadString("DefaultThemeName");

        public event EventHandler Updated;

        internal void OnUpdated()
        {
            var evt = Updated;
            if (evt != null) Updated(this, EventArgs.Empty);
        }

        public IndentTheme(bool isDefaultTheme = false)
        {
            Name = DefaultThemeName;
            DefaultLineFormat = new LineFormat();
            NumberedOverride = new Dictionary<int, LineFormat>();
            EmptyLineMode = IndentGuide.EmptyLineMode.SameAsLineAboveLogical;
            VisibleAtText = false;
            RegistryName = isDefaultTheme ? Guid.Empty : Guid.NewGuid();
        }

        public IndentTheme Clone(bool newKey = false)
        {
            var inst = new IndentTheme();
            if (!newKey) inst.RegistryName = RegistryName;
            inst.Name = Name;
            inst.DefaultLineFormat = DefaultLineFormat.Clone();
            foreach (var item in NumberedOverride) inst.NumberedOverride[item.Key] = item.Value.Clone();
            inst.EmptyLineMode = EmptyLineMode;
            inst.VisibleAtText = VisibleAtText;
            return inst;
        }

        [ResourceDisplayName("ThemeNameDisplayName")]
        [ResourceDescription("ThemeNameDescription")]
        public string Name { get; set; }

        private Guid RegistryName;
        public bool IsDefault { get { return RegistryName.Equals(Guid.Empty); } }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LineFormat DefaultLineFormat { get; set; }

        [Browsable(false)]
        public IDictionary<int, LineFormat> NumberedOverride { get; private set; }

        private class EmptyLineModeTypeConverter : EnumResourceTypeConverter<EmptyLineMode>
        { }

        [ResourceDisplayName("EmptyLineModeDisplayName")]
        [ResourceDescription("EmptyLineModeDescription")]
        [TypeConverter(typeof(EmptyLineModeTypeConverter))]
        public EmptyLineMode EmptyLineMode { get; set; }

        [ResourceDisplayName("VisibleAtTextDisplayName")]
        [ResourceDescription("VisibleAtTextDescription")]
        [DefaultValue(false)]
        public bool VisibleAtText { get; set; }
        
        public static IndentTheme Load(RegistryKey reg, string themeKey)
        {
            var theme = new IndentTheme();

            using (var key = reg.OpenSubKey(themeKey))
            {
                theme.RegistryName = Guid.Parse(themeKey);
                theme.Name = (string)key.GetValue("Name", themeKey);
                theme.EmptyLineMode = (EmptyLineMode)TypeDescriptor.GetConverter(typeof(EmptyLineMode))
                    .ConvertFromInvariantString((string)key.GetValue("EmptyLineMode"));
                theme.VisibleAtText = bool.Parse((string)key.GetValue("VisibleAtText", "false"));

                theme.DefaultLineFormat = LineFormat.FromInvariantStrings(
                    (string)key.GetValue("LineStyle"),
                    (string)key.GetValue("LineColor"),
                    (string)key.GetValue("Visible"));

                foreach (var subkeyName in key.GetSubKeyNames())
                {
                    LineFormat format;
                    using (var subkey = key.OpenSubKey(subkeyName))
                    {
                        format = LineFormat.FromInvariantStrings(
                            (string)subkey.GetValue("LineStyle"),
                            (string)subkey.GetValue("LineColor"),
                            (string)subkey.GetValue("Visible"));
                    }
                    
                    int i;
                    if (int.TryParse(subkeyName, out i))
                    {
                        theme.NumberedOverride[i] = format;
                    }
                    else
                    {
                        Trace.WriteLine(string.Format("IndentGuide::Unable to parse {0}", subkeyName));
                    }
                }
            }

            return theme;
        }

        public static IndentTheme Load(IVsSettingsReader reader, string key)
        {
            var theme = new IndentTheme();
            string temp;

            theme.RegistryName = Guid.Parse(key);

            reader.ReadSettingString(key, out temp);
            theme.Name = temp;

            reader.ReadSettingAttribute(key, "EmptyLineMode", out temp);
            theme.EmptyLineMode = (EmptyLineMode)TypeDescriptor.GetConverter(typeof(EmptyLineMode))
                .ConvertFromInvariantString(temp);
            reader.ReadSettingAttribute(key, "VisibleAtText", out temp);
            theme.VisibleAtText = bool.Parse(temp);

            string lineStyle, lineColor, visible;
            reader.ReadSettingAttribute(key, "LineStyle", out lineStyle);
            reader.ReadSettingAttribute(key, "LineColor", out lineColor);
            reader.ReadSettingAttribute(key, "Visible", out visible);
            theme.DefaultLineFormat = LineFormat.FromInvariantStrings(lineStyle, lineColor, visible);

            string subkeys;
            reader.ReadSettingAttribute(key, "Subkeys", out subkeys);
            if (!string.IsNullOrEmpty(subkeys))
            {
                foreach (var subkey in subkeys.Split(';'))
                {
                    if (string.IsNullOrEmpty(subkeys)) continue;

                    int i = subkey.LastIndexOf('.');
                    if (i < 0) continue;

                    reader.ReadSettingAttribute(subkey, "LineStyle", out lineStyle);
                    reader.ReadSettingAttribute(subkey, "LineColor", out lineColor);
                    reader.ReadSettingAttribute(subkey, "Visible", out visible);
                    var format = LineFormat.FromInvariantStrings(lineStyle, lineColor, visible);

                    var keypart = subkey.Substring(i + 1);
                    if (int.TryParse(keypart, out i))
                    {
                        theme.NumberedOverride[i] = format;
                    }
                    else
                    {
                        Trace.WriteLine(string.Format("IndentGuide::Unable to parse {0}", keypart));
                    }
                }
            }

            return theme;
        }

        public string Save(RegistryKey reg)
        {
            using (var key = reg.CreateSubKey(RegistryName.ToString("B")))
            {
                key.SetValue("Name", Name);
                key.SetValue("EmptyLineMode", TypeDescriptor.GetConverter(typeof(EmptyLineMode))
                    .ConvertToInvariantString(EmptyLineMode));
                key.SetValue("VisibleAtText", VisibleAtText.ToString());

                string lineStyle, lineColor, visible;
                DefaultLineFormat.ToInvariantStrings(out lineStyle, out lineColor, out visible);
                key.SetValue("LineStyle", lineStyle);
                key.SetValue("LineColor", lineColor);
                key.SetValue("Visible", visible);

                foreach (var subkey in key.GetSubKeyNames())
                {
                    key.DeleteSubKeyTree(subkey, false);
                }

                foreach(var item in NumberedOverride)
                {
                    using (var subkey = key.CreateSubKey(item.Key.ToString(CultureInfo.InvariantCulture)))
                    {
                        item.Value.ToInvariantStrings(out lineStyle, out lineColor, out visible);
                        subkey.SetValue("LineStyle", lineStyle);
                        subkey.SetValue("LineColor", lineColor);
                        subkey.SetValue("Visible", visible);
                    }
                }
            }
            return RegistryName.ToString("B");
        }

        public string Save(IVsSettingsWriter writer)
        {
            var key = RegistryName.ToString();
            writer.WriteSettingString(key, Name);
            writer.WriteSettingAttribute(key, "EmptyLineMode", TypeDescriptor.GetConverter(typeof(EmptyLineMode))
                .ConvertToInvariantString(EmptyLineMode));
            writer.WriteSettingAttribute(key, "VisibleAtText", VisibleAtText.ToString());

            string lineStyle, lineColor, visible;
            DefaultLineFormat.ToInvariantStrings(out lineStyle, out lineColor, out visible);
            writer.WriteSettingAttribute(key, "LineStyle", lineStyle);
            writer.WriteSettingAttribute(key, "LineColor", lineColor);
            writer.WriteSettingAttribute(key, "Visible", visible);

            var sb = new StringBuilder();

            foreach (var item in NumberedOverride)
            {
                var subkey = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", key, item.Key);
                sb.Append(subkey);
                sb.Append(";");

                writer.WriteSettingString(subkey, "");
                
                item.Value.ToInvariantStrings(out lineStyle, out lineColor, out visible);
                writer.WriteSettingAttribute(subkey, "LineStyle", lineStyle);
                writer.WriteSettingAttribute(subkey, "LineColor", lineColor);
                writer.WriteSettingAttribute(subkey, "Visible", visible);
            }

            if (sb.Length > 1)
            {
                sb.Length -= 1;
                writer.WriteSettingAttribute(key, "Subkeys", sb.ToString());
            }

            return key;
        }

        public void Delete(RegistryKey reg)
        {
            reg.DeleteSubKeyTree(RegistryName.ToString("B"));
        }

        public int CompareTo(IndentTheme other)
        {
            if (null == other) return -1;
            if (IsDefault && other.IsDefault) return 0;
            if (IsDefault) return -1;
            if (other.IsDefault) return 1;
            return Name.CompareTo(other.Name);
        }

        internal void Apply()
        {
            var toRemove = NumberedOverride
                .Where(kv => DefaultLineFormat.Equals(kv.Value))
                .Select(kv => kv.Key)
                .ToList();
            
            foreach (var key in toRemove)
                NumberedOverride.Remove(key);
        }
    }
}
