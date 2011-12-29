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

        public static LineFormat FromInvariantStrings(string lineStyle, string lineColor, int visible)
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
                inst.Visible = visible != 0;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("IndentGuide::Error parsing " + visible.ToString() + " into Visible");
                Trace.WriteLine(" - Exception: " + ex.ToString());
            }
            return inst;
        }

        public void ToInvariantStrings(out string lineStyle, out string lineColor, out int visible)
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
                visible = Visible ? 1 : 0;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("IndentGuide::Error converting Visible into string");
                Trace.WriteLine(" - Exception: " + ex.ToString());
                visible = 1;
            }
        }

        public static LineFormat FromInvariantStrings(string lineStyle, string lineColor, string visible)
        {
            int visibleInt = 1;
            try
            {
                visibleInt = bool.Parse(visible) ? 1 : 0;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("IndentGuide::Error converting Visible into bool");
                Trace.WriteLine(" - Exception: " + ex.ToString());
            }
            return FromInvariantStrings(lineStyle, lineColor, visibleInt);
        }

        public void ToInvariantStrings(out string lineStyle, out string lineColor, out string visible)
        {
            int visibleInt;
            ToInvariantStrings(out lineStyle, out lineColor, out visibleInt);
            visible = (visibleInt != 0).ToString();
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

        public IndentTheme()
        {
            ContentType = null;
            DefaultLineFormat = new LineFormat();
            NumberedOverride = new Dictionary<int, LineFormat>();
            TopToBottom = true;
            VisibleAligned = true;
            VisibleUnaligned = false;
            VisibleAtTextEnd = false;
            VisibleEmpty = true;
            VisibleEmptyAtEnd = true;
        }

        public IndentTheme Clone()
        {
            var inst = new IndentTheme();
            inst.ContentType = ContentType;
            inst.DefaultLineFormat = DefaultLineFormat.Clone();
            foreach (var item in NumberedOverride) inst.NumberedOverride[item.Key] = item.Value.Clone();
            inst.VisibleAligned = VisibleAligned;
            inst.VisibleUnaligned = VisibleUnaligned;
            inst.VisibleAtTextEnd = VisibleAtTextEnd;
            inst.VisibleEmpty = VisibleEmpty;
            inst.VisibleEmptyAtEnd = VisibleEmptyAtEnd;
            return inst;
        }

        [ResourceDisplayName("ContentTypeDisplayName")]
        [ResourceDescription("ContentTypeDescription")]
        public string ContentType { get; set; }

        public bool IsDefault { get { return ContentType == null; } }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LineFormat DefaultLineFormat { get; set; }

        [Browsable(false)]
        public IDictionary<int, LineFormat> NumberedOverride { get; private set; }

        /// <summary>
        /// True to scan from top to bottom; false to scan from bottom to top.
        /// </summary>
        [ResourceDisplayName("TopToBottomDisplayName")]
        [ResourceDescription("TopToBottomDescription")]
        public bool TopToBottom { get; set; }

        /// <summary>
        /// True to copy guidelines from the previous non-empty line into empty
        /// lines.
        /// </summary>
        [ResourceDisplayName("VisibleEmptyDisplayName")]
        [ResourceDescription("VisibleEmptyDescription")]
        public bool VisibleEmpty { get; set; }

        /// <summary>
        /// True to copy 'at end' guidelines from the previous non-empty line
        /// into empty lines.
        /// </summary>
        [ResourceDisplayName("VisibleEmptyAtEndDisplayName")]
        [ResourceDescription("VisibleEmptyAtEndDescription")]
        public bool VisibleEmptyAtEnd { get; set; }

        /// <summary>
        /// True to display guidelines at transitions from whitespace to text.
        /// </summary>
        [ResourceDisplayName("VisibleAtTextEndDisplayName")]
        [ResourceDescription("VisibleAtTextEndDescription")]
        public bool VisibleAtTextEnd { get; set; }

        /// <summary>
        /// True to always display guidelines at multiples of indent size.
        /// </summary>
        [ResourceDisplayName("VisibleAlignedDisplayName")]
        [ResourceDescription("VisibleAlignedDescription")]
        public bool VisibleAligned { get; set; }

        /// <summary>
        /// True to always display guidelines at textual indents.
        /// </summary>
        [ResourceDisplayName("VisibleUnalignedDisplayName")]
        [ResourceDescription("VisibleUnalignedDescription")]
        public bool VisibleUnaligned { get; set; }

        public static IndentTheme Load(RegistryKey reg, string themeKey)
        {
            var theme = new IndentTheme();

            using (var key = reg.OpenSubKey(themeKey))
            {
                theme.ContentType = (themeKey == DefaultThemeName) ? null : themeKey;
                theme.TopToBottom = (int)key.GetValue("TopToBottom", 1) != 0;
                theme.VisibleAligned = (int)key.GetValue("VisibleAligned", 1) != 0;
                theme.VisibleUnaligned = (int)key.GetValue("VisibleUnaligned", 0) != 0;
                theme.VisibleAtTextEnd = (int)key.GetValue("VisibleAtTextEnd", 0) != 0;
                theme.VisibleEmpty = (int)key.GetValue("VisibleEmpty", 1) != 0;
                theme.VisibleEmptyAtEnd = (int)key.GetValue("VisibleEmptyAtEnd", 1) != 0;

                theme.DefaultLineFormat = LineFormat.FromInvariantStrings(
                    (string)key.GetValue("LineStyle"),
                    (string)key.GetValue("LineColor"),
                    (int)key.GetValue("Visible", 1));

                foreach (var subkeyName in key.GetSubKeyNames())
                {
                    LineFormat format;
                    using (var subkey = key.OpenSubKey(subkeyName))
                    {
                        format = LineFormat.FromInvariantStrings(
                            (string)subkey.GetValue("LineStyle"),
                            (string)subkey.GetValue("LineColor"),
                            (int)subkey.GetValue("Visible", 1));
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

            theme.ContentType = (key == DefaultThemeName) ? null : key;

            reader.ReadSettingAttribute(key, "TopToBottom", out temp);
            theme.TopToBottom = bool.Parse(temp);
            reader.ReadSettingAttribute(key, "VisibleAligned", out temp);
            theme.VisibleAligned = bool.Parse(temp);
            reader.ReadSettingAttribute(key, "VisibleUnaligned", out temp);
            theme.VisibleUnaligned = bool.Parse(temp);
            reader.ReadSettingAttribute(key, "VisibleAtTextEnd", out temp);
            theme.VisibleAtTextEnd = bool.Parse(temp);
            reader.ReadSettingAttribute(key, "VisibleEmpty", out temp);
            theme.VisibleEmpty = bool.Parse(temp);
            reader.ReadSettingAttribute(key, "VisibleEmptyAtEnd", out temp);
            theme.VisibleEmptyAtEnd = bool.Parse(temp);

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
            using (var key = reg.CreateSubKey(ContentType ?? DefaultThemeName))
            {
                key.SetValue("TopToBottom", TopToBottom ? 1 : 0);
                key.SetValue("VisibleAligned", VisibleAligned ? 1 : 0);
                key.SetValue("VisibleUnaligned", VisibleUnaligned ? 1 : 0);
                key.SetValue("VisibleAtTextEnd", VisibleAtTextEnd ? 1 : 0);
                key.SetValue("VisibleEmpty", VisibleEmpty ? 1 : 0);
                key.SetValue("VisibleEmptyAtEnd", VisibleEmptyAtEnd ? 1 : 0);

                string lineStyle, lineColor;
                int visible;
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
            return ContentType;
        }

        public string Save(IVsSettingsWriter writer)
        {
            var key = ContentType ?? DefaultThemeName;
            writer.WriteSettingAttribute(key, "TopToBottom", TopToBottom.ToString());
            writer.WriteSettingAttribute(key, "VisibleAligned", VisibleAligned.ToString());
            writer.WriteSettingAttribute(key, "VisibleUnaligned", VisibleUnaligned.ToString());
            writer.WriteSettingAttribute(key, "VisibleAtTextEnd", VisibleAtTextEnd.ToString());
            writer.WriteSettingAttribute(key, "VisibleEmpty", VisibleEmpty.ToString());
            writer.WriteSettingAttribute(key, "VisibleEmptyAtEnd", VisibleEmptyAtEnd.ToString());

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
            reg.DeleteSubKeyTree(ContentType);
        }

        public int CompareTo(IndentTheme other)
        {
            if (null == other) return -1;
            if (IsDefault && other.IsDefault) return 0;
            if (IsDefault) return -1;
            if (other.IsDefault) return 1;
            return ContentType.CompareTo(other.ContentType);
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
