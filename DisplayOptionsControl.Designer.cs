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
            this.linePreview = new IndentGuide.LinePreview();
            this.gridLineStyle = new System.Windows.Forms.PropertyGrid();
            this.grpLineOverrides = new System.Windows.Forms.GroupBox();
            this.panelLineOverrides = new System.Windows.Forms.TableLayoutPanel();
            this.lstOverrides = new System.Windows.Forms.ListBox();
            this.panelLineOverrideFormat = new System.Windows.Forms.TableLayoutPanel();
            this.gridLineOverride = new System.Windows.Forms.PropertyGrid();
            this.panelLineOverrideIndex = new System.Windows.Forms.TableLayoutPanel();
            this.chkLineOverrideUnaligned = new System.Windows.Forms.RadioButton();
            this.txtLineFormatText = new System.Windows.Forms.ComboBox();
            this.chkLineOverrideText = new System.Windows.Forms.RadioButton();
            this.chkLineOverrideIndex = new System.Windows.Forms.RadioButton();
            this.txtLineFormatIndex = new System.Windows.Forms.NumericUpDown();
            this.lineOverridePreview = new IndentGuide.LinePreview();
            this.tableLineOverrideAddRemove = new System.Windows.Forms.TableLayoutPanel();
            this.btnAddOverride = new System.Windows.Forms.Button();
            this.btnRemoveOverride = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.grpLineStyle.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.grpLineOverrides.SuspendLayout();
            this.panelLineOverrides.SuspendLayout();
            this.panelLineOverrideFormat.SuspendLayout();
            this.panelLineOverrideIndex.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLineFormatIndex)).BeginInit();
            this.tableLineOverrideAddRemove.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.grpLineStyle, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.grpLineOverrides, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(592, 317);
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
            this.grpLineStyle.Size = new System.Drawing.Size(586, 100);
            this.grpLineStyle.TabIndex = 4;
            this.grpLineStyle.TabStop = false;
            this.grpLineStyle.Text = "grpLineStyle";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9F));
            this.tableLayoutPanel2.Controls.Add(this.linePreview, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.gridLineStyle, 3, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(580, 81);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // linePreview
            // 
            this.linePreview.BackColor = System.Drawing.SystemColors.Window;
            this.linePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linePreview.Location = new System.Drawing.Point(47, 3);
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
            this.gridLineStyle.Location = new System.Drawing.Point(137, 3);
            this.gridLineStyle.Name = "gridLineStyle";
            this.gridLineStyle.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.gridLineStyle.Size = new System.Drawing.Size(393, 75);
            this.gridLineStyle.TabIndex = 4;
            this.gridLineStyle.ToolbarVisible = false;
            this.gridLineStyle.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.gridLineStyle_PropertyValueChanged);
            // 
            // grpLineOverrides
            // 
            this.grpLineOverrides.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLineOverrides.AutoSize = true;
            this.grpLineOverrides.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpLineOverrides.Controls.Add(this.panelLineOverrides);
            this.grpLineOverrides.Location = new System.Drawing.Point(3, 109);
            this.grpLineOverrides.Name = "grpLineOverrides";
            this.grpLineOverrides.Size = new System.Drawing.Size(586, 176);
            this.grpLineOverrides.TabIndex = 5;
            this.grpLineOverrides.TabStop = false;
            this.grpLineOverrides.Text = "grpLineOverrides";
            // 
            // panelLineOverrides
            // 
            this.panelLineOverrides.AutoSize = true;
            this.panelLineOverrides.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelLineOverrides.ColumnCount = 2;
            this.panelLineOverrides.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.panelLineOverrides.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.panelLineOverrides.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.panelLineOverrides.Controls.Add(this.lstOverrides, 0, 0);
            this.panelLineOverrides.Controls.Add(this.panelLineOverrideFormat, 1, 0);
            this.panelLineOverrides.Controls.Add(this.tableLineOverrideAddRemove, 0, 1);
            this.panelLineOverrides.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLineOverrides.Location = new System.Drawing.Point(3, 16);
            this.panelLineOverrides.Name = "panelLineOverrides";
            this.panelLineOverrides.RowCount = 2;
            this.panelLineOverrides.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelLineOverrides.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelLineOverrides.Size = new System.Drawing.Size(580, 157);
            this.panelLineOverrides.TabIndex = 0;
            // 
            // lstOverrides
            // 
            this.lstOverrides.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstOverrides.FormattingEnabled = true;
            this.lstOverrides.IntegralHeight = false;
            this.lstOverrides.Location = new System.Drawing.Point(3, 3);
            this.lstOverrides.Name = "lstOverrides";
            this.lstOverrides.Size = new System.Drawing.Size(187, 122);
            this.lstOverrides.Sorted = true;
            this.lstOverrides.TabIndex = 0;
            this.lstOverrides.SelectedIndexChanged += new System.EventHandler(this.lstOverrides_SelectedIndexChanged);
            this.lstOverrides.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.lstOverrides_Format);
            // 
            // panelLineOverrideFormat
            // 
            this.panelLineOverrideFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLineOverrideFormat.AutoSize = true;
            this.panelLineOverrideFormat.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelLineOverrideFormat.ColumnCount = 2;
            this.panelLineOverrideFormat.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panelLineOverrideFormat.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelLineOverrideFormat.Controls.Add(this.gridLineOverride, 1, 0);
            this.panelLineOverrideFormat.Controls.Add(this.panelLineOverrideIndex, 1, 1);
            this.panelLineOverrideFormat.Controls.Add(this.lineOverridePreview, 0, 0);
            this.panelLineOverrideFormat.Location = new System.Drawing.Point(193, 0);
            this.panelLineOverrideFormat.Margin = new System.Windows.Forms.Padding(0);
            this.panelLineOverrideFormat.Name = "panelLineOverrideFormat";
            this.panelLineOverrideFormat.RowCount = 2;
            this.panelLineOverrides.SetRowSpan(this.panelLineOverrideFormat, 2);
            this.panelLineOverrideFormat.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelLineOverrideFormat.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelLineOverrideFormat.Size = new System.Drawing.Size(387, 157);
            this.panelLineOverrideFormat.TabIndex = 3;
            // 
            // gridLineOverride
            // 
            this.gridLineOverride.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridLineOverride.CommandsVisibleIfAvailable = false;
            this.gridLineOverride.HelpVisible = false;
            this.gridLineOverride.Location = new System.Drawing.Point(84, 3);
            this.gridLineOverride.Name = "gridLineOverride";
            this.gridLineOverride.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.gridLineOverride.Size = new System.Drawing.Size(300, 75);
            this.gridLineOverride.TabIndex = 4;
            this.gridLineOverride.ToolbarVisible = false;
            this.gridLineOverride.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.gridLineStyle_PropertyValueChanged);
            // 
            // panelLineOverrideIndex
            // 
            this.panelLineOverrideIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLineOverrideIndex.AutoSize = true;
            this.panelLineOverrideIndex.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelLineOverrideIndex.ColumnCount = 2;
            this.panelLineOverrideIndex.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelLineOverrideIndex.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.panelLineOverrideIndex.Controls.Add(this.chkLineOverrideUnaligned, 0, 2);
            this.panelLineOverrideIndex.Controls.Add(this.txtLineFormatText, 1, 1);
            this.panelLineOverrideIndex.Controls.Add(this.chkLineOverrideText, 0, 1);
            this.panelLineOverrideIndex.Controls.Add(this.chkLineOverrideIndex, 0, 0);
            this.panelLineOverrideIndex.Controls.Add(this.txtLineFormatIndex, 1, 0);
            this.panelLineOverrideIndex.Location = new System.Drawing.Point(81, 81);
            this.panelLineOverrideIndex.Margin = new System.Windows.Forms.Padding(0);
            this.panelLineOverrideIndex.Name = "panelLineOverrideIndex";
            this.panelLineOverrideIndex.RowCount = 3;
            this.panelLineOverrideIndex.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelLineOverrideIndex.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelLineOverrideIndex.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelLineOverrideIndex.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.panelLineOverrideIndex.Size = new System.Drawing.Size(306, 76);
            this.panelLineOverrideIndex.TabIndex = 5;
            // 
            // chkLineOverrideUnaligned
            // 
            this.chkLineOverrideUnaligned.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.chkLineOverrideUnaligned.AutoSize = true;
            this.chkLineOverrideUnaligned.Location = new System.Drawing.Point(16, 56);
            this.chkLineOverrideUnaligned.Name = "chkLineOverrideUnaligned";
            this.chkLineOverrideUnaligned.Size = new System.Drawing.Size(151, 17);
            this.chkLineOverrideUnaligned.TabIndex = 3;
            this.chkLineOverrideUnaligned.TabStop = true;
            this.chkLineOverrideUnaligned.Text = "chkLineOverrideUnaligned";
            this.chkLineOverrideUnaligned.UseVisualStyleBackColor = true;
            this.chkLineOverrideUnaligned.CheckedChanged += new System.EventHandler(this.chkLineOverride_CheckedChanged);
            // 
            // txtLineFormatText
            // 
            this.txtLineFormatText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLineFormatText.FormattingEnabled = true;
            this.txtLineFormatText.Location = new System.Drawing.Point(173, 29);
            this.txtLineFormatText.Name = "txtLineFormatText";
            this.txtLineFormatText.Size = new System.Drawing.Size(130, 21);
            this.txtLineFormatText.TabIndex = 1;
            this.txtLineFormatText.Text = "txtLineFormatText";
            this.txtLineFormatText.Visible = false;
            // 
            // chkLineOverrideText
            // 
            this.chkLineOverrideText.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.chkLineOverrideText.AutoSize = true;
            this.chkLineOverrideText.Location = new System.Drawing.Point(43, 31);
            this.chkLineOverrideText.Name = "chkLineOverrideText";
            this.chkLineOverrideText.Size = new System.Drawing.Size(124, 17);
            this.chkLineOverrideText.TabIndex = 0;
            this.chkLineOverrideText.TabStop = true;
            this.chkLineOverrideText.Text = "chkLineOverrideText";
            this.chkLineOverrideText.UseVisualStyleBackColor = true;
            this.chkLineOverrideText.Visible = false;
            // 
            // chkLineOverrideIndex
            // 
            this.chkLineOverrideIndex.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.chkLineOverrideIndex.AutoSize = true;
            this.chkLineOverrideIndex.Location = new System.Drawing.Point(38, 4);
            this.chkLineOverrideIndex.Name = "chkLineOverrideIndex";
            this.chkLineOverrideIndex.Size = new System.Drawing.Size(129, 17);
            this.chkLineOverrideIndex.TabIndex = 0;
            this.chkLineOverrideIndex.TabStop = true;
            this.chkLineOverrideIndex.Text = "chkLineOverrideIndex";
            this.chkLineOverrideIndex.UseVisualStyleBackColor = true;
            this.chkLineOverrideIndex.CheckedChanged += new System.EventHandler(this.chkLineOverride_CheckedChanged);
            // 
            // txtLineFormatIndex
            // 
            this.txtLineFormatIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLineFormatIndex.Location = new System.Drawing.Point(173, 3);
            this.txtLineFormatIndex.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtLineFormatIndex.Name = "txtLineFormatIndex";
            this.txtLineFormatIndex.Size = new System.Drawing.Size(130, 20);
            this.txtLineFormatIndex.TabIndex = 2;
            this.txtLineFormatIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtLineFormatIndex.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtLineFormatIndex.ValueChanged += new System.EventHandler(this.txtLineFormatIndex_ValueChanged);
            // 
            // lineOverridePreview
            // 
            this.lineOverridePreview.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lineOverridePreview.BackColor = System.Drawing.SystemColors.Window;
            this.lineOverridePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lineOverridePreview.Location = new System.Drawing.Point(3, 3);
            this.lineOverridePreview.Name = "lineOverridePreview";
            this.lineOverridePreview.Size = new System.Drawing.Size(75, 75);
            this.lineOverridePreview.Style = IndentGuide.LineStyle.Solid;
            this.lineOverridePreview.TabIndex = 4;
            // 
            // tableLineOverrideAddRemove
            // 
            this.tableLineOverrideAddRemove.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLineOverrideAddRemove.AutoSize = true;
            this.tableLineOverrideAddRemove.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLineOverrideAddRemove.ColumnCount = 2;
            this.tableLineOverrideAddRemove.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLineOverrideAddRemove.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLineOverrideAddRemove.Controls.Add(this.btnAddOverride, 0, 0);
            this.tableLineOverrideAddRemove.Controls.Add(this.btnRemoveOverride, 1, 0);
            this.tableLineOverrideAddRemove.Location = new System.Drawing.Point(0, 128);
            this.tableLineOverrideAddRemove.Margin = new System.Windows.Forms.Padding(0);
            this.tableLineOverrideAddRemove.Name = "tableLineOverrideAddRemove";
            this.tableLineOverrideAddRemove.RowCount = 1;
            this.tableLineOverrideAddRemove.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLineOverrideAddRemove.Size = new System.Drawing.Size(193, 29);
            this.tableLineOverrideAddRemove.TabIndex = 6;
            // 
            // btnAddOverride
            // 
            this.btnAddOverride.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAddOverride.AutoSize = true;
            this.btnAddOverride.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddOverride.Location = new System.Drawing.Point(3, 3);
            this.btnAddOverride.Name = "btnAddOverride";
            this.btnAddOverride.Size = new System.Drawing.Size(91, 23);
            this.btnAddOverride.TabIndex = 2;
            this.btnAddOverride.Text = "btnAddOverride";
            this.btnAddOverride.UseVisualStyleBackColor = true;
            this.btnAddOverride.Click += new System.EventHandler(this.btnAddOverride_Click);
            // 
            // btnRemoveOverride
            // 
            this.btnRemoveOverride.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRemoveOverride.AutoSize = true;
            this.btnRemoveOverride.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRemoveOverride.Enabled = false;
            this.btnRemoveOverride.Location = new System.Drawing.Point(100, 3);
            this.btnRemoveOverride.Name = "btnRemoveOverride";
            this.btnRemoveOverride.Size = new System.Drawing.Size(112, 23);
            this.btnRemoveOverride.TabIndex = 1;
            this.btnRemoveOverride.Text = "btnRemoveOverride";
            this.btnRemoveOverride.UseVisualStyleBackColor = true;
            this.btnRemoveOverride.Click += new System.EventHandler(this.btnRemoveOverride_Click);
            // 
            // DisplayOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DisplayOptionsControl";
            this.Size = new System.Drawing.Size(592, 317);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.grpLineStyle.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.grpLineOverrides.ResumeLayout(false);
            this.grpLineOverrides.PerformLayout();
            this.panelLineOverrides.ResumeLayout(false);
            this.panelLineOverrides.PerformLayout();
            this.panelLineOverrideFormat.ResumeLayout(false);
            this.panelLineOverrideFormat.PerformLayout();
            this.panelLineOverrideIndex.ResumeLayout(false);
            this.panelLineOverrideIndex.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLineFormatIndex)).EndInit();
            this.tableLineOverrideAddRemove.ResumeLayout(false);
            this.tableLineOverrideAddRemove.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox grpLineStyle;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private LinePreview linePreview;
        private System.Windows.Forms.PropertyGrid gridLineStyle;
        private System.Windows.Forms.GroupBox grpLineOverrides;
        private System.Windows.Forms.TableLayoutPanel panelLineOverrides;
        private System.Windows.Forms.ListBox lstOverrides;
        private System.Windows.Forms.Button btnAddOverride;
        private System.Windows.Forms.Button btnRemoveOverride;
        private System.Windows.Forms.TableLayoutPanel panelLineOverrideFormat;
        private System.Windows.Forms.PropertyGrid gridLineOverride;
        private System.Windows.Forms.TableLayoutPanel panelLineOverrideIndex;
        private System.Windows.Forms.RadioButton chkLineOverrideIndex;
        private System.Windows.Forms.ComboBox txtLineFormatText;
        private System.Windows.Forms.RadioButton chkLineOverrideText;
        private System.Windows.Forms.NumericUpDown txtLineFormatIndex;
        private LinePreview lineOverridePreview;
        private System.Windows.Forms.TableLayoutPanel tableLineOverrideAddRemove;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.RadioButton chkLineOverrideUnaligned;
    }
}
