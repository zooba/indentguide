using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.ComponentModelHost;
using System.Diagnostics;
using Microsoft.VisualStudio.Text.Editor;

namespace IndentGuide
{
    public partial class ThemeOptionsControl : UserControl
    {
        private readonly IndentGuideService Service;
        private readonly IVsTextManager TextManagerService;
        private readonly IVsEditorAdaptersFactoryService EditorAdapters;

        private readonly List<IndentTheme> ChangedThemes;
        private readonly List<IndentTheme> DeletedThemes;

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

            ChangedThemes = new List<IndentTheme>();
            DeletedThemes = new List<IndentTheme>();
        }

        void Child_ThemeChanged(object sender, ThemeEventArgs e)
        {
            if (!ChangedThemes.Contains(e.Theme)) ChangedThemes.Add(e.Theme);
        }

        internal void Activate()
        {
            LocalThemes = Service.Themes.Values.Select(t => t.Clone()).ToList();
            LocalThemes.Sort();
            UpdateThemeList();

            try
            {
                IVsTextView view = null;
                IWpfTextView wpfView = null;
                TextManagerService.GetActiveView(0, null, out view);
                wpfView = EditorAdapters.GetWpfTextView(view);
                CurrentContentType = wpfView.TextDataModel.ContentType.DisplayName;
            }
            catch
            {
                CurrentContentType = null;
            }

            Child.Activate();
        }

        internal void Apply()
        {
            Child.Apply();
            
            if (btnThemeSaveAs.Enabled) btnThemeSaveAs.PerformClick();

            if (ChangedThemes.Any())
            {
                foreach (var theme in ChangedThemes)
                {
                    theme.Apply();
                    Service.Themes[theme.Name] = theme;
                    if (theme.IsDefault) Service.DefaultTheme = theme;
                }
                if (!DeletedThemes.Any()) Service.OnThemesChanged();
                ChangedThemes.Clear();
            }
            if (DeletedThemes.Any())
            {
                foreach (var theme in DeletedThemes)
                {
                    Service.Themes.Remove(theme.Name);
                }
                Service.OnThemesChanged();
                DeletedThemes.Clear();
            }
        }

        internal void Cancel()
        {
            Child.Cancel();
        }

        private IndentTheme _ActiveTheme;
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
                    UpdateDisplay(_ActiveTheme, old);
                }
                else
                {
                    UpdateDisplay(_ActiveTheme, _ActiveTheme);
                }
            }
        }

        private List<IndentTheme> _LocalThemes;
        internal List<IndentTheme> LocalThemes
        {
            get
            {
                return _LocalThemes;
            }
            set
            {
                _LocalThemes = value;
                try
                {
                    ChangedThemes.Clear();
                    DeletedThemes.Clear();
                    if (_LocalThemes.Count == 0)
                    {
                        var defaultTheme = new IndentTheme(true);
                        _LocalThemes.Add(defaultTheme);
                        ChangedThemes.Add(defaultTheme);
                    }

                    Invoke((Action)UpdateThemeList);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("IndentGuide::get_LocalThemes: {0}", ex));
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
                var theme = LocalThemes.FirstOrDefault(t => t.Name.Equals(CurrentContentType,
                    StringComparison.InvariantCultureIgnoreCase));
                if (theme == null)
                {
                    cmbTheme.Text = CurrentContentType;
                    btnThemeSaveAs.PerformClick();
                }
                else
                {
                    cmbTheme.SelectedItem = theme;
                }
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
                cmbTheme.Items.Clear();
                foreach (var theme in LocalThemes)
                {
                    cmbTheme.Items.Add(theme);
                }
                if (cmbTheme.Items.Count > 0) cmbTheme.SelectedIndex = 0;
                btnThemeDelete.Enabled = false;
                btnThemeSaveAs.Enabled = false;
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
                        control.Text = ResourceLoader.LoadString(control.Name);
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
                e.Value = ((IndentTheme)e.ListItem).Name;
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
        }

        private void cmbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActiveTheme = cmbTheme.SelectedItem as IndentTheme;

            btnThemeSaveAs.Enabled = false;
            btnThemeDelete.Enabled = (ActiveTheme != null) && !ActiveTheme.IsDefault;

            UpdateDisplay();
        }

        private void cmbTheme_TextChanged(object sender, EventArgs e)
        {
            var match = LocalThemes.FirstOrDefault(t => t.Name.Equals(cmbTheme.Text, StringComparison.CurrentCultureIgnoreCase));
            if (match == null)
            {
                if (LocalThemes.Contains(ActiveTheme)) ActiveTheme = ActiveTheme.Clone(true);
                btnThemeSaveAs.Enabled = true;
                btnThemeDelete.Enabled = false;
            }
            else
            {
                ActiveTheme = match;
                btnThemeSaveAs.Enabled = false;
                btnThemeDelete.Enabled = !ActiveTheme.IsDefault;
            }
            UpdateDisplay();
        }

        private void btnThemeSaveAs_Click(object sender, EventArgs e)
        {
            if (ActiveTheme == null) return;

            try
            {
                var newTheme = ActiveTheme;
                newTheme.Name = cmbTheme.Text;
                ChangedThemes.Add(newTheme);
                LocalThemes.Add(newTheme);
                LocalThemes.Sort();
                UpdateThemeList();
                cmbTheme.SelectedItem = newTheme;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("IndentGuide::btnThemeSaveAs_Click: {0}", ex));
            }
        }

        private void btnThemeDelete_Click(object sender, EventArgs e)
        {
            if (ActiveTheme == null || ActiveTheme.IsDefault) return;

            try
            {
                DeletedThemes.Add(ActiveTheme);
                LocalThemes.Remove(ActiveTheme);

                int i = cmbTheme.SelectedIndex;
                cmbTheme.Items.Remove(ActiveTheme);
                if (i < cmbTheme.Items.Count) cmbTheme.SelectedIndex = i;
                else cmbTheme.SelectedIndex = cmbTheme.Items.Count - 1;
                ActiveTheme = cmbTheme.SelectedItem as IndentTheme;
                UpdateDisplay();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("IndentGuide::btnThemeDelete_Click: {0}", ex));
            }
        }

    }
}
