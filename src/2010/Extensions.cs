using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Threading;

namespace IndentGuide {
    static class Extensions {
        public static object GetValue(this PropertyInfo prop, object obj) {
            return prop.GetValue(obj, null);
        }

        public static void SetValue(this PropertyInfo prop, object obj, object value) {
            prop.SetValue(obj, value, null);
        }

        public static T GetCustomAttribute<T>(this ICustomAttributeProvider provider) {
            return provider.GetCustomAttributes(false).OfType<T>().FirstOrDefault();
        }

        public static void InvokeAsync(this Dispatcher dispatcher, Action action) {
            dispatcher.BeginInvoke(action);
        }
    }
}
