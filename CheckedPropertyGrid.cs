using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

namespace IndentGuide
{
    public partial class CheckedPropertyGrid : UserControl
    {
        private readonly Dictionary<string, PropertyBox> CheckBoxes;

        class PropertyBox
        {
            public CheckBox CheckBox { get; set; }
            public string Name { get; private set; }
            public string DisplayName { get; private set; }
            public string Description { get; private set; }
            
            private readonly PropertyInfo PropInfo;
            
            public PropertyBox(PropertyInfo propInfo)
            {
                PropInfo = propInfo;
                Name = "chk" + propInfo.Name;
                DisplayName = GetDisplayName();
                Description = GetDescription();

                CheckBox = new CheckBox {
                    Name = Name,
                    Text = DisplayName,
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoSize = true,
                    Margin = new Padding(3, 1, 3, 2)
                };
                CheckBox.Tag = new WeakReference(this);
            }

            public void SetValue(object target)
            {
                try
                {
                    PropInfo.SetValue(target, CheckBox.Checked, null);
                }
                catch
                {
                    CheckBox.Enabled = false;
                }
            }

            public void GetValue(object source)
            {
                try
                {
                    CheckBox.Checked = (bool)PropInfo.GetValue(source, null);
                    CheckBox.Enabled = true;
                }
                catch
                {
                    CheckBox.Enabled = false;
                }
            }

            private string GetDisplayName()
            {
                try
                {
                    var name = PropInfo.GetCustomAttributes(false).OfType<DisplayNameAttribute>().First();
                    return name.DisplayName;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("CheckedPropertyGrid.GetDisplayName Exception: " + ex.ToString());
                    return PropInfo.Name;
                }
            }

            private string GetDescription()
            {
                try
                {
                    var descr = PropInfo.GetCustomAttributes(false).OfType<DescriptionAttribute>().First();
                    return descr.Description;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("CheckedPropertyGrid.GetDescription Exception: " + ex.ToString());
                    return PropInfo.Name;
                }
            }

        }

        public CheckedPropertyGrid()
        {
            InitializeComponent();
            panel.HorizontalScroll.Visible = false;

            CheckBoxes = new Dictionary<string, PropertyBox>();
        }

        public event EventHandler PropertyValueChanged;
        private void OnPropertyValueChanged()
        {
            var evt = PropertyValueChanged;
            if (evt != null)
                evt(this, EventArgs.Empty);
        }

        private object _SelectedObject;
        [Browsable(false)]
        public object SelectedObject
        {
            get { return _SelectedObject; }
            set
            {
                _SelectedObject = value;
                if (InvokeRequired)
                    BeginInvoke((Action)UpdateObject);
                else
                    UpdateObject();
            }
        }

        private void UpdateObject()
        {
            var obj = SelectedObject;
            if (obj == null) return;
            if (SelectableType != obj.GetType()) return;

            foreach (var check in CheckBoxes.Values)
            {
                check.GetValue(obj);
            }
        }

        private Type _SelectableType;
        [Browsable(false)]
        public Type SelectableType
        {
            get { return _SelectableType; }
            set
            {
                _SelectableType = value;
                if (InvokeRequired)
                    BeginInvoke((Action)UpdateType);
                else
                    UpdateType();
            }
        }

        private void UpdateType()
        {
            SuspendLayout();

            var type = SelectableType;
            if (type == null)
            {
                table.Controls.Clear();
                CheckBoxes.Clear();
                toolTip.RemoveAll();
                table.RowCount = 1;
            }
            else
            {
                CheckBoxes.Clear();
                foreach (var prop in type.GetProperties())
                {
                    if (!prop.CanWrite || !prop.GetSetMethod().IsPublic) continue;

                    var check = new PropertyBox(prop);
                    CheckBoxes[check.Name] = check;
                    check.CheckBox.CheckedChanged += new EventHandler(check_CheckedChanged);
                }

                table.Controls.Clear();
                toolTip.RemoveAll();
                table.RowCount = CheckBoxes.Count;
                int row = 0;
                foreach (var name in CheckBoxes.Keys.OrderBy(k => k))
                {
                    table.RowStyles[row].SizeType = SizeType.AutoSize;
                    var check = CheckBoxes[name];
                    table.Controls.Add(check.CheckBox, 0, row++);
                    toolTip.SetToolTip(check.CheckBox, check.Description);
                }
            }

            ResumeLayout(true);
        }

        void check_CheckedChanged(object sender, EventArgs e)
        {
            var check = sender as CheckBox;
            Debug.Assert(check != null);
            if (check == null) return;

            var weakref = check.Tag as WeakReference;
            Debug.Assert(weakref != null);
            if (weakref == null) return;

            var propInfo = weakref.Target as PropertyBox;
            if (propInfo == null) return;

            propInfo.SetValue(SelectedObject);

            OnPropertyValueChanged();
        }

    }
}
