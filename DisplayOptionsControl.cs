using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace IndentGuide
{
    public partial class DisplayOptionsControl : UserControl
    {
        private DisplayOptions OptionsOwner;
        private IndentGuideService Service;

        public DisplayOptionsControl(DisplayOptions owner = null)
        {
            InitializeComponent();

            ChangedThemes = new List<IndentTheme>();
            DeletedThemes = new List<IndentTheme>();

            OptionsOwner = owner;
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
                lblCurrentContentType.Text = value ?? ResourceLoader.LoadString("UnknownContentType");
                btnCustomizeThisContentType.Enabled = (value != null);
                _CurrentContentType = value;
            }
        }

        private void UpdateThemeList()
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

        private void LoadControlStrings(IEnumerable<Control> controls)
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

        private void DisplayOptionsControl_Load(object sender, EventArgs e)
        {
            Service = (IndentGuideService)ServiceProvider.GlobalProvider.GetService(typeof(SIndentGuide));
            LoadControlStrings(this.Controls.OfType<Control>());

            var items = ResourceLoader.LoadString(txtLineFormatIndex.Name + "_Items").Split(';');
            txtLineFormatIndex.Items.Clear();
            txtLineFormatIndex.Items.AddRange(items);
            txtLineFormatIndex.SelectedIndex = 0;
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

        private IndentTheme ActiveTheme = null;

        private void UpdateDisplay()
        {
            if (ActiveTheme == null)
            {
                btnResetLineFormat.Enabled = false;
                gridLineStyle.SelectedObject = null;
                linePreview.ForeColor = Color.Teal;
                linePreview.Style = LineStyle.Solid;
                chkLineLogical.Checked = true;
                chkLineAbove.Checked = true;
            }
            else
            {
                LineFormat activeFormat = ActiveTheme.DefaultLineFormat;
                int i = txtLineFormatIndex.SelectedIndex;
                if (i > 0)
                {
                    if (!ActiveTheme.NumberedOverride.TryGetValue(i, out activeFormat))
                    {
                        activeFormat = ActiveTheme.DefaultLineFormat.Clone();
                        ActiveTheme.NumberedOverride[i] = activeFormat;
                    }
                    btnResetLineFormat.Enabled = !ActiveTheme.DefaultLineFormat.Equals(activeFormat);
                }
                else
                {
                    btnResetLineFormat.Enabled = !ActiveTheme.DefaultLineFormat.Equals(new LineFormat());
                }

                gridLineStyle.SelectedObject = activeFormat;
                linePreview.ForeColor = activeFormat.LineColor;
                linePreview.Style = activeFormat.LineStyle;
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

        private void gridLineStyle_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (!ChangedThemes.Contains(ActiveTheme)) ChangedThemes.Add(ActiveTheme);
            UpdateDisplay();
        }

        internal void SaveIfRequired()
        {
            if (btnThemeSaveAs.Enabled) btnThemeSaveAs.PerformClick();
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

        private int txtLineFormatIndex_PreviousIndex = -1;
        private void txtLineFormatIndex_SelectedItemChanged(object sender, EventArgs e)
        {
            LineFormat prevFormat;
            if (ActiveTheme != null &&
                txtLineFormatIndex_PreviousIndex > 0 &&
                ActiveTheme.NumberedOverride.TryGetValue(txtLineFormatIndex_PreviousIndex, out prevFormat) &&
                prevFormat.Equals(ActiveTheme.DefaultLineFormat))
            {
                ActiveTheme.NumberedOverride.Remove(txtLineFormatIndex_PreviousIndex);
            }

            txtLineFormatIndex_PreviousIndex = txtLineFormatIndex.SelectedIndex;

            UpdateDisplay();
        }

        private void btnResetLineFormat_Click(object sender, EventArgs e)
        {
            if (ActiveTheme == null) return;

            int i = txtLineFormatIndex.SelectedIndex;
            if (i > 0)
            {
                ActiveTheme.NumberedOverride.Remove(i);
            }
            else
            {
                ActiveTheme.Apply();
                ActiveTheme.DefaultLineFormat = new LineFormat();
            }
            ChangedThemes.Add(ActiveTheme);
            
            UpdateDisplay();
        }
    }
}
