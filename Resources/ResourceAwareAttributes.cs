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
using System.Globalization;
using System.Resources;

namespace IndentGuide {
    internal static class ResourceLoader {
        private static ResourceManager ResourceManager;

        static ResourceLoader() {
            ResourceManager = new ResourceManager("IndentGuide.Resources.Strings", typeof(ResourceLoader).Assembly);
        }

        internal static string LoadString(string id, CultureInfo culture = null) {
            var str = ResourceManager.GetString(id, culture ?? CultureInfo.CurrentCulture);
            if (string.IsNullOrEmpty(str)) {
                Trace.TraceWarning("No resource found for {0}", id);
            }
            return str;
        }

        internal static string LoadString(string id, string fallback, CultureInfo culture = null) {
            try {
                return LoadString(id, culture);
            } catch {
                Trace.TraceWarning("No resource found for {0}", id);
                return fallback;
            }
        }
    }

    public class ResourceDescriptionAttribute : DescriptionAttribute {
        public ResourceDescriptionAttribute(string resourceId)
            : base(ResourceLoader.LoadString(resourceId)) { }
    }

    public class ResourceCategoryAttribute : CategoryAttribute {
        public ResourceCategoryAttribute(string resourceId)
            : base(ResourceLoader.LoadString(resourceId)) { }
    }

    public class ResourceDisplayNameAttribute : DisplayNameAttribute {
        public ResourceDisplayNameAttribute(string resourceId)
            : base(ResourceLoader.LoadString(resourceId)) { }
    }

    public class EnumResourceTypeConverter<T> : EnumConverter {
        private Dictionary<T, string> ToCurrentCulture;
        private Dictionary<string, T> FromCurrentCulture;

        public EnumResourceTypeConverter()
            : base(typeof(T)) {
            ToCurrentCulture = new Dictionary<T, string>();
            FromCurrentCulture = new Dictionary<string, T>();

            var prefix = typeof(T).Name + "_";
            foreach (var name in Enum.GetNames(typeof(T))) {
                var localized = ResourceLoader.LoadString(prefix + name, null, CultureInfo.CurrentCulture);
                var value = (T)Enum.Parse(typeof(T), name);
                ToCurrentCulture[value] = localized;
                FromCurrentCulture[localized] = value;
            }
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
            if (destinationType == typeof(string)) {
                if (culture.Equals(CultureInfo.CurrentCulture)) {
                    return ToCurrentCulture[(T)value];
                } else if (culture.Equals(CultureInfo.InvariantCulture)) {
                    return value.ToString();
                }
                var name = value.ToString();
                return ResourceLoader.LoadString(typeof(T).Name + "_" + name, name, culture);
            } else {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (value.GetType() == typeof(string)) {
                if (culture.Equals(CultureInfo.CurrentCulture)) {
                    return FromCurrentCulture[(string)value];
                } else if (culture.Equals(CultureInfo.InvariantCulture)) {
                    return (T)Enum.Parse(typeof(T), (string)value);
                }

                var prefix = typeof(T).Name + "_";
                foreach (var name in Enum.GetNames(typeof(T))) {
                    var localized = ResourceLoader.LoadString(prefix + name, null, culture);
                    if (localized.Equals(value)) return (T)Enum.Parse(typeof(T), name);
                }
                return default(T);
            } else {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
}
