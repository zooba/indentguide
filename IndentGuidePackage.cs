using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace IndentGuide
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "7.0", IconResourceID = 400)]
    [ProvideMenuResource(1000, 1)]
    [Description("Indent Guides Package")]
    [Guid("959BEB25-6C38-440A-A37F-5D6717E9A41B")]
    public sealed class IndentGuidePackage : Package
    {
        public IndentGuidePackage() { }

        private static readonly Guid guidIndentGuideCmdSet = Guid.Parse("1AE9DCDB-7723-4651-ABDC-3D4BBAA0731F");
        private const int cmdidViewIndentGuides = 0x0102;

        [Import(typeof(IEditorOptionsFactoryService))]
        internal IEditorOptionsFactoryService EditorOptionsFactory = null;
        
        private MenuCommand menuViewIndentGuides;

        protected override void Initialize()
        {
            Trace.WriteLine("Entering Initialise() of IndentGuide");
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (mcs != null)
            {
                // Create the command for the tool window
                CommandID toolwndCommandID = new CommandID(guidIndentGuideCmdSet, cmdidViewIndentGuides);
                menuViewIndentGuides = new MenuCommand(ToggleVisibility, toolwndCommandID);
                mcs.AddCommand(menuViewIndentGuides);
            }
            if (EditorOptionsFactory != null)
            {
                var options = EditorOptionsFactory.GlobalOptions;

                menuViewIndentGuides.Checked = options.GetOptionValue<bool>("IndentGuideVisibility");
            }
        }

        private void ToggleVisibility(object sender, EventArgs e)
        {
            if (EditorOptionsFactory != null)
            {
                var options = EditorOptionsFactory.GlobalOptions;
                
                options.SetOptionValue("IndentGuideVisibility", !options.GetOptionValue<bool>("IndentGuideVisibility"));
                menuViewIndentGuides.Checked = options.GetOptionValue<bool>("IndentGuideVisibility");
            }
        }
    }
}
