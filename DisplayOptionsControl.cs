using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace IndentGuide
{
    public partial class DisplayOptionsControl : UserControl
    {
        private IndentGuideService Service;
        
        public DisplayOptionsControl()
        {
            InitializeComponent();

            ChangedThemes = new List<IndentTheme>();
            DeletedThemes = new List<IndentTheme>();
        }

        internal readonly List<IndentTheme> ChangedThemes;
        internal readonly List<IndentTheme> DeletedThemes;

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
                ChangedThemes.Clear();
                DeletedThemes.Clear();

                Invoke((Action)(() =>
                {
                    cmbTheme.Items.Clear();
                    foreach (var theme in LocalThemes)
                    {
                        cmbTheme.Items.Add(theme);
                    }
                    cmbTheme.SelectedIndex = 0;
                }));
            }
        }

        private void LoadControlStrings(IEnumerable<Control> controls)
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

        private void DisplayOptionsControl_Load(object sender, EventArgs e)
        {
            Service = (IndentGuideService)ServiceProvider.GlobalProvider.GetService(typeof(SIndentGuide));
            LoadControlStrings(this.Controls.OfType<Control>());
        }

        private void cmbTheme_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = ((IndentTheme)e.ListItem).Name;
        }

        private IndentTheme ActiveTheme = null;

        private void UpdateDisplay()
        {
            if (ActiveTheme == null)
            {
                gridLineStyle.SelectedObject = null;
                linePreview.ForeColor = Color.Teal;
                linePreview.Style = LineStyle.Solid;
                chkLineLogical.Checked = true;
                chkLineAbove.Checked = true;
            }
            else
            {
                gridLineStyle.SelectedObject = ActiveTheme.LineFormat;
                linePreview.ForeColor = ActiveTheme.LineFormat.LineColor;
                linePreview.Style = ActiveTheme.LineFormat.LineStyle;
                switch (ActiveTheme.EmptyLineMode)
                {
                case EmptyLineMode.NoGuides:
                    break;
                case EmptyLineMode.SameAsLineAboveActual:
                    chkLineActual.Checked = true;
                    chkLineAbove.Checked = true;
                    break;
                case EmptyLineMode.SameAsLineAboveLogical:
                    chkLineLogical.Checked = true;
                    chkLineAbove.Checked = true;
                    break;
                case EmptyLineMode.SameAsLineBelowActual:
                    chkLineActual.Checked = true;
                    chkLineBelow.Checked = true;
                    break;
                case EmptyLineMode.SameAsLineBelowLogical:
                    chkLineLogical.Checked = true;
                    chkLineBelow.Checked = true;
                    break;
                }
            }
        }

        private void cmbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActiveTheme = cmbTheme.SelectedItem as IndentTheme;
            
            btnThemeSaveAs.Enabled = false;
            btnThemeDelete.Enabled = !ActiveTheme.IsDefault;

            UpdateDisplay();
        }

        private void cmbTheme_TextChanged(object sender, EventArgs e)
        {
            btnThemeSaveAs.Enabled = !LocalThemes.Any(t => t.Name.Equals(cmbTheme.Text));
        }

        private void gridLineStyle_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (!ChangedThemes.Contains(ActiveTheme)) ChangedThemes.Add(ActiveTheme);
            UpdateDisplay();
        }

        private void btnThemeSaveAs_Click(object sender, EventArgs e)
        {
            if (ActiveTheme == null) return;

            var newTheme = ActiveTheme.Clone(true);
            newTheme.Name = cmbTheme.Text;
            ChangedThemes.Add(newTheme);
            var localThemes = LocalThemes.ToList();
            localThemes.Add(newTheme);
            localThemes.Sort();
            LocalThemes = localThemes;
            cmbTheme.SelectedItem = newTheme;
        }

        private void btnThemeDelete_Click(object sender, EventArgs e)
        {
            if (ActiveTheme == null) return;
            if (ActiveTheme.IsDefault) return;

            DeletedThemes.Add(ActiveTheme);
            LocalThemes.Remove(ActiveTheme);
            
            int i = cmbTheme.SelectedIndex;
            cmbTheme.Items.Remove(ActiveTheme);
            if (i < cmbTheme.Items.Count) cmbTheme.SelectedIndex = i;
            else cmbTheme.SelectedIndex = cmbTheme.Items.Count - 1;
        }

        private void chkEmptyLineBehaviour_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveTheme == null) return;
            if (!ChangedThemes.Contains(ActiveTheme)) ChangedThemes.Add(ActiveTheme);
            if (chkLineActual.Checked)
            {
                ActiveTheme.EmptyLineMode = chkLineAbove.Checked
                    ? EmptyLineMode.SameAsLineAboveActual
                    : EmptyLineMode.SameAsLineBelowActual;
            }
            else
            {
                ActiveTheme.EmptyLineMode = chkLineAbove.Checked
                    ? EmptyLineMode.SameAsLineAboveLogical
                    : EmptyLineMode.SameAsLineBelowLogical;
            }
        }

    }
}
