namespace IndentGuide
{
    partial class DisplayOptionsControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.grpLineStyle = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lstOverrides = new System.Windows.Forms.ListBox();
            this.linePreview = new IndentGuide.LinePreview();
            this.gridLineStyle = new System.Windows.Forms.PropertyGrid();
            this.grpLineMode = new System.Windows.Forms.GroupBox();
            this.panelLineOverrides = new System.Windows.Forms.TableLayoutPanel();
            this.lineTextPreview = new IndentGuide.LineTextPreview();
            this.gridLineMode = new System.Windows.Forms.PropertyGrid();
            this.lstModePresets = new System.Windows.Forms.ComboBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.grpLineStyle.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.grpLineMode.SuspendLayout();
            this.panelLineOverrides.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.grpLineStyle, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.grpLineMode, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(455, 317);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // grpLineStyle
            // 
            this.grpLineStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLineStyle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpLineStyle.Controls.Add(this.tableLayoutPanel2);
            this.grpLineStyle.Location = new System.Drawing.Point(3, 3);
            this.grpLineStyle.Name = "grpLineStyle";
            this.grpLineStyle.Size = new System.Drawing.Size(449, 100);
            this.grpLineStyle.TabIndex = 4;
            this.grpLineStyle.TabStop = false;
            this.grpLineStyle.Text = "grpLineStyle";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel2.Controls.Add(this.lstOverrides, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.linePreview, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.gridLineStyle, 3, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(443, 81);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // lstOverrides
            // 
            this.lstOverrides.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstOverrides.FormattingEnabled = true;
            this.lstOverrides.IntegralHeight = false;
            this.lstOverrides.Location = new System.Drawing.Point(21, 3);
            this.lstOverrides.Name = "lstOverrides";
            this.lstOverrides.Size = new System.Drawing.Size(102, 75);
            this.lstOverrides.TabIndex = 0;
            this.lstOverrides.SelectedIndexChanged += new System.EventHandler(this.lstOverrides_SelectedIndexChanged);
            this.lstOverrides.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.lstOverrides_Format);
            // 
            // linePreview
            // 
            this.linePreview.BackColor = System.Drawing.SystemColors.Window;
            this.linePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linePreview.Location = new System.Drawing.Point(129, 3);
            this.linePreview.Name = "linePreview";
            this.linePreview.Size = new System.Drawing.Size(75, 75);
            this.linePreview.Style = IndentGuide.LineStyle.Solid;
            this.linePreview.TabIndex = 0;
            // 
            // gridLineStyle
            // 
            this.gridLineStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridLineStyle.CommandsVisibleIfAvailable = false;
            this.gridLineStyle.HelpVisible = false;
            this.gridLineStyle.Location = new System.Drawing.Point(210, 3);
            this.gridLineStyle.Name = "gridLineStyle";
            this.gridLineStyle.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.gridLineStyle.Size = new System.Drawing.Size(211, 75);
            this.gridLineStyle.TabIndex = 4;
            this.gridLineStyle.ToolbarVisible = false;
            this.gridLineStyle.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.gridLineStyle_PropertyValueChanged);
            // 
            // grpLineMode
            // 
            this.grpLineMode.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpLineMode.Controls.Add(this.panelLineOverrides);
            this.grpLineMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLineMode.Location = new System.Drawing.Point(3, 109);
            this.grpLineMode.Name = "grpLineMode";
            this.grpLineMode.Size = new System.Drawing.Size(449, 205);
            this.grpLineMode.TabIndex = 5;
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
            this.panelLineOverrides.Controls.Add(this.gridLineMode, 0, 1);
            this.panelLineOverrides.Controls.Add(this.lstModePresets, 0, 0);
            this.panelLineOverrides.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLineOverrides.Location = new System.Drawing.Point(3, 16);
            this.panelLineOverrides.Name = "panelLineOverrides";
            this.panelLineOverrides.RowCount = 2;
            this.panelLineOverrides.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelLineOverrides.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelLineOverrides.Size = new System.Drawing.Size(443, 186);
            this.panelLineOverrides.TabIndex = 0;
            // 
            // lineTextPreview
            // 
            this.lineTextPreview.BackColor = System.Drawing.SystemColors.Window;
            this.lineTextPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lineTextPreview.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lineTextPreview.IndentSize = 4;
            this.lineTextPreview.Location = new System.Drawing.Point(225, 4);
            this.lineTextPreview.Margin = new System.Windows.Forms.Padding(4);
            this.lineTextPreview.Name = "lineTextPreview";
            this.panelLineOverrides.SetRowSpan(this.lineTextPreview, 2);
            this.lineTextPreview.Size = new System.Drawing.Size(214, 178);
            this.lineTextPreview.TabIndex = 1;
            this.lineTextPreview.Text = "lineTextPreview";
            // 
            // gridLineMode
            // 
            this.gridLineMode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridLineMode.CommandsVisibleIfAvailable = false;
            this.gridLineMode.Location = new System.Drawing.Point(3, 30);
            this.gridLineMode.Name = "gridLineMode";
            this.gridLineMode.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.gridLineMode.Size = new System.Drawing.Size(215, 153);
            this.gridLineMode.TabIndex = 4;
            this.gridLineMode.ToolbarVisible = false;
            this.gridLineMode.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.gridLineMode_PropertyValueChanged);
            // 
            // lstModePresets
            // 
            this.lstModePresets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lstModePresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstModePresets.FormattingEnabled = true;
            this.lstModePresets.Location = new System.Drawing.Point(3, 3);
            this.lstModePresets.Name = "lstModePresets";
            this.lstModePresets.Size = new System.Drawing.Size(215, 21);
            this.lstModePresets.TabIndex = 5;
            this.lstModePresets.SelectedIndexChanged += new System.EventHandler(this.lstModePresets_SelectedIndexChanged);
            // 
            // DisplayOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DisplayOptionsControl";
            this.Size = new System.Drawing.Size(455, 317);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.grpLineStyle.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.grpLineMode.ResumeLayout(false);
            this.panelLineOverrides.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox grpLineStyle;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private LinePreview linePreview;
        private System.Windows.Forms.PropertyGrid gridLineStyle;
        private System.Windows.Forms.GroupBox grpLineMode;
        private System.Windows.Forms.TableLayoutPanel panelLineOverrides;
        private System.Windows.Forms.ListBox lstOverrides;
        private System.Windows.Forms.ToolTip toolTip;
        private LineTextPreview lineTextPreview;
        private System.Windows.Forms.PropertyGrid gridLineMode;
        private System.Windows.Forms.ComboBox lstModePresets;
    }
}
