using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text;

namespace IndentGuide
{
    internal static class ExtensionMethods
    {
        public static bool IsEmpty(this IWpfTextViewLine line)
        {
            if (line.Length == 0) return true;

            var span = new SnapshotSpan(line.Start, line.End);
            var text = span.Snapshot.GetText(span.Span);

            return string.IsNullOrWhiteSpace(text);
        }


        public static System.Drawing.Color ToSDC(this System.Windows.Media.Color source)
        {
            return System.Drawing.Color.FromArgb(source.A, source.R, source.G, source.B);
        }

        public static System.Windows.Media.Color ToSWMC(this System.Drawing.Color source)
        {
            return System.Windows.Media.Color.FromArgb(source.A, source.R, source.G, source.B);
        }
    }
}
