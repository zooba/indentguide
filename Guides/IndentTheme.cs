using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace IndentGuide {
    /// <summary>
    /// The supported styles of guideline.
    /// </summary>
    [Flags]
    public enum LineStyle {
        Solid = 1,
        Thick = 2,
        Glow = 4,
        Dotted = 0x100,
        DottedThick = Dotted | Thick,
        DottedGlow = Dotted | Glow,
        Dashed = 0x200,
        DashedThick = Dashed | Thick,
        DashedGlow = Dashed | Glow
    }

    public static class LineStyleExtensions {
        public static double GetStrokeThickness(this LineStyle style) {
            if (style.HasFlag(LineStyle.Thick))
                return 3.0;
            else
                return 1.0;
        }

        public static System.Windows.Media.DoubleCollection GetStrokeDashArray(this LineStyle style) {
            if (style.HasFlag(LineStyle.Dotted))
                return new System.Windows.Media.DoubleCollection { 1.0, 2.0, 1.0, 2.0 };
            else if (style.HasFlag(LineStyle.Dashed))
                return new System.Windows.Media.DoubleCollection { 3.0, 3.0, 3.0, 3.0 };
            else
                return null;
        }

        public static float[] GetDashPattern(this LineStyle style) {
            var dashArray = style.GetStrokeDashArray();
            if (dashArray == null)
                return null;
            else
                return dashArray.Select(i => (float)i).ToArray();
        }
    }

    /// <summary>
    /// The format of a particular type of guideline.
    /// </summary>
    public class LineFormat : IEquatable<LineFormat> {
        public LineFormat() {
            Reset();
        }

        public LineFormat Clone() {
            return new LineFormat {
                Visible = Visible,
                LineStyle = LineStyle,
                LineColor = LineColor,
                HighlightStyle = HighlightStyle,
                HighlightColor = HighlightColor
            };
        }

        public void Reset() {
            Visible = true;
            LineStyle = LineStyle.Dotted;
            LineColor = Color.DimGray;
            HighlightStyle = LineStyle.DottedGlow;
            HighlightColor = Color.Cyan;
        }

        [ResourceDescription("VisibilityDescription")]
        [ResourceCategory("Appearance")]
        public bool Visible { get; set; }

        class LineStyleConverter : EnumResourceTypeConverter<LineStyle> { }

        [ResourceDisplayName("LineStyleDisplayName")]
        [ResourceDescription("LineStyleDescription")]
        [ResourceCategory("Appearance")]
        [TypeConverter(typeof(LineStyleConverter))]
        public LineStyle LineStyle { get; set; }

        [ResourceDisplayName("LineColorDisplayName")]
        [ResourceDescription("LineColorDescription")]
        [ResourceCategory("Appearance")]
        [TypeConverter(typeof(ColorConverter))]
        public Color LineColor { get; set; }

        [ResourceDisplayName("HighlightStyleDisplayName")]
        [ResourceDescription("HighlightStyleDescription")]
        [ResourceCategory("Appearance")]
        [TypeConverter(typeof(LineStyleConverter))]
        public LineStyle HighlightStyle { get; set; }

        [ResourceDisplayName("HighlightColorDisplayName")]
        [ResourceDescription("HighlightColorDescription")]
        [ResourceCategory("Appearance")]
        [TypeConverter(typeof(ColorConverter))]
        public Color HighlightColor { get; set; }

        public static LineFormat FromInvariantStrings(string lineStyle, string lineColor, string highlightStyle, string highlightColor, int visible) {
            var inst = new LineFormat();
            try {
                inst.LineStyle = (LineStyle)TypeDescriptor.GetConverter(typeof(LineStyle))
                    .ConvertFromInvariantString(lineStyle);
            } catch (Exception ex) {
                Trace.WriteLine("IndentGuide::Error parsing " + lineStyle.ToString() + " into LineStyle");
                Trace.WriteLine(" - Exception: " + ex.ToString());
            }

            try {
                inst.LineColor = (Color)TypeDescriptor.GetConverter(typeof(Color))
                    .ConvertFromInvariantString(lineColor);
            } catch (Exception ex) {
                Trace.WriteLine("IndentGuide::Error parsing " + lineColor.ToString() + " into LineColor");
                Trace.WriteLine(" - Exception: " + ex.ToString());
            }

            try {
                inst.HighlightStyle = (LineStyle)TypeDescriptor.GetConverter(typeof(LineStyle))
                    .ConvertFromInvariantString(highlightStyle ?? lineStyle);
            } catch (Exception ex) {
                Trace.WriteLine("IndentGuide::Error parsing " + highlightStyle.ToString() + " into HighlightStyle");
                Trace.WriteLine(" - Exception: " + ex.ToString());
            }

            try {
                inst.HighlightColor = (Color)TypeDescriptor.GetConverter(typeof(Color))
                    .ConvertFromInvariantString(highlightColor ?? lineColor);
            } catch (Exception ex) {
                Trace.WriteLine("IndentGuide::Error parsing " + highlightColor.ToString() + " into HighlightColor");
                Trace.WriteLine(" - Exception: " + ex.ToString());
            }

            try {
                inst.Visible = visible != 0;
            } catch (Exception ex) {
                Trace.WriteLine("IndentGuide::Error parsing " + visible.ToString() + " into Visible");
                Trace.WriteLine(" - Exception: " + ex.ToString());
            }
            return inst;
        }

        public void ToInvariantStrings(out string lineStyle, out string lineColor, out string highlightStyle, out string highlightColor, out int visible) {
            try {
                lineStyle = TypeDescriptor.GetConverter(typeof(LineStyle))
                    .ConvertToInvariantString(LineStyle);
            } catch (Exception ex) {
                Trace.WriteLine("IndentGuide::Error converting LineStyle into string");
                Trace.WriteLine(" - Exception: " + ex.ToString());
                lineStyle = LineStyle.Dotted.ToString();
            }

            try {
                lineColor = TypeDescriptor.GetConverter(typeof(Color))
                    .ConvertToInvariantString(LineColor);
            } catch (Exception ex) {
                Trace.WriteLine("IndentGuide::Error converting LineColor into string");
                Trace.WriteLine(" - Exception: " + ex.ToString());
                lineColor = Color.Teal.ToString();
            }

            try {
                highlightStyle = TypeDescriptor.GetConverter(typeof(LineStyle))
                    .ConvertToInvariantString(HighlightStyle);
            } catch (Exception ex) {
                Trace.WriteLine("IndentGuide::Error converting LineStyle into string");
                Trace.WriteLine(" - Exception: " + ex.ToString());
                highlightStyle = LineStyle.Dotted.ToString();
            }

            try {
                highlightColor = TypeDescriptor.GetConverter(typeof(Color))
                    .ConvertToInvariantString(HighlightColor);
            } catch (Exception ex) {
                Trace.WriteLine("IndentGuide::Error converting LineColor into string");
                Trace.WriteLine(" - Exception: " + ex.ToString());
                highlightColor = Color.Red.ToString();
            }

            try {
                visible = Visible ? 1 : 0;
            } catch (Exception ex) {
                Trace.WriteLine("IndentGuide::Error converting Visible into string");
                Trace.WriteLine(" - Exception: " + ex.ToString());
                visible = 1;
            }
        }

        public static LineFormat FromInvariantStrings(string lineStyle, string lineColor, string highlightStyle, string highlightColor, string visible) {
            int visibleInt = 1;
            try {
                visibleInt = bool.Parse(visible) ? 1 : 0;
            } catch (Exception ex) {
                Trace.WriteLine("IndentGuide::Error converting Visible into bool");
                Trace.WriteLine(" - Exception: " + ex.ToString());
            }
            return FromInvariantStrings(lineStyle, lineColor, highlightStyle, highlightColor, visibleInt);
        }

        public void ToInvariantStrings(out string lineStyle, out string lineColor, out string highlightStyle, out string highlightColor, out string visible) {
            int visibleInt;
            ToInvariantStrings(out lineStyle, out lineColor, out highlightStyle, out highlightColor, out visibleInt);
            visible = (visibleInt != 0).ToString();
        }

        #region IEquatable<LineFormat> Members

        public bool Equals(LineFormat other) {
            if (null == other) return false;
            return LineStyle.Equals(other.LineStyle) &&
                LineColor.Equals(other.LineColor) &&
                HighlightStyle.Equals(other.HighlightStyle) &&
                HighlightColor.Equals(other.HighlightColor) &&
                Visible.Equals(other.Visible);
        }

        #endregion
    }

    public class LineBehavior : IEquatable<LineBehavior> {
        public LineBehavior() {
            ExtendInwardsOnly = true;
            VisibleAligned = true;
            VisibleUnaligned = false;
            VisibleAtTextEnd = false;
            VisibleEmpty = true;
            VisibleEmptyAtEnd = true;
        }

        public LineBehavior Clone() {
            return new LineBehavior {
                ExtendInwardsOnly = ExtendInwardsOnly,
                VisibleAligned = VisibleAligned,
                VisibleUnaligned = VisibleUnaligned,
                VisibleAtTextEnd = VisibleAtTextEnd,
                VisibleEmpty = VisibleEmpty,
                VisibleEmptyAtEnd = VisibleEmptyAtEnd,
            };
        }

        public override bool Equals(object obj) {
            return Equals(obj as LineBehavior);
        }

        public bool Equals(LineBehavior other) {
            return other != null &&
                ExtendInwardsOnly == other.ExtendInwardsOnly &&
                VisibleAligned == other.VisibleAligned &&
                VisibleAtTextEnd == other.VisibleAtTextEnd &&
                VisibleEmpty == other.VisibleEmpty &&
                VisibleEmptyAtEnd == other.VisibleEmptyAtEnd &&
                VisibleUnaligned == other.VisibleUnaligned;
        }

        public override int GetHashCode() {
            return (ExtendInwardsOnly ? 1 : 0) |
                (VisibleAligned ? 2 : 0) |
                (VisibleAtTextEnd ? 4 : 0) |
                (VisibleEmpty ? 8 : 0) |
                (VisibleEmptyAtEnd ? 16 : 0) |
                (VisibleUnaligned ? 32 : 0);
        }

        /// <summary>
        /// True to require guides to appear on both sides of empty lines.
        /// </summary>
        [ResourceDisplayName("ExtendInwardsOnlyDisplayName")]
        [ResourceDescription("ExtendInwardsOnlyDescription")]
        [SortOrder(4)]
        public bool ExtendInwardsOnly { get; set; }

        /// <summary>
        /// True to copy guidelines from the previous non-empty line into empty
        /// lines.
        /// </summary>
        [ResourceDisplayName("VisibleEmptyDisplayName")]
        [ResourceDescription("VisibleEmptyDescription")]
        [SortOrder(3)]
        public bool VisibleEmpty { get; set; }

        /// <summary>
        /// True to copy 'at end' guidelines from the previous non-empty line
        /// into empty lines.
        /// </summary>
        [ResourceDisplayName("VisibleEmptyAtEndDisplayName")]
        [ResourceDescription("VisibleEmptyAtEndDescription")]
        [SortOrder(6)]
        public bool VisibleEmptyAtEnd { get; set; }

        /// <summary>
        /// True to display guidelines at transitions from whitespace to text.
        /// </summary>
        [ResourceDisplayName("VisibleAtTextEndDisplayName")]
        [ResourceDescription("VisibleAtTextEndDescription")]
        [SortOrder(5)]
        public bool VisibleAtTextEnd { get; set; }

        /// <summary>
        /// True to always display guidelines at multiples of indent size.
        /// </summary>
        [ResourceDisplayName("VisibleAlignedDisplayName")]
        [ResourceDescription("VisibleAlignedDescription")]
        [SortOrder(1)]
        public bool VisibleAligned { get; set; }

        /// <summary>
        /// True to always display guidelines at textual indents.
        /// </summary>
        [ResourceDisplayName("VisibleUnalignedDisplayName")]
        [ResourceDescription("VisibleUnalignedDescription")]
        [SortOrder(2)]
        public bool VisibleUnaligned { get; set; }

        internal void Load(RegistryKey key) {
            ExtendInwardsOnly = (int)key.GetValue("ExtendInwardsOnly", 1) != 0;
            VisibleAligned = (int)key.GetValue("VisibleAligned", 1) != 0;
            VisibleUnaligned = (int)key.GetValue("VisibleUnaligned", 0) != 0;
            VisibleAtTextEnd = (int)key.GetValue("VisibleAtTextEnd", 0) != 0;
            VisibleEmpty = (int)key.GetValue("VisibleEmpty", 1) != 0;
            VisibleEmptyAtEnd = (int)key.GetValue("VisibleEmptyAtEnd", 1) != 0;
        }

        internal void Load(IVsSettingsReader reader, string key) {
            string temp;
            reader.ReadSettingAttribute(key, "ExtendInwardsOnly", out temp);
            ExtendInwardsOnly = bool.Parse(temp);
            reader.ReadSettingAttribute(key, "VisibleAligned", out temp);
            VisibleAligned = bool.Parse(temp);
            reader.ReadSettingAttribute(key, "VisibleUnaligned", out temp);
            VisibleUnaligned = bool.Parse(temp);
            reader.ReadSettingAttribute(key, "VisibleAtTextEnd", out temp);
            VisibleAtTextEnd = bool.Parse(temp);
            reader.ReadSettingAttribute(key, "VisibleEmpty", out temp);
            VisibleEmpty = bool.Parse(temp);
            reader.ReadSettingAttribute(key, "VisibleEmptyAtEnd", out temp);
            VisibleEmptyAtEnd = bool.Parse(temp);
        }

        internal void Save(RegistryKey key) {
            key.SetValue("ExtendInwardsOnly", ExtendInwardsOnly ? 1 : 0);
            key.SetValue("VisibleAligned", VisibleAligned ? 1 : 0);
            key.SetValue("VisibleUnaligned", VisibleUnaligned ? 1 : 0);
            key.SetValue("VisibleAtTextEnd", VisibleAtTextEnd ? 1 : 0);
            key.SetValue("VisibleEmpty", VisibleEmpty ? 1 : 0);
            key.SetValue("VisibleEmptyAtEnd", VisibleEmptyAtEnd ? 1 : 0);
        }

        internal void Save(IVsSettingsWriter writer, string key) {
            writer.WriteSettingAttribute(key, "ExtendInwardsOnly", ExtendInwardsOnly.ToString());
            writer.WriteSettingAttribute(key, "VisibleAligned", VisibleAligned.ToString());
            writer.WriteSettingAttribute(key, "VisibleUnaligned", VisibleUnaligned.ToString());
            writer.WriteSettingAttribute(key, "VisibleAtTextEnd", VisibleAtTextEnd.ToString());
            writer.WriteSettingAttribute(key, "VisibleEmpty", VisibleEmpty.ToString());
            writer.WriteSettingAttribute(key, "VisibleEmptyAtEnd", VisibleEmptyAtEnd.ToString());
        }
    }

    /// <summary>
    /// A theme for a particular language or document type.
    /// </summary>
    public class IndentTheme : IComparable<IndentTheme>, IEquatable<IndentTheme> {
        public static readonly string DefaultThemeName = ResourceLoader.LoadString("DefaultThemeName");
        public const int DefaultFormatIndex = int.MinValue;
        public const int UnalignedFormatIndex = -1;
        public const int CaretFormatIndex_Deprecated = -2;

        public event EventHandler Updated;

        internal void OnUpdated() {
            var evt = Updated;
            if (evt != null) Updated(this, EventArgs.Empty);
        }

        public IndentTheme() {
            ContentType = null;
            LineFormats = new Dictionary<int, LineFormat>();
            DefaultLineFormat = new LineFormat();
            Behavior = new LineBehavior();
        }

        public IndentTheme Clone() {
            var inst = new IndentTheme();
            inst.ContentType = ContentType;
            foreach (var item in LineFormats)
                inst.LineFormats[item.Key] = item.Value.Clone();
            inst.Behavior = Behavior.Clone();
            return inst;
        }

        public static string FormatIndexToString(int formatIndex) {
            if (formatIndex == DefaultFormatIndex)
                return "Default";
            else if (formatIndex == UnalignedFormatIndex)
                return "Unaligned";
            else if (formatIndex == CaretFormatIndex_Deprecated)
                throw new NotImplementedException("Caret format index has been removed");
            else
                return formatIndex.ToString(CultureInfo.InvariantCulture);
        }

        public static int? FormatIndexFromString(string source) {
            int result;
            if (source == "Default")
                return DefaultFormatIndex;
            else if (source == "Unaligned")
                return UnalignedFormatIndex;
            else if (source == "Caret")
                throw new NotImplementedException("Caret format index has been removed");
            else if (int.TryParse(source, out result))
                return result;
            else
                return null;
        }

        [ResourceDisplayName("ContentTypeDisplayName")]
        [ResourceDescription("ContentTypeDescription")]
        public string ContentType { get; set; }

        public bool IsDefault { get { return ContentType == null; } }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LineFormat DefaultLineFormat {
            get {
                LineFormat format;
                if (LineFormats.TryGetValue(DefaultFormatIndex, out format))
                    return format;

                return LineFormats[DefaultFormatIndex] = new LineFormat();
            }
            set { LineFormats[DefaultFormatIndex] = value; }
        }

        [Browsable(false)]
        public IDictionary<int, LineFormat> LineFormats { get; private set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LineBehavior Behavior { get; set; }

        public static IndentTheme Load(RegistryKey reg, string themeKey) {
            var theme = new IndentTheme();

            using (var key = reg.OpenSubKey(themeKey)) {
                theme.ContentType = (themeKey == DefaultThemeName) ? null : themeKey;
                theme.Behavior.Load(key);

                foreach (var subkeyName in key.GetSubKeyNames()) {
                    LineFormat format;
                    using (var subkey = key.OpenSubKey(subkeyName)) {
                        format = LineFormat.FromInvariantStrings(
                            (string)subkey.GetValue("LineStyle"),
                            (string)subkey.GetValue("LineColor"),
                            (string)subkey.GetValue("HighlightStyle"),
                            (string)subkey.GetValue("HighlightColor"),
                            (int)subkey.GetValue("Visible", 1));
                    }

                    int? i = FormatIndexFromString(subkeyName);
                    if (i.HasValue)
                        theme.LineFormats[i.Value] = format;
                    else
                        Trace.WriteLine(string.Format("IndentGuide::Unable to parse {0}", subkeyName));
                }
            }

            return theme;
        }

        public static IndentTheme Load(IVsSettingsReader reader, string key) {
            var theme = new IndentTheme();

            theme.ContentType = (key == DefaultThemeName) ? null : key;
            theme.Behavior.Load(reader, key);

            string lineStyle, lineColor, highlightStyle, highlightColor, visible;
            string subkeys;
            reader.ReadSettingString(key, out subkeys);
            if (!string.IsNullOrEmpty(subkeys)) {
                foreach (var subkey in subkeys.Split(';')) {
                    if (string.IsNullOrEmpty(subkeys)) continue;

                    int i = subkey.LastIndexOf('.');
                    if (i < 0) continue;

                    reader.ReadSettingAttribute(subkey, "LineStyle", out lineStyle);
                    reader.ReadSettingAttribute(subkey, "LineColor", out lineColor);
                    reader.ReadSettingAttribute(subkey, "HighlightStyle", out highlightStyle);
                    reader.ReadSettingAttribute(subkey, "HighlightColor", out highlightColor);
                    reader.ReadSettingAttribute(subkey, "Visible", out visible);
                    var format = LineFormat.FromInvariantStrings(lineStyle, lineColor, highlightStyle, highlightColor, visible);

                    var keypart = subkey.Substring(i + 1);
                    int? formatIndex = FormatIndexFromString(keypart);
                    if (formatIndex.HasValue)
                        theme.LineFormats[formatIndex.Value] = format;
                    else
                        Trace.WriteLine(string.Format("IndentGuide::Unable to parse {0}", keypart));
                }
            }

            return theme;
        }

        public string Save(RegistryKey reg) {
            using (var key = reg.CreateSubKey(ContentType ?? DefaultThemeName)) {
                Behavior.Save(key);

                foreach (var subkey in key.GetSubKeyNames()) {
                    key.DeleteSubKeyTree(subkey, false);
                }

                string lineStyle, lineColor, highlightStyle, highlightColor;
                int visible;

                foreach (var item in LineFormats) {
                    if (item.Value.Equals(DefaultLineFormat) && item.Key != DefaultFormatIndex) {
                        continue;
                    }
                    var subkeyName = FormatIndexToString(item.Key);

                    using (var subkey = key.CreateSubKey(subkeyName)) {
                        item.Value.ToInvariantStrings(out lineStyle, out lineColor, out highlightStyle, out highlightColor, out visible);
                        subkey.SetValue("LineStyle", lineStyle);
                        subkey.SetValue("LineColor", lineColor);
                        subkey.SetValue("HighlightStyle", highlightStyle);
                        subkey.SetValue("HighlightColor", highlightColor);
                        subkey.SetValue("Visible", visible);
                    }
                }
            }
            return ContentType;
        }

        public string Save(IVsSettingsWriter writer) {
            var key = ContentType ?? DefaultThemeName;
            var subkeys = string.Join(";", LineFormats.Select(item => key + "." + FormatIndexToString(item.Key)));
            writer.WriteSettingString(key, subkeys);

            Behavior.Save(writer, key);

            string lineStyle, lineColor, highlightStyle, highlightColor, visible;
            foreach (var item in LineFormats) {
                var subkeyName = key + "." + FormatIndexToString(item.Key);

                writer.WriteSettingString(subkeyName, "");

                item.Value.ToInvariantStrings(out lineStyle, out lineColor, out highlightStyle, out highlightColor, out visible);
                writer.WriteSettingAttribute(subkeyName, "LineStyle", lineStyle);
                writer.WriteSettingAttribute(subkeyName, "LineColor", lineColor);
                writer.WriteSettingAttribute(subkeyName, "HighlightStyle", highlightStyle);
                writer.WriteSettingAttribute(subkeyName, "HighlightColor", highlightColor);
                writer.WriteSettingAttribute(subkeyName, "Visible", visible);
            }

            return key;
        }

        public void Delete(RegistryKey reg) {
            reg.DeleteSubKeyTree(ContentType);
        }

        public int CompareTo(IndentTheme other) {
            if (null == other) return -1;
            if (IsDefault && other.IsDefault) return 0;
            if (IsDefault) return -1;
            if (other.IsDefault) return 1;
            return String.Compare(ContentType, other.ContentType, StringComparison.Ordinal);
        }

        public override bool Equals(object obj) {
            return this.Equals(obj as IndentTheme);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public bool Equals(IndentTheme other) {
            if (other == null) return false;
            if (IsDefault && other.IsDefault) return true;
            return String.Equals(ContentType, other.ContentType, StringComparison.Ordinal);
        }

        internal void Apply() {
            var toRemove = LineFormats
                .Where(kv => kv.Key != DefaultFormatIndex)
                .Where(kv => DefaultLineFormat.Equals(kv.Value) || null == kv.Value)
                .Select(kv => kv.Key)
                .ToList();

            foreach (var key in toRemove)
                LineFormats.Remove(key);
        }
    }
}
