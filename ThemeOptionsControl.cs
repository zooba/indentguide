using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace IndentGuide
{
    public partial class ThemeOptionsControl : UserControl
    {
        private readonly IndentGuideService Service;
        private readonly IVsTextManager TextManagerService;
        private readonly IVsEditorAdaptersFactoryService EditorAdapters;

        private static readonly List<IndentTheme> LocalThemes = new List<IndentTheme>();
        private static readonly List<IndentTheme> ChangedThemes = new List<IndentTheme>();
        private static readonly List<IndentTheme> DeletedThemes = new List<IndentTheme>();

        private readonly IThemeAwareDialog Child;

        public ThemeOptionsControl(IThemeAwareDialog child)
        {
            InitializeComponent();

            Child = child;
            var control = child as Control;
            Debug.Assert(Child != null);
            Debug.Assert(control != null);

            tableContent.Controls.Add(control);
            tableContent.SetColumn(control, 0);
            tableContent.SetRow(control, 1);

            Child.ThemeChanged += new EventHandler<ThemeEventArgs>(Child_ThemeChanged);
            
            var provider = ServiceProvider.GlobalProvider;
            Service = provider.GetService(typeof(SIndentGuide)) as IndentGuideService;
            Child.Service = (IIndentGuide)Service;

            TextManagerService = (IVsTextManager)provider.GetService(typeof(SVsTextManager));

            var componentModel = (IComponentModel)provider.GetService(typeof(SComponentModel));
            EditorAdapters = (IVsEditorAdaptersFactoryService)componentModel
                .GetService<IVsEditorAdaptersFactoryService>();
        }

        void Child_ThemeChanged(object sender, ThemeEventArgs e)
        {
            if (!ChangedThemes.Contains(e.Theme)) ChangedThemes.Add(e.Theme);
        }

        static int ActivationCount = 0;
        bool IsActive = false;
        internal void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
                if (Interlocked.Increment(ref ActivationCount) == 1)
                {
                    try
                    {
                        LocalThemes.Clear();
                        ChangedThemes.Clear();
                        DeletedThemes.Clear();

                        LocalThemes.Add(Service.DefaultTheme.Clone());
                        LocalThemes.AddRange(Service.Themes.Values.Select(t => t.Clone()));
                        UpdateThemeList();
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(string.Format("IndentGuide::Activate: {0}", ex));
                    }
                }
            }
            UpdateThemeList();

            try
            {
                IVsTextView view = null;
                IWpfTextView wpfView = null;
                TextManagerService.GetActiveView(0, null, out view);
                if (view == null)
                    CurrentContentType = null;
                else
                {
                    wpfView = EditorAdapters.GetWpfTextView(view);
                    CurrentContentType = wpfView.TextDataModel.ContentType.DisplayName;
                }
            }
            catch
            {
                CurrentContentType = null;
            }

            Child.Activate();

            ActiveTheme = LocalThemes.FirstOrDefault(theme => theme.ContentType == CurrentContentType);
            if (ActiveTheme == null)
                ActiveTheme = LocalThemes.FirstOrDefault(theme => theme.IsDefault);
        }

        internal void Apply()
        {
            Child.Apply();
            bool needsRefresh = false;
            
            if (ChangedThemes.Any())
            {
                foreach (var theme in ChangedThemes)
                {
                    theme.Apply();
                    if (theme.IsDefault)
                        Service.DefaultTheme = theme;
                    else
                        Service.Themes[theme.ContentType] = theme;
                }
                needsRefresh = true;
                ChangedThemes.Clear();
            }
            if (DeletedThemes.Any())
            {
                foreach (var theme in DeletedThemes)
                {
                    if (!theme.IsDefault)
                        Service.Themes.Remove(theme.ContentType);
                }
                needsRefresh = true;
                DeletedThemes.Clear();
            }

            if (needsRefresh)
                Service.OnThemesChanged();
        }

        internal void Close()
        {
            if (Interlocked.Decrement(ref ActivationCount) == 0)
            {
                LocalThemes.Clear();
                ChangedThemes.Clear();
                DeletedThemes.Clear();
            }
            IsActive = false;
        }

        private static IndentTheme _ActiveTheme;
        protected IndentTheme ActiveTheme
        {
            get { return _ActiveTheme; }
            set
            {
                if (_ActiveTheme != value)
                {
                    var old = _ActiveTheme;
                    _ActiveTheme = value;
                    Child.ActiveTheme = value;
                    if (cmbTheme.SelectedItem != value)
                        cmbTheme.SelectedItem = value;
                    UpdateDisplay(_ActiveTheme, old);
                }
                else
                {
                    UpdateDisplay(_ActiveTheme, _ActiveTheme);
                }
            }
        }

        private string _CurrentContentType;
        internal string CurrentContentType
        {
            get { return _CurrentContentType; }
            set
            {
                btnCustomizeThisContentType.Text = value ?? "";
                btnCustomizeThisContentType.Visible = (value != null);
                _CurrentContentType = value;
            }
        }

        private void btnCustomizeThisContentType_Click(object sender, EventArgs e)
        {
            try
            {
                var theme = LocalThemes.FirstOrDefault(t => t.ContentType != null &&
                    t.ContentType.Equals(CurrentContentType, StringComparison.InvariantCultureIgnoreCase));
                if (theme == null)
                {
                    if (ActiveTheme == null)
                        theme = new IndentTheme();
                    else
                        theme = ActiveTheme.Clone();
                    theme.ContentType = CurrentContentType;
                    LocalThemes.Add(theme);
                    ChangedThemes.Add(theme);
                    DeletedThemes.RemoveAll(t => t.ContentType == theme.ContentType);
                    UpdateThemeList();
                }
                cmbTheme.SelectedItem = theme;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("IndentGuide::btnCustomizeThisContentType_Click: {0}", ex));
            }
        }

        protected void UpdateThemeList()
        {
            try
            {
                LocalThemes.Sort();
                cmbTheme.Items.Clear();
                foreach (var theme in LocalThemes)
                {
                    cmbTheme.Items.Add(theme);
                }

                cmbTheme.SelectedItem = ActiveTheme;

                btnThemeDelete.Enabled = (ActiveTheme != null);
                btnThemeDelete.Text = ResourceLoader.LoadString(ActiveTheme.IsDefault ? "btnThemeReset" : "btnThemeDelete");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("IndentGuide::UpdateThemeList: {0}", ex));
            }
        }

        protected void LoadControlStrings(IEnumerable<Control> controls)
        {
            try
            {
                foreach (var control in controls)
                {
                    try
                    {
                        control.Text = ResourceLoader.LoadString(control.Name) ?? control.Text;
                    }
                    catch (InvalidOperationException) { }

                    if (control.Controls.Count > 0)
                    {
                        LoadControlStrings(control.Controls.OfType<Control>());
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("IndentGuide::LoadControlStrings: {0}", ex));
            }
        }

        private void cmbTheme_Format(object sender, ListControlConvertEventArgs e)
        {
            try
            {
                e.Value = ((IndentTheme)e.ListItem).ContentType ?? IndentTheme.DefaultThemeName;
            }
            catch
            {
                e.Value = (e.ListItem ?? "(null)").ToString();
            }
        }

        protected void UpdateDisplay()
        {
            UpdateDisplay(ActiveTheme, ActiveTheme);
        }

        protected void UpdateDisplay(IndentTheme active, IndentTheme previous)
        {
            Child.Update(active, previous);
        }

        private void ThemeOptionsControl_Load(object sender, EventArgs e)
        {
            LoadControlStrings(Controls.OfType<Control>());
            toolTip.SetToolTip(btnCustomizeThisContentType, ResourceLoader.LoadString("tooltipCustomizeThisContentType"));
        }

        private void cmbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActiveTheme = cmbTheme.SelectedItem as IndentTheme;

            btnThemeDelete.Enabled = (ActiveTheme != null);
            btnThemeDelete.Text = ResourceLoader.LoadString(ActiveTheme.IsDefault ? "btnThemeReset" : "btnThemeDelete");

            UpdateDisplay();
        }

        private void btnThemeDelete_Click(object sender, EventArgs e)
        {
            if (ActiveTheme == null) return;

            try
            {
                if (ActiveTheme.IsDefault)
                {
                    ChangedThemes.RemoveAll(theme => theme.IsDefault);
                    LocalThemes.Remove(ActiveTheme);
                    ActiveTheme = new IndentTheme();
                    LocalThemes.Add(ActiveTheme);
                    ChangedThemes.Add(ActiveTheme);
                    UpdateThemeList();
                }
                else
                {
                    DeletedThemes.Add(ActiveTheme);
                    LocalThemes.Remove(ActiveTheme);

                    int i = cmbTheme.SelectedIndex;
                    cmbTheme.Items.Remove(ActiveTheme);
                    if (i < cmbTheme.Items.Count) cmbTheme.SelectedIndex = i;
                    else cmbTheme.SelectedIndex = cmbTheme.Items.Count - 1;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("IndentGuide::btnThemeDelete_Click: {0}", ex));
            }
        }

    }
}
