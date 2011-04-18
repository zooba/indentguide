using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace IndentGuide
{
    [Export(typeof(EditorOptionDefinition))]
    internal sealed class IndentGuideVisibilityOption : EditorOptionDefinition<bool>
    {
        public override EditorOptionKey<bool> Key
        {
            get { return new EditorOptionKey<bool>("IndentGuideVisibility"); }
        }

        public override bool Default
        {
            get { return true; }
        }
    }
}
