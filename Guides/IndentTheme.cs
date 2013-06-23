/* ****************************************************************************
 * Copyright 2012 Steve Dower
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio;
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
    public class LineFormat {
        static readonly LineFormat DefaultLineFormat = new LineFormat();
        protected virtual LineFormat Default {
            get { return DefaultLineFormat; }
        }

        protected LineFormat() {
            Theme = null;
            FormatIndex = DefaultFormatIndex;
            Visible = true;
            LineStyle = LineStyle.Dotted;
            LineColor = Color.DimGray;
            HighlightStyle = LineStyle.DottedGlow;
            HighlightColor = Color.DodgerBlue;
        }

        public LineFormat(IndentTheme theme)
            : this() {
            Theme = theme;
        }

        internal IndentTheme Theme { get; set; }

        internal LineFormat BaseFormat {
            get {
                return (Theme != null ? Theme.DefaultLineFormat : null) ?? Default;
            }
        }

        public virtual LineFormat Clone(IndentTheme theme) {
            return new LineFormat(theme) {
                FormatIndex = FormatIndex,
                Visible = Visible,
                LineStyle = LineStyle,
                LineColor = LineColor,
                HighlightStyle = HighlightStyle,
                HighlightColor = HighlightColor
            };
        }

        internal virtual bool ShouldSerialize() {
            return Theme == null ||
                (FormatIndex ?? DefaultFormatIndex) == DefaultFormatIndex ||
                ShouldSerializeVisible() ||
                ShouldSerializeLineStyle() ||
                ShouldSerializeLineColor() ||
                ShouldSerializeHighlightStyle() ||
                ShouldSerializeHighlightColor();
        }

        internal virtual void Reset() {
            ResetVisible();
            ResetLineStyle();
            ResetLineColor();
            ResetHighlightStyle();
            ResetHighlightColor();
        }

        public const int DefaultFormatIndex = int.MinValue;
        public const int UnalignedFormatIndex = -1;

        public const int FirstIndentFormatIndex = UnalignedFormatIndex, LastIndentFormatIndex = 999;

        [Browsable(false)]
        public int? FormatIndex { get; set; }

        [Browsable(false)]
        public string FormatIndexName {
            get {
                if (!FormatIndex.HasValue) {
                    return null;
                } else if (FormatIndex == DefaultFormatIndex) {
                    return "Default";
                } else if (FormatIndex == UnalignedFormatIndex) {
                    return "Unaligned";
                } else {
                    return FormatIndex.GetValueOrDefault().ToString(CultureInfo.InvariantCulture);
                }
            }
            set {
                int result;
                if (value == "Default") {
                    FormatIndex = DefaultFormatIndex;
                } else if (value == "Unaligned") {
                    FormatIndex = UnalignedFormatIndex;
                } else if (int.TryParse(value, out result)) {
                    FormatIndex = result;
                } else {
                    FormatIndex = null;
                }
            }
        }

        [ResourceDescription("VisibilityDescription")]
        [ResourceCategory("Appearance")]
        public bool Visible { get; set; }

        bool ShouldSerializeVisible() {
            return Visible != BaseFormat.Visible;
        }

        void ResetVisible() {
            Visible = BaseFormat.Visible;
        }

        class LineStyleConverter : EnumResourceTypeConverter<LineStyle> { }

        [ResourceDisplayName("LineStyleDisplayName")]
        [ResourceDescription("LineStyleDescription")]
        [ResourceCategory("Appearance")]
        [TypeConverter(typeof(LineStyleConverter))]
        public LineStyle LineStyle { get; set; }

        bool ShouldSerializeLineStyle() {
            return LineStyle != BaseFormat.LineStyle;
        }

        void ResetLineStyle() {
            LineStyle = BaseFormat.LineStyle;
        }

        [ResourceDisplayName("LineColorDisplayName")]
        [ResourceDescription("LineColorDescription")]
        [ResourceCategory("Appearance")]
        [TypeConverter(typeof(ColorConverter))]
        public Color LineColor { get; set; }

        bool ShouldSerializeLineColor() {
            return LineColor != BaseFormat.LineColor;
        }

        void ResetLineColor() {
            LineColor = BaseFormat.LineColor;
        }

        [ResourceDisplayName("HighlightStyleDisplayName")]
        [ResourceDescription("HighlightStyleDescription")]
        [ResourceCategory("Appearance")]
        [TypeConverter(typeof(LineStyleConverter))]
        public LineStyle HighlightStyle { get; set; }

        bool ShouldSerializeHighlightStyle() {
            return HighlightStyle != BaseFormat.HighlightStyle;
        }

        void ResetHighlightStyle() {
            HighlightStyle = BaseFormat.HighlightStyle;
        }

        [ResourceDisplayName("HighlightColorDisplayName")]
        [ResourceDescription("HighlightColorDescription")]
        [ResourceCategory("Appearance")]
        [TypeConverter(typeof(ColorConverter))]
        public Color HighlightColor { get; set; }

        bool ShouldSerializeHighlightColor() {
            return HighlightColor != BaseFormat.HighlightColor;
        }

        void ResetHighlightColor() {
            HighlightColor = BaseFormat.HighlightColor;
        }

        public static LineFormat FromInvariantStrings(IndentTheme theme, Dictionary<string, string> values) {
            var subclass = typeof(LineFormat);
            string subclassName;
            if (values.TryGetValue("TypeName", out subclassName)) {
                subclass = Type.GetType(subclassName, throwOnError: false) ?? typeof(LineFormat);
            }
            
            var inst = subclass.InvokeMember(null, BindingFlags.CreateInstance, null, null, new[] { theme }) as LineFormat;
            if (inst == null) {
                throw new InvalidOperationException("Unable to create instance of " + subclass.FullName);
            }

            foreach (var kv in values) {
                var prop = subclass.GetProperty(kv.Key);
                if (prop != null) {
                    try {
                        prop.SetValue(inst, TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(kv.Value));
                    } catch (Exception ex) {
                        Trace.TraceError("Error setting {0} to {1}:\n", kv.Key, kv.Value, ex);
                    }
                } else {
                    Trace.TraceWarning("Unable to find property {0} on type {1}", kv.Key, subclass.FullName);
                }
            }

            return inst;
        }

        public Dictionary<string, string> ToInvariantStrings() {
            var values = new Dictionary<string, string>();

            var subclass = this.GetType();
            if (subclass != typeof(LineFormat)) {
                if (subclass.Assembly == typeof(LineFormat).Assembly) {
                    values["TypeName"] = subclass.FullName;
                } else {
                    values["TypeName"] = subclass.AssemblyQualifiedName;
                }
            }

            foreach (var prop in subclass.GetProperties()) {
                var browsable = prop.GetCustomAttribute<BrowsableAttribute>();
                if (browsable == null || browsable.Browsable) {
                    values[prop.Name] = TypeDescriptor.GetConverter(prop.PropertyType).ConvertToInvariantString(prop.GetValue(this));
                }
            }

            return values;
        }
    }

    public class PageWidthMarkerFormat : LineFormat {
        static readonly LineFormat DefaultPageWidthMarkerFormat = new PageWidthMarkerFormat();
        protected override LineFormat Default {
            get { return DefaultPageWidthMarkerFormat; }
        }

        public const int FirstPageWidthIndex = 1000, LastPageWidthIndex = 1999;

        internal int GetFormatIndex() {
            return Position + FirstPageWidthIndex;
        }

        [ResourceDisplayName("PageWidthPositionDisplayName")]
        [ResourceDescription("PageWidthPositionDescription")]
        [ResourceCategory("Appearance")]
        public int Position { get; set; }

        bool ShouldSerializePosition() {
            return true;
        }

        void ResetPosition() {
            Position = 80;
        }

        protected PageWidthMarkerFormat()
            : base() {
            Position = 80;
        }

        public PageWidthMarkerFormat(IndentTheme theme)
            : this() {
            Theme = theme;
        }

        public override LineFormat Clone(IndentTheme theme) {
            return new PageWidthMarkerFormat(theme) {
                FormatIndex = FormatIndex,
                Visible = Visible,
                LineColor = LineColor,
                LineStyle = LineStyle,
                HighlightColor = HighlightColor,
                HighlightStyle = HighlightStyle,
                Position = Position,
            };
        }


        internal override bool ShouldSerialize() {
            return base.ShouldSerialize() || ShouldSerializePosition();
        }

        internal override void Reset() {
            base.Reset();
            ResetPosition();
        }
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
        public static readonly string DefaultCaretHandler = typeof(CaretNearestLeft).FullName;

        public event EventHandler Updated;

        internal void OnUpdated() {
            var evt = Updated;
            if (evt != null) Updated(this, EventArgs.Empty);
        }

        public IndentTheme() {
            ContentType = null;
            LineFormats = new Dictionary<int, LineFormat>();
            PageWidthMarkers = new PageWidthMarkerGetter(this);
            DefaultLineFormat = new LineFormat(this);
            Behavior = new LineBehavior();
            CaretHandler = DefaultCaretHandler;
        }

        public IndentTheme Clone() {
            var inst = new IndentTheme();
            inst.ContentType = ContentType;
            foreach (var item in LineFormats) {
                inst.LineFormats[item.Key] = item.Value.Clone(inst);
            }
            inst.Behavior = Behavior.Clone();
            inst.CaretHandler = CaretHandler;
            return inst;
        }

        [ResourceDisplayName("ContentTypeDisplayName")]
        [ResourceDescription("ContentTypeDescription")]
        public string ContentType { get; set; }

        public bool IsDefault { get { return ContentType == null; } }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LineFormat DefaultLineFormat {
            get {
                LineFormat format;
                if (LineFormats.TryGetValue(LineFormat.DefaultFormatIndex, out format)) {
                    return format;
                }

                return LineFormats[LineFormat.DefaultFormatIndex] = new LineFormat(this);
            }
            set { LineFormats[LineFormat.DefaultFormatIndex] = value; }
        }

        [Browsable(false)]
        public IDictionary<int, LineFormat> LineFormats { get; private set; }

        [Browsable(false)]
        public PageWidthMarkerGetter PageWidthMarkers { get; private set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LineBehavior Behavior { get; set; }

        [Browsable(false)]
        public string CaretHandler { get; set; }

        public static IndentTheme Load(RegistryKey reg, string themeKey) {
            var theme = new IndentTheme();

            using (var key = reg.OpenSubKey(themeKey)) {
                theme.ContentType = (themeKey == DefaultThemeName) ? null : themeKey;
                theme.Behavior.Load(key);
                theme.CaretHandler = (string)key.GetValue("CaretHandler") ?? DefaultCaretHandler;

                foreach (var subkeyName in key.GetSubKeyNames()) {
                    Dictionary<string, string> values;
                    using (var subkey = key.OpenSubKey(subkeyName)) {
                        values = subkey.GetValueNames().ToDictionary(name => name, name => (string)subkey.GetValue(name));
                    }

                    var format = LineFormat.FromInvariantStrings(theme, values);
                    format.FormatIndexName = subkeyName;
                    if (format.FormatIndex.HasValue) {
                        theme.LineFormats[format.FormatIndex.GetValueOrDefault()] = format;
                    } else {
                        Trace.TraceWarning("{0}.{1} is not a valid format index.", themeKey, subkeyName);
                    }
                }
            }

            return theme;
        }

        public static IndentTheme Load(IVsSettingsReader reader, string themeKey) {
            var theme = new IndentTheme();

            theme.ContentType = (themeKey == DefaultThemeName) ? null : themeKey;
            theme.Behavior.Load(reader, themeKey);
            string caretHandler;
            reader.ReadSettingString("CaretHandler", out caretHandler);
            theme.CaretHandler = caretHandler ?? DefaultCaretHandler;

            string subkeyNames, settingNames;
            ErrorHandler.ThrowOnFailure(reader.ReadSettingString(themeKey, out subkeyNames));
            if (!string.IsNullOrEmpty(subkeyNames)) {
                foreach (var subkeyName in subkeyNames.Split(';')) {
                    if (string.IsNullOrEmpty(subkeyName)) continue;

                    int i = subkeyName.LastIndexOf('.');
                    if (i < 0) continue;

                    var values = new Dictionary<string, string>();
                    if (ErrorHandler.Failed(reader.ReadSettingAttribute(subkeyName, "Keys", out settingNames)) ||
                        string.IsNullOrEmpty(settingNames)) {
                        settingNames = "LineStyle;LineColor;HighlightStyle;HighlightColor;Visible";
                    }
                    foreach (var setting in settingNames.Split(';')) {
                        if (string.IsNullOrEmpty(subkeyName)) continue;
                        string value;
                        ErrorHandler.ThrowOnFailure(reader.ReadSettingAttribute(subkeyName, setting, out value));
                        if (!string.IsNullOrEmpty(value)) {
                            values[setting] = value;
                        }
                    }
                    
                    var format = LineFormat.FromInvariantStrings(theme, values);
                    format.FormatIndexName = subkeyName.Substring(i + 1);
                    if (format.FormatIndex.HasValue) {
                        theme.LineFormats[format.FormatIndex.GetValueOrDefault()] = format;
                    } else {
                        Trace.TraceWarning("{0}.{1} is not a valid format index.", themeKey, subkeyName);
                    }
                }
            }

            return theme;
        }

        public string Save(RegistryKey reg) {
            using (var key = reg.CreateSubKey(ContentType ?? DefaultThemeName)) {
                Behavior.Save(key);
                key.SetValue("CaretHandler", CaretHandler ?? DefaultCaretHandler);

                foreach (var subkey in key.GetSubKeyNames()) {
                    key.DeleteSubKeyTree(subkey, false);
                }

                foreach (var item in LineFormats.Values.Where(item => item.ShouldSerialize())) {
                    using (var subkey = key.CreateSubKey(item.FormatIndexName)) {
                        foreach (var kv in item.ToInvariantStrings()) {
                            subkey.SetValue(kv.Key, kv.Value);
                        }
                    }
                }
            }
            return ContentType;
        }

        public string Save(IVsSettingsWriter writer) {
            var key = ContentType ?? DefaultThemeName;
            var subkeys = string.Join(";", LineFormats.Values
                .Where(item => item.ShouldSerialize())
                .Select(item => key + "." + item.FormatIndexName));
            writer.WriteSettingString(key, subkeys);

            Behavior.Save(writer, key);
            writer.WriteSettingString("CaretHandler", CaretHandler ?? DefaultCaretHandler);

            foreach (var item in LineFormats.Values.Where(item => item.ShouldSerialize())) {
                var subkeyName = key + "." + item.FormatIndexName;

                writer.WriteSettingString(subkeyName, "");

                var values = item.ToInvariantStrings();
                writer.WriteSettingAttribute(subkeyName, "Keys", string.Join(";", values.Keys));
                foreach (var kv in values) {
                    writer.WriteSettingAttribute(subkeyName, kv.Key, kv.Value);
                }
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
            return String.Compare(ContentType, other.ContentType, StringComparison.OrdinalIgnoreCase);
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
            return String.Equals(ContentType, other.ContentType, StringComparison.OrdinalIgnoreCase);
        }

        internal void Apply() {
            LineFormats = LineFormats.Values
                .Where(item => item != null && item.ShouldSerialize())
                .Where(item => item.FormatIndex.HasValue)
                .ToDictionary(item => item.FormatIndex.Value);
        }

        public class PageWidthMarkerGetter : IEnumerable<PageWidthMarkerFormat> {
            readonly IndentTheme Theme;

            internal PageWidthMarkerGetter(IndentTheme theme) {
                Theme = theme;
            }

            public PageWidthMarkerFormat this[int index] {
                get { return Theme.LineFormats[index + PageWidthMarkerFormat.FirstPageWidthIndex] as PageWidthMarkerFormat; }
            }

            public bool TryGetValue(int index, out LineFormat value) {
                return Theme.LineFormats.TryGetValue(index + PageWidthMarkerFormat.FirstPageWidthIndex, out value) &&
                    value is PageWidthMarkerFormat;
            }

            public IEnumerator<PageWidthMarkerFormat> GetEnumerator() {
                return Theme.LineFormats.Values.OfType<PageWidthMarkerFormat>().GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
                return Theme.LineFormats.Values.OfType<PageWidthMarkerFormat>().GetEnumerator();
            }
        }
    }
}
