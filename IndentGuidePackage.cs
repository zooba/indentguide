using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel.Design;

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

        protected override void Initialize()
        {
            Trace.WriteLine("Entering Initialise() of IndentGuide");
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the tool window
                CommandID toolwndCommandID = new CommandID(guidIndentGuideCmdSet, cmdidViewIndentGuides);
                MenuCommand menuViewIndentGuides = new MenuCommand(ToggleVisibility, toolwndCommandID);
                mcs.AddCommand(menuViewIndentGuides);
            }
        }

        private void ToggleVisibility(object sender, EventArgs e)
        { }
    }
}
