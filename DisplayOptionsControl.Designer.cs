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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.grpLineStyle = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.gridLineStyle = new System.Windows.Forms.PropertyGrid();
            this.txtLineFormatIndex = new System.Windows.Forms.DomainUpDown();
            this.lblLineFormatIndex = new System.Windows.Forms.Label();
            this.btnResetLineFormat = new System.Windows.Forms.Button();
            this.grpLineBehavior = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.chkLineAbove = new System.Windows.Forms.RadioButton();
            this.chkLineBelow = new System.Windows.Forms.RadioButton();
            this.lblShowActualLogical = new System.Windows.Forms.Label();
            this.lblShowAboveBelow = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.chkLineActual = new System.Windows.Forms.RadioButton();
            this.chkLineLogical = new System.Windows.Forms.RadioButton();
            this.linePreview = new IndentGuide.LinePreview();
            this.tableLayoutPanel1.SuspendLayout();
            this.grpLineStyle.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.grpLineBehavior.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.grpLineStyle, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.grpLineBehavior, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(395, 317);
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
            this.grpLineStyle.Size = new System.Drawing.Size(389, 100);
            this.grpLineStyle.TabIndex = 4;
            this.grpLineStyle.TabStop = false;
            this.grpLineStyle.Text = "grpLineStyle";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.linePreview, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.gridLineStyle, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtLineFormatIndex, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblLineFormatIndex, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnResetLineFormat, 1, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(383, 81);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // gridLineStyle
            // 
            this.gridLineStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridLineStyle.CommandsVisibleIfAvailable = false;
            this.gridLineStyle.HelpVisible = false;
            this.gridLineStyle.LineColor = System.Drawing.SystemColors.Control;
            this.gridLineStyle.Location = new System.Drawing.Point(233, 3);
            this.gridLineStyle.Name = "gridLineStyle";
            this.gridLineStyle.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.tableLayoutPanel2.SetRowSpan(this.gridLineStyle, 3);
            this.gridLineStyle.Size = new System.Drawing.Size(147, 75);
            this.gridLineStyle.TabIndex = 4;
            this.gridLineStyle.ToolbarVisible = false;
            this.gridLineStyle.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.gridLineStyle_PropertyValueChanged);
            // 
            // txtLineFormatIndex
            // 
            this.txtLineFormatIndex.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtLineFormatIndex.AutoSize = true;
            this.txtLineFormatIndex.Items.Add("txtLineFormatIndex_Items");
            this.txtLineFormatIndex.Location = new System.Drawing.Point(84, 30);
            this.txtLineFormatIndex.Name = "txtLineFormatIndex";
            this.txtLineFormatIndex.Size = new System.Drawing.Size(143, 20);
            this.txtLineFormatIndex.TabIndex = 2;
            this.txtLineFormatIndex.Text = "txtLineFormatIndex";
            this.txtLineFormatIndex.SelectedItemChanged += new System.EventHandler(this.txtLineFormatIndex_SelectedItemChanged);
            // 
            // lblLineFormatIndex
            // 
            this.lblLineFormatIndex.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblLineFormatIndex.AutoSize = true;
            this.lblLineFormatIndex.Location = new System.Drawing.Point(108, 14);
            this.lblLineFormatIndex.Name = "lblLineFormatIndex";
            this.lblLineFormatIndex.Size = new System.Drawing.Size(95, 13);
            this.lblLineFormatIndex.TabIndex = 1;
            this.lblLineFormatIndex.Text = "lblLineFormatIndex";
            // 
            // btnResetLineFormat
            // 
            this.btnResetLineFormat.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnResetLineFormat.AutoSize = true;
            this.btnResetLineFormat.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnResetLineFormat.Location = new System.Drawing.Point(99, 57);
            this.btnResetLineFormat.Name = "btnResetLineFormat";
            this.btnResetLineFormat.Size = new System.Drawing.Size(112, 21);
            this.btnResetLineFormat.TabIndex = 3;
            this.btnResetLineFormat.Text = "btnResetLineFormat";
            this.btnResetLineFormat.UseVisualStyleBackColor = true;
            this.btnResetLineFormat.Click += new System.EventHandler(this.btnResetLineFormat_Click);
            // 
            // grpLineBehavior
            // 
            this.grpLineBehavior.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLineBehavior.AutoSize = true;
            this.grpLineBehavior.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpLineBehavior.Controls.Add(this.tableLayoutPanel3);
            this.grpLineBehavior.Location = new System.Drawing.Point(3, 109);
            this.grpLineBehavior.Name = "grpLineBehavior";
            this.grpLineBehavior.Size = new System.Drawing.Size(389, 71);
            this.grpLineBehavior.TabIndex = 5;
            this.grpLineBehavior.TabStop = false;
            this.grpLineBehavior.Text = "grpLineBehavior";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel2, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblShowActualLogical, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblShowAboveBelow, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(383, 52);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.chkLineAbove);
            this.flowLayoutPanel2.Controls.Add(this.chkLineBelow);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(336, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(100, 46);
            this.flowLayoutPanel2.TabIndex = 3;
            // 
            // chkLineAbove
            // 
            this.chkLineAbove.AutoSize = true;
            this.chkLineAbove.Location = new System.Drawing.Point(3, 3);
            this.chkLineAbove.Name = "chkLineAbove";
            this.chkLineAbove.Size = new System.Drawing.Size(94, 17);
            this.chkLineAbove.TabIndex = 0;
            this.chkLineAbove.TabStop = true;
            this.chkLineAbove.Text = "chkLineAbove";
            this.chkLineAbove.UseVisualStyleBackColor = true;
            this.chkLineAbove.CheckedChanged += new System.EventHandler(this.chkEmptyLineBehaviour_CheckedChanged);
            // 
            // chkLineBelow
            // 
            this.chkLineBelow.AutoSize = true;
            this.chkLineBelow.Location = new System.Drawing.Point(3, 26);
            this.chkLineBelow.Name = "chkLineBelow";
            this.chkLineBelow.Size = new System.Drawing.Size(92, 17);
            this.chkLineBelow.TabIndex = 1;
            this.chkLineBelow.TabStop = true;
            this.chkLineBelow.Text = "chkLineBelow";
            this.chkLineBelow.UseVisualStyleBackColor = true;
            this.chkLineBelow.CheckedChanged += new System.EventHandler(this.chkEmptyLineBehaviour_CheckedChanged);
            // 
            // lblShowActualLogical
            // 
            this.lblShowActualLogical.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblShowActualLogical.AutoSize = true;
            this.lblShowActualLogical.Location = new System.Drawing.Point(3, 19);
            this.lblShowActualLogical.Name = "lblShowActualLogical";
            this.lblShowActualLogical.Size = new System.Drawing.Size(108, 13);
            this.lblShowActualLogical.TabIndex = 0;
            this.lblShowActualLogical.Text = "lblShowActualLogical";
            // 
            // lblShowAboveBelow
            // 
            this.lblShowAboveBelow.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblShowAboveBelow.AutoSize = true;
            this.lblShowAboveBelow.Location = new System.Drawing.Point(226, 19);
            this.lblShowAboveBelow.Name = "lblShowAboveBelow";
            this.lblShowAboveBelow.Size = new System.Drawing.Size(104, 13);
            this.lblShowAboveBelow.TabIndex = 2;
            this.lblShowAboveBelow.Text = "lblShowAboveBelow";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.chkLineActual);
            this.flowLayoutPanel1.Controls.Add(this.chkLineLogical);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(117, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(103, 46);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // chkLineActual
            // 
            this.chkLineActual.AutoSize = true;
            this.chkLineActual.Location = new System.Drawing.Point(3, 3);
            this.chkLineActual.Name = "chkLineActual";
            this.chkLineActual.Size = new System.Drawing.Size(93, 17);
            this.chkLineActual.TabIndex = 0;
            this.chkLineActual.TabStop = true;
            this.chkLineActual.Text = "chkLineActual";
            this.chkLineActual.UseVisualStyleBackColor = true;
            this.chkLineActual.CheckedChanged += new System.EventHandler(this.chkEmptyLineBehaviour_CheckedChanged);
            // 
            // chkLineLogical
            // 
            this.chkLineLogical.AutoSize = true;
            this.chkLineLogical.Location = new System.Drawing.Point(3, 26);
            this.chkLineLogical.Name = "chkLineLogical";
            this.chkLineLogical.Size = new System.Drawing.Size(97, 17);
            this.chkLineLogical.TabIndex = 1;
            this.chkLineLogical.TabStop = true;
            this.chkLineLogical.Text = "chkLineLogical";
            this.chkLineLogical.UseVisualStyleBackColor = true;
            this.chkLineLogical.CheckedChanged += new System.EventHandler(this.chkEmptyLineBehaviour_CheckedChanged);
            // 
            // linePreview
            // 
            this.linePreview.BackColor = System.Drawing.SystemColors.Window;
            this.linePreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.linePreview.Location = new System.Drawing.Point(3, 3);
            this.linePreview.Name = "linePreview";
            this.tableLayoutPanel2.SetRowSpan(this.linePreview, 3);
            this.linePreview.Size = new System.Drawing.Size(75, 75);
            this.linePreview.Style = IndentGuide.LineStyle.Solid;
            this.linePreview.TabIndex = 0;
            // 
            // DisplayOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DisplayOptionsControl";
            this.Size = new System.Drawing.Size(395, 317);
            this.Load += new System.EventHandler(this.DisplayOptionsControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.grpLineStyle.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.grpLineBehavior.ResumeLayout(false);
            this.grpLineBehavior.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox grpLineStyle;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private LinePreview linePreview;
        private System.Windows.Forms.PropertyGrid gridLineStyle;
        private System.Windows.Forms.GroupBox grpLineBehavior;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label lblShowActualLogical;
        private System.Windows.Forms.Label lblShowAboveBelow;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.RadioButton chkLineAbove;
        private System.Windows.Forms.RadioButton chkLineBelow;
        private System.Windows.Forms.RadioButton chkLineActual;
        private System.Windows.Forms.RadioButton chkLineLogical;
        private System.Windows.Forms.DomainUpDown txtLineFormatIndex;
        private System.Windows.Forms.Label lblLineFormatIndex;
        private System.Windows.Forms.Button btnResetLineFormat;
    }
}
