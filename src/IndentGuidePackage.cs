/* ****************************************************************************
 * Copyright 2015 Steve Dower
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 * ***************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;

namespace IndentGuide {
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "16", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(DisplayOptions), "IndentGuide", "Display", 110, 120, false)]
    [ProvideOptionPage(typeof(BehaviorOptions), "IndentGuide", "Behavior\\QuickSet", 110, 130, false)]
    [ProvideOptionPage(typeof(CustomBehaviorOptions), "IndentGuide", "Behavior\\Custom", 110, 140, false)]
    [ProvideOptionPage(typeof(CaretOptions), "IndentGuide", "Highlighting", 110, 150, false)]
    [ProvideOptionPage(typeof(PageWidthOptions), "IndentGuide", "PageWidth", 110, 160, false)]
    [ProvideProfile(typeof(ProfileManager), "IndentGuide", "Styles", 110, 220, false, DescriptionResourceID = 230)]
    [ProvideService(typeof(SIndentGuide), IsAsyncQueryable = true)]
    [ResourceDescription("IndentGuidePackage")]
    [Guid(Guids.IndentGuidePackageGuid)]
    // auto-load the extension instead on on-demand: http://www.mztools.com/articles/2013/MZ2013027.aspx
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class IndentGuidePackage : AsyncPackage {
        private static readonly Guid guidIndentGuideCmdSet = Guid.Parse(Guids.IndentGuideCmdSetGuid);
        private const int cmdidViewIndentGuides = 0x0103;

        private EnvDTE.WindowEvents WindowEvents;
        private IndentGuideService Service;
        private bool CommandVisible;

        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            JoinableTaskFactory = ThreadHelper.JoinableTaskFactory;

            // Switches to the UI thread in order to consume some services used in command initialization
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // Prepare event
            var dte = await GetServiceAsync(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            if (dte != null)
            {
                CommandVisible = false;
                WindowEvents = dte.Events.WindowEvents;
                WindowEvents.WindowActivated += WindowEvents_WindowActivated;
                WindowEvents.WindowClosing += WindowEvents_WindowClosing;
            }

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (mcs != null)
            {
                // Create the command for the tool window
                CommandID viewIndentCommandID = new CommandID(guidIndentGuideCmdSet, cmdidViewIndentGuides);
                var menuCmd = new OleMenuCommand(ToggleVisibility, viewIndentCommandID);
                menuCmd.BeforeQueryStatus += new EventHandler(BeforeQueryStatus);

                mcs.AddCommand(menuCmd);
            }

            // Adds a service on the background thread
            AddService(typeof(IndentGuideService), CreateIndentGuideServiceAsync);
        }

        private async System.Threading.Tasks.Task<object> CreateIndentGuideServiceAsync(IAsyncServiceContainer container, CancellationToken cancellationToken, Type serviceType)
        {
            Service = new IndentGuideService(this);
            await System.Threading.Tasks.Task.Run(() =>
           {
               Service.Upgrade();
               Service.Load();
           });
            return Service;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                Service.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Saves the current settings.
        /// </summary>
        void DTEEvents_OnBeginShutdown() {
            if (Service != null) {
                Service.Save();
            }
        }

        void WindowEvents_WindowActivated(EnvDTE.Window GotFocus, EnvDTE.Window LostFocus) {
            CommandVisible = (GotFocus != null && GotFocus.Kind == "Document");
        }

        void WindowEvents_WindowClosing(EnvDTE.Window Window) {
            if (Window.DTE.ActiveWindow == Window) {
                CommandVisible = false;
            }
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

        public new static JoinableTaskFactory JoinableTaskFactory { get; set; }

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
                Trace.TraceInformation("IndentGuideService.CURRENT_VERSION == {0:X}", version);
                return version;
            } catch (Exception ex) {
                Trace.TraceError("IndentGuide::GetCurrentVersion: {0}", ex);
                return DEFAULT_VERSION;
            }
        }
    }
}
