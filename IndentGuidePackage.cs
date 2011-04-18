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
using Microsoft.VisualStudio.TextManager.Interop;

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

        private MenuCommand menuViewIndentGuides;
        private EnvDTE.WindowEvents WindowEvents;

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
                menuViewIndentGuides.Checked = true;
            }
            var dte = GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            if (dte != null)
            {
                WindowEvents = dte.Events.WindowEvents;
                WindowEvents.WindowActivated += new EnvDTE._dispWindowEvents_WindowActivatedEventHandler(WindowEvents_WindowActivated);
            }
        }

        void WindowEvents_WindowActivated(EnvDTE.Window GotFocus, EnvDTE.Window LostFocus)
        {
            // Update the current setting
            // BUG: Currently updates to the window losing focus

            var viewHost = GetActiveTextViewHost();
            if (viewHost == null) return;

            var options = viewHost.TextView.Options;
            menuViewIndentGuides.Checked = options.GetOptionValue(IndentGuideVisibilityOption.OptionKey);
        }

        private void ToggleVisibility(object sender, EventArgs e)
        {
            // Notify the current view

            var viewHost = GetActiveTextViewHost();
            if (viewHost == null) return;
                
            var options = viewHost.TextView.Options;
            var newSetting = !options.GetOptionValue(IndentGuideVisibilityOption.OptionKey);
            menuViewIndentGuides.Checked = newSetting;
            options.SetOptionValue(IndentGuideVisibilityOption.OptionKey, newSetting);
        }

        private IWpfTextViewHost GetActiveTextViewHost()
        {
            var txtMgr = GetService(typeof(SVsTextManager)) as IVsTextManager;
            if (txtMgr == null) return null;

            IVsTextView view = null;
            txtMgr.GetActiveView(1, null, out view);
            if (view == null) return null;

            var userData = view as IVsUserData;
            if (userData == null) return null;

            object oViewHost;
            var guidViewHost = Microsoft.VisualStudio.Editor.DefGuidList.guidIWpfTextViewHost;
            userData.GetData(ref guidViewHost, out oViewHost);
            
            return oViewHost as IWpfTextViewHost;
        }
    }
}
