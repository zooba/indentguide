using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace IndentGuide
{
    internal static class ResourceLoader
    {
        private static System.Resources.ResourceManager ResourceManager;

        static ResourceLoader()
        {
            ResourceManager = IndentGuide.Strings.ResourceManager;
        }

        internal static string LoadString(string id, CultureInfo culture = null)
        {
            return ResourceManager.GetString(id, culture ?? CultureInfo.CurrentUICulture);
        }
        
        internal static string LoadString(string id, string fallback, CultureInfo culture = null)
        {
            try
            {
                return LoadString(id, culture);
            }
            catch
            {
                return fallback;
            }
        }
    }

    public class ResourceDescriptionAttribute : DescriptionAttribute
    {
        public ResourceDescriptionAttribute(string resourceId)
            : base(ResourceLoader.LoadString(resourceId))
        { }
    }
    
    public class ResourceCategoryAttribute : CategoryAttribute
    {
        public ResourceCategoryAttribute(string resourceId)
            : base(ResourceLoader.LoadString(resourceId))
        { }
    }

    public class ResourceDisplayNameAttribute : DisplayNameAttribute
    {
        public ResourceDisplayNameAttribute(string resourceId)
            : base(ResourceLoader.LoadString(resourceId))
        { }
    }

    public class EnumResourceTypeConverter<T> : EnumConverter
    {
        private Dictionary<T, string> ToCurrentCulture;
        private Dictionary<string, T> FromCurrentCulture;

        public EnumResourceTypeConverter()
            : base(typeof(T))
        {
            ToCurrentCulture = new Dictionary<T, string>();
            FromCurrentCulture = new Dictionary<string, T>();

            var prefix = typeof(T).Name + "_";
            foreach (var name in Enum.GetNames(typeof(T)))
            {
                var localized = ResourceLoader.LoadString(prefix + name, null, CultureInfo.CurrentCulture);
                var value = (T)Enum.Parse(typeof(T), name);
                ToCurrentCulture[value] = localized;
                FromCurrentCulture[localized] = value;
            }
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (culture.Equals(CultureInfo.CurrentCulture))
                {
                    return ToCurrentCulture[(T)value];
                }
                else if (culture.Equals(CultureInfo.InvariantCulture))
                {
                    return value.ToString();
                }
                var name = value.ToString();
                return ResourceLoader.LoadString(typeof(T).Name + "_" + name, name, culture);
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                if (culture.Equals(CultureInfo.CurrentCulture))
                {
                    return FromCurrentCulture[(string)value];
                }
                else if (culture.Equals(CultureInfo.InvariantCulture))
                {
                    return (T)Enum.Parse(typeof(T), (string)value);
                }

                var prefix = typeof(T).Name + "_";
                foreach (var name in Enum.GetNames(typeof(T)))
                {
                    var localized = ResourceLoader.LoadString(prefix + name, null, culture);
                    if (localized.Equals(value)) return (T)Enum.Parse(typeof(T), name);
                }
                return default(T);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
}
