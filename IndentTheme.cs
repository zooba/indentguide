using System;
using System.ComponentModel;
using System.Drawing;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

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
    public class LineFormat
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
            LineFormat = new LineFormat();
            EmptyLineMode = IndentGuide.EmptyLineMode.SameAsLineAboveLogical;
            RegistryName = isDefaultTheme ? Guid.Empty : Guid.NewGuid();
        }

        public IndentTheme Clone(bool makeNonDefault = false)
        {
            var inst = new IndentTheme(!makeNonDefault && IsDefault);
            inst.Name = Name;
            inst.LineFormat = LineFormat.Clone();
            inst.EmptyLineMode = EmptyLineMode;
            if (!makeNonDefault) inst.RegistryName = RegistryName;
            return inst;
        }

        [ResourceDisplayName("ThemeNameDisplayName")]
        [ResourceDescription("ThemeNameDescription")]
        public string Name { get; set; }

        private Guid RegistryName;
        public bool IsDefault { get { return RegistryName.Equals(Guid.Empty); } }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LineFormat LineFormat { get; private set; }

        private class EmptyLineModeTypeConverter : EnumResourceTypeConverter<EmptyLineMode>
        { }

        [ResourceDisplayName("EmptyLineModeDisplayName")]
        [ResourceDescription("EmptyLineModeDescription")]
        [TypeConverter(typeof(EmptyLineModeTypeConverter))]
        public EmptyLineMode EmptyLineMode { get; set; }

        public static IndentTheme Load(RegistryKey reg, string subkey)
        {
            var theme = new IndentTheme();
            
            using (var key = reg.OpenSubKey(subkey))
            {
                theme.RegistryName = Guid.Parse(subkey);
                theme.Name = (string)key.GetValue("Name", subkey);
                theme.EmptyLineMode = (EmptyLineMode)TypeDescriptor.GetConverter(typeof(EmptyLineMode))
                    .ConvertFromInvariantString((string)key.GetValue("EmptyLineMode"));

                theme.LineFormat.LineColor = (Color)TypeDescriptor.GetConverter(typeof(Color))
                    .ConvertFromInvariantString((string)key.GetValue("LineColor"));
                theme.LineFormat.LineStyle = (LineStyle)TypeDescriptor.GetConverter(typeof(LineStyle))
                    .ConvertFromInvariantString((string)key.GetValue("LineStyle"));
                theme.LineFormat.Visible = bool.Parse((string)key.GetValue("Visible"));
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

            reader.ReadSettingAttribute(key, "LineColor", out temp);
            theme.LineFormat.LineColor = (Color)TypeDescriptor.GetConverter(typeof(Color))
                .ConvertFromInvariantString(temp);
            reader.ReadSettingAttribute(key, "LineStyle", out temp);
            theme.LineFormat.LineStyle = (LineStyle)TypeDescriptor.GetConverter(typeof(LineStyle))
                .ConvertFromInvariantString(temp);
            reader.ReadSettingAttribute(key, "Visible", out temp);
            theme.LineFormat.Visible = bool.Parse(temp);

            return theme;
        }

        public string Save(RegistryKey reg)
        {
            using (var key = reg.CreateSubKey(RegistryName.ToString("B")))
            {
                key.SetValue("Name", Name);
                key.SetValue("EmptyLineMode", TypeDescriptor.GetConverter(typeof(EmptyLineMode))
                    .ConvertToInvariantString(EmptyLineMode));

                key.SetValue("LineColor", TypeDescriptor.GetConverter(typeof(Color))
                    .ConvertToInvariantString(LineFormat.LineColor));
                key.SetValue("LineStyle", TypeDescriptor.GetConverter(typeof(LineStyle))
                    .ConvertToInvariantString(LineFormat.LineStyle));
                key.SetValue("Visible", LineFormat.Visible.ToString());
            }
            return RegistryName.ToString("B");
        }

        public string Save(IVsSettingsWriter writer)
        {
            var key = RegistryName.ToString();
            writer.WriteSettingString(key, Name);
            writer.WriteSettingAttribute(key, "EmptyLineMode", TypeDescriptor.GetConverter(typeof(EmptyLineMode))
                .ConvertToInvariantString(EmptyLineMode));

            writer.WriteSettingAttribute(key, "LineColor", TypeDescriptor.GetConverter(typeof(Color))
                .ConvertToInvariantString(LineFormat.LineColor));
            writer.WriteSettingAttribute(key, "LineStyle", TypeDescriptor.GetConverter(typeof(LineStyle))
                .ConvertToInvariantString(LineFormat.LineStyle));
            writer.WriteSettingAttribute(key, "Visible", LineFormat.Visible.ToString());

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

    }
}
