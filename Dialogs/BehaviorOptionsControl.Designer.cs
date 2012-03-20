namespace IndentGuide {
    partial class BehaviorOptionsControl {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tabTabs = new System.Windows.Forms.TabControl();
            this.tabPageQuick = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.preset1 = new IndentGuide.LineTextPreview();
            this.preset2 = new IndentGuide.LineTextPreview();
            this.preset3 = new IndentGuide.LineTextPreview();
            this.preset4 = new IndentGuide.LineTextPreview();
            this.preset5 = new IndentGuide.LineTextPreview();
            this.preset6 = new IndentGuide.LineTextPreview();
            this.tabPageAdvanced = new System.Windows.Forms.TabPage();
            this.grpLineMode = new System.Windows.Forms.GroupBox();
            this.panelLineOverrides = new System.Windows.Forms.TableLayoutPanel();
            this.lineTextPreview = new IndentGuide.LineTextPreview();
            this.gridLineMode = new IndentGuide.CheckedPropertyGrid();
            this.tabTabs.SuspendLayout();
            this.tabPageQuick.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabPageAdvanced.SuspendLayout();
            this.grpLineMode.SuspendLayout();
            this.panelLineOverrides.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabTabs
            // 
            this.tabTabs.Controls.Add(this.tabPageQuick);
            this.tabTabs.Controls.Add(this.tabPageAdvanced);
            this.tabTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabTabs.Location = new System.Drawing.Point(0, 0);
            this.tabTabs.Name = "tabTabs";
            this.tabTabs.SelectedIndex = 0;
            this.tabTabs.Size = new System.Drawing.Size(473, 326);
            this.tabTabs.TabIndex = 0;
            // 
            // tabPageQuick
            // 
            this.tabPageQuick.Controls.Add(this.tableLayoutPanel1);
            this.tabPageQuick.Location = new System.Drawing.Point(4, 22);
            this.tabPageQuick.Name = "tabPageQuick";
            this.tabPageQuick.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageQuick.Size = new System.Drawing.Size(465, 300);
            this.tabPageQuick.TabIndex = 0;
            this.tabPageQuick.Text = "tabPageQuick";
            this.tabPageQuick.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.preset1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.preset2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.preset3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.preset4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.preset5, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.preset6, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(459, 294);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // preset1
            // 
            this.preset1.BackColor = System.Drawing.SystemColors.Window;
            this.preset1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.preset1.IndentSize = 4;
            this.preset1.Location = new System.Drawing.Point(3, 3);
            this.preset1.Name = "preset1";
            this.preset1.Size = new System.Drawing.Size(147, 141);
            this.preset1.TabIndex = 0;
            this.preset1.Text = "Preset text";
            this.preset1.Click += new System.EventHandler(this.Preset_Click);
            // 
            // preset2
            // 
            this.preset2.BackColor = System.Drawing.SystemColors.Window;
            this.preset2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.preset2.IndentSize = 4;
            this.preset2.Location = new System.Drawing.Point(156, 3);
            this.preset2.Name = "preset2";
            this.preset2.Size = new System.Drawing.Size(147, 141);
            this.preset2.TabIndex = 0;
            this.preset2.Text = "Preset text";
            this.preset2.Click += new System.EventHandler(this.Preset_Click);
            // 
            // preset3
            // 
            this.preset3.BackColor = System.Drawing.SystemColors.Window;
            this.preset3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.preset3.IndentSize = 4;
            this.preset3.Location = new System.Drawing.Point(309, 3);
            this.preset3.Name = "preset3";
            this.preset3.Size = new System.Drawing.Size(147, 141);
            this.preset3.TabIndex = 0;
            this.preset3.Text = "Preset text";
            this.preset3.Click += new System.EventHandler(this.Preset_Click);
            // 
            // preset4
            // 
            this.preset4.BackColor = System.Drawing.SystemColors.Window;
            this.preset4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.preset4.IndentSize = 4;
            this.preset4.Location = new System.Drawing.Point(3, 150);
            this.preset4.Name = "preset4";
            this.preset4.Size = new System.Drawing.Size(147, 141);
            this.preset4.TabIndex = 0;
            this.preset4.Text = "Preset text";
            this.preset4.Click += new System.EventHandler(this.Preset_Click);
            // 
            // preset5
            // 
            this.preset5.BackColor = System.Drawing.SystemColors.Window;
            this.preset5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.preset5.IndentSize = 4;
            this.preset5.Location = new System.Drawing.Point(156, 150);
            this.preset5.Name = "preset5";
            this.preset5.Size = new System.Drawing.Size(147, 141);
            this.preset5.TabIndex = 0;
            this.preset5.Text = "Preset text";
            this.preset5.Click += new System.EventHandler(this.Preset_Click);
            // 
            // preset6
            // 
            this.preset6.BackColor = System.Drawing.SystemColors.Window;
            this.preset6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.preset6.IndentSize = 4;
            this.preset6.Location = new System.Drawing.Point(309, 150);
            this.preset6.Name = "preset6";
            this.preset6.Size = new System.Drawing.Size(147, 141);
            this.preset6.TabIndex = 0;
            this.preset6.Text = "Preset text";
            this.preset6.Click += new System.EventHandler(this.Preset_Click);
            // 
            // tabPageAdvanced
            // 
            this.tabPageAdvanced.Controls.Add(this.grpLineMode);
            this.tabPageAdvanced.Location = new System.Drawing.Point(4, 22);
            this.tabPageAdvanced.Name = "tabPageAdvanced";
            this.tabPageAdvanced.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAdvanced.Size = new System.Drawing.Size(465, 300);
            this.tabPageAdvanced.TabIndex = 1;
            this.tabPageAdvanced.Text = "tabPageAdvanced";
            this.tabPageAdvanced.UseVisualStyleBackColor = true;
            // 
            // grpLineMode
            // 
            this.grpLineMode.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpLineMode.Controls.Add(this.panelLineOverrides);
            this.grpLineMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLineMode.Location = new System.Drawing.Point(3, 3);
            this.grpLineMode.Name = "grpLineMode";
            this.grpLineMode.Size = new System.Drawing.Size(459, 294);
            this.grpLineMode.TabIndex = 6;
            this.grpLineMode.TabStop = false;
            this.grpLineMode.Text = "grpLineMode";
            // 
            // panelLineOverrides
            // 
            this.panelLineOverrides.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelLineOverrides.ColumnCount = 2;
            this.panelLineOverrides.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelLineOverrides.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelLineOverrides.Controls.Add(this.lineTextPreview, 1, 0);
            this.panelLineOverrides.Controls.Add(this.gridLineMode, 0, 0);
            this.panelLineOverrides.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLineOverrides.Location = new System.Drawing.Point(3, 16);
            this.panelLineOverrides.Name = "panelLineOverrides";
            this.panelLineOverrides.RowCount = 1;
            this.panelLineOverrides.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelLineOverrides.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.panelLineOverrides.Size = new System.Drawing.Size(453, 275);
            this.panelLineOverrides.TabIndex = 0;
            // 
            // lineTextPreview
            // 
            this.lineTextPreview.BackColor = System.Drawing.SystemColors.Window;
            this.lineTextPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lineTextPreview.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lineTextPreview.IndentSize = 4;
            this.lineTextPreview.Location = new System.Drawing.Point(230, 4);
            this.lineTextPreview.Margin = new System.Windows.Forms.Padding(4);
            this.lineTextPreview.Name = "lineTextPreview";
            this.lineTextPreview.Size = new System.Drawing.Size(219, 267);
            this.lineTextPreview.TabIndex = 1;
            this.lineTextPreview.Text = "lineTextPreview";
            // 
            // gridLineMode
            // 
            this.gridLineMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLineMode.Location = new System.Drawing.Point(3, 3);
            this.gridLineMode.Name = "gridLineMode";
            this.gridLineMode.SelectableType = null;
            this.gridLineMode.SelectedObject = null;
            this.gridLineMode.Size = new System.Drawing.Size(220, 269);
            this.gridLineMode.TabIndex = 6;
            this.gridLineMode.PropertyValueChanged += new System.EventHandler(this.gridLineMode_PropertyValueChanged);
            // 
            // BehaviorOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabTabs);
            this.DoubleBuffered = true;
            this.Name = "BehaviorOptionsControl";
            this.Size = new System.Drawing.Size(473, 326);
            this.tabTabs.ResumeLayout(false);
            this.tabPageQuick.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabPageAdvanced.ResumeLayout(false);
            this.grpLineMode.ResumeLayout(false);
            this.panelLineOverrides.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabTabs;
        private System.Windows.Forms.TabPage tabPageQuick;
        private System.Windows.Forms.TabPage tabPageAdvanced;
        private System.Windows.Forms.GroupBox grpLineMode;
        private System.Windows.Forms.TableLayoutPanel panelLineOverrides;
        private LineTextPreview lineTextPreview;
        private CheckedPropertyGrid gridLineMode;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private LineTextPreview preset1;
        private LineTextPreview preset2;
        private LineTextPreview preset3;
        private LineTextPreview preset4;
        private LineTextPreview preset5;
        private LineTextPreview preset6;
    }
}
