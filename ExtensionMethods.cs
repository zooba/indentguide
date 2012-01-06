using System;
using System.Collections.Generic;
using System.Linq;

namespace IndentGuide
{
    internal static class ExtensionMethods
    {
        public static int LeadingWhitespace(this string source, int indentSize)
        {
            int count = 0;
            foreach (var c in source)
            {
                if (c == ' ')
                    count += 1;
                else if (c == '\t')
                    count += indentSize - (count % indentSize);
                else
                    break;
            }
            return count;
        }

        public static System.Drawing.Color ToSDC(this System.Windows.Media.Color source)
        {
            return System.Drawing.Color.FromArgb(source.A, source.R, source.G, source.B);
        }

        public static System.Windows.Media.Color ToSWMC(this System.Drawing.Color source)
        {
            return System.Windows.Media.Color.FromArgb(source.A, source.R, source.G, source.B);
        }

        public static float[] ToFloatArray(this IEnumerable<double> source)
        {
            return source.Select(i => (float)i).ToArray();
        }
    }
}
