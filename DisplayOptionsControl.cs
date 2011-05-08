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
        }

        private void DisplayOptionsControl_Load(object sender, EventArgs e)
        {
            Service = (IndentGuideService)ServiceProvider.GlobalProvider.GetService(typeof(SIndentGuide));

            foreach (var control in this.Controls.OfType<Control>())
            {
                try
                {
                    control.Text = ResourceLoader.LoadString(control.Name);
                }
                catch (InvalidOperationException) { }
            }

            cmbTheme.Items.Clear();
            foreach (var theme in Service.Themes.Values.OrderBy(t => t))
            {
                cmbTheme.Items.Add(theme);
            }
        }

        private void cmbTheme_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = ((IndentTheme)e.ListItem).Name;
        }
    }
}
