using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.Text.Editor;

namespace IndentGuide {
    internal static class ExtensionMethods {
        public static int LeadingWhitespace(this string source, int tabSize) {
            int count = 0;
            foreach (var c in source) {
                if (c == ' ')
                    count += 1;
                else if (c == '\t')
                    count += tabSize - (count % tabSize);
                else
                    break;
            }
            return count;
        }

        public static System.Drawing.Color ToSDC(this System.Windows.Media.Color source) {
            return System.Drawing.Color.FromArgb(source.A, source.R, source.G, source.B);
        }

        public static System.Windows.Media.Color ToSWMC(this System.Drawing.Color source) {
            return System.Windows.Media.Color.FromArgb(source.A, source.R, source.G, source.B);
        }

        public static System.Drawing.Color AsInverted(this System.Drawing.Color source) {
            return System.Drawing.Color.FromArgb(source.A, 255 - source.R, 255 - source.G, 255 - source.B);
        }

        public static float[] ToFloatArray(this IEnumerable<double> source) {
            return source.Select(i => (float)i).ToArray();
        }

        public static void AddAdornment(this IAdornmentLayer layer, LineSpan lineSpan) {
            UIElement guide;
            if (lineSpan != null && (guide = lineSpan.Adornment as System.Windows.Shapes.Line) != null) {
                layer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, lineSpan, guide, null);
            }
        }
    }
}
