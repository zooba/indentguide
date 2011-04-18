using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.Windows.Media;

namespace IndentGuide
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Indent Guides")]
    [Name("Indent Guides")]
    [DisplayName("Indent Guides")]
    [UserVisible(true)]
    internal sealed class IndentGuideFormat : ClassificationFormatDefinition
    {
        internal const string Name = "Indent Guides";

        public IndentGuideFormat()
        {
            ForegroundColor = Colors.Teal;
        }
    }
}
