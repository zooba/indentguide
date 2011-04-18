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
        public static readonly EditorOptionKey<bool> OptionKey = new EditorOptionKey<bool>("IndentGuideVisibility");
        
        public override EditorOptionKey<bool> Key
        {
            get { return OptionKey; }
        }

        public override bool Default
        {
            get { return true; }
        }
    }
}
