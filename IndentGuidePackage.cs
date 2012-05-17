using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace IndentGuide {
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "12 (beta 3)", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(DisplayOptions), "IndentGuide", "Display", 110, 120, false)]
    [ProvideOptionPage(typeof(BehaviorOptions), "IndentGuide", "Behavior\\QuickSet", 110, 130, false)]
    [ProvideOptionPage(typeof(CustomBehaviorOptions), "IndentGuide", "Behavior\\Custom", 110, 140, false)]
    [ProvideProfile(typeof(ProfileManager), "IndentGuide", "Styles", 110, 220, false,
        MigrationType = ProfileMigrationType.PassThrough, DescriptionResourceID = 230)]
    [ProvideService(typeof(SIndentGuide))]
    [ResourceDescription("IndentGuidePackage")]
    [Guid(Guids.IndentGuidePackageGuid)]
    public sealed class IndentGuidePackage : Package {
        public IndentGuidePackage() {
            var container = (IServiceContainer)this;
            var callback = new ServiceCreatorCallback(CreateService);
            container.AddService(typeof(SIndentGuide), callback, true);
        }

        private object CreateService(IServiceContainer container, Type serviceType) {
            if (typeof(SIndentGuide) == serviceType)
                return new IndentGuideService(this);
            else
                return null;
        }

        private static readonly Guid guidIndentGuideCmdSet = Guid.Parse(Guids.IndentGuideCmdSetGuid);
        private const int cmdidViewIndentGuides2010 = 0x0102;
        private const int cmdidViewIndentGuides11 = 0x0103;
        private int cmdidViewIndentGuides = cmdidViewIndentGuides2010;

        private EnvDTE.WindowEvents WindowEvents;
        private IIndentGuide Service;
        private bool CommandVisible;

        protected override void Initialize() {
            base.Initialize();

            // Prepare event
            var dte = GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            if (dte != null) {
                if (dte.Version.StartsWith("11.")) {
                    cmdidViewIndentGuides = cmdidViewIndentGuides11;
                }

                CommandVisible = false;
                WindowEvents = dte.Events.WindowEvents;
                WindowEvents.WindowActivated +=
                    new EnvDTE._dispWindowEvents_WindowActivatedEventHandler(WindowEvents_WindowActivated);
            }

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (mcs != null) {
                // Create the command for the tool window
                CommandID viewIndentCommandID = new CommandID(guidIndentGuideCmdSet, cmdidViewIndentGuides);
                var menuCmd = new OleMenuCommand(ToggleVisibility, viewIndentCommandID);
                menuCmd.BeforeQueryStatus += new EventHandler(BeforeQueryStatus);

                mcs.AddCommand(menuCmd);
            }

            // Assume the service exists, otherwise, crash the extension.
            Service = (IIndentGuide)GetService(typeof(SIndentGuide));
        }

        /// <summary>
        /// Saves the current settings.
        /// </summary>
        void DTEEvents_OnBeginShutdown() {
            if (Service != null)
                Service.Save();
        }

        void WindowEvents_WindowActivated(EnvDTE.Window GotFocus, EnvDTE.Window LostFocus) {
            CommandVisible = (GotFocus != null && GotFocus.Kind == "Document");
        }

        void BeforeQueryStatus(object sender, EventArgs e) {
            var item = (OleMenuCommand)sender;

            item.Enabled = true;
            item.Checked = Service.Visible;
            item.Visible = CommandVisible;
        }

        private void ToggleVisibility(object sender, EventArgs e) {
            Service.Visible = !Service.Visible;
        }

        // Default version is 10.9.0.0, also known as 11 (beta 1).
        // This was the version prior to the version field being added.
        public const int DEFAULT_VERSION = 0x000A0900;

        private static readonly int CURRENT_VERSION = GetCurrentVersion();

        public static int Version { get { return CURRENT_VERSION; } }

        private static int GetCurrentVersion() {
            var assembly = typeof(IndentGuideService).Assembly;
            var attribs = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyFileVersionAttribute), false);
            if (attribs.Length == 0) return DEFAULT_VERSION;

            var attrib = (System.Reflection.AssemblyFileVersionAttribute)attribs[0];

            try {
                int version = attrib.Version.Split('.')
                    .Select(p => int.Parse(p))
                    .Take(3)
                    .Aggregate(0, (acc, i) => acc << 8 | i);
#if DEBUG
                Trace.WriteLine(string.Format("IndentGuideService.CURRENT_VERSION == {0:X}", version));
#endif
                return version;
            } catch (Exception ex) {
                Trace.WriteLine(string.Format("IndentGuide::GetCurrentVersion: {0}", ex));
                return DEFAULT_VERSION;
            }
        }
    }
}
