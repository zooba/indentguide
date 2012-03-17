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
            this.tabPageAdvanced = new System.Windows.Forms.TabPage();
            this.grpLineMode = new System.Windows.Forms.GroupBox();
            this.panelLineOverrides = new System.Windows.Forms.TableLayoutPanel();
            this.lineTextPreview = new IndentGuide.LineTextPreview();
            this.gridLineMode = new IndentGuide.CheckedPropertyGrid();
            this.lstStyles = new System.Windows.Forms.ListView();
            this.tabTabs.SuspendLayout();
            this.tabPageQuick.SuspendLayout();
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
            this.tabPageQuick.Controls.Add(this.lstStyles);
            this.tabPageQuick.Location = new System.Drawing.Point(4, 22);
            this.tabPageQuick.Name = "tabPageQuick";
            this.tabPageQuick.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageQuick.Size = new System.Drawing.Size(465, 300);
            this.tabPageQuick.TabIndex = 0;
            this.tabPageQuick.Text = "tabPageQuick";
            this.tabPageQuick.UseVisualStyleBackColor = true;
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
            // lstStyles
            // 
            this.lstStyles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstStyles.Location = new System.Drawing.Point(3, 3);
            this.lstStyles.MultiSelect = false;
            this.lstStyles.Name = "lstStyles";
            this.lstStyles.Size = new System.Drawing.Size(459, 294);
            this.lstStyles.TabIndex = 0;
            this.lstStyles.UseCompatibleStateImageBehavior = false;
            // 
            // BehaviorOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabTabs);
            this.Name = "BehaviorOptionsControl";
            this.Size = new System.Drawing.Size(473, 326);
            this.tabTabs.ResumeLayout(false);
            this.tabPageQuick.ResumeLayout(false);
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
        private System.Windows.Forms.ListView lstStyles;
    }
}
