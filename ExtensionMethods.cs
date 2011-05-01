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
    }
}
