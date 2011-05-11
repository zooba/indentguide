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
            this.cmbTheme = new System.Windows.Forms.ComboBox();
            this.btnThemeSaveAs = new System.Windows.Forms.Button();
            this.btnThemeDelete = new System.Windows.Forms.Button();
            this.grpLineStyle = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.linePreview = new IndentGuide.LinePreview();
            this.gridLineStyle = new System.Windows.Forms.PropertyGrid();
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
            this.lblContentType = new System.Windows.Forms.Label();
            this.grpCurrentContentType = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.lblCurrentContentTypeIs = new System.Windows.Forms.Label();
            this.lblCurrentContentType = new System.Windows.Forms.Label();
            this.btnCustomizeThisContentType = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.grpLineStyle.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.grpLineBehavior.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.grpCurrentContentType.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.cmbTheme, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnThemeSaveAs, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnThemeDelete, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.grpLineStyle, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.grpLineBehavior, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblContentType, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.grpCurrentContentType, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(395, 317);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // cmbTheme
            // 
            this.cmbTheme.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTheme.FormattingEnabled = true;
            this.cmbTheme.Location = new System.Drawing.Point(87, 4);
            this.cmbTheme.Name = "cmbTheme";
            this.cmbTheme.Size = new System.Drawing.Size(95, 21);
            this.cmbTheme.TabIndex = 0;
            this.cmbTheme.SelectedIndexChanged += new System.EventHandler(this.cmbTheme_SelectedIndexChanged);
            this.cmbTheme.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.cmbTheme_Format);
            this.cmbTheme.TextChanged += new System.EventHandler(this.cmbTheme_TextChanged);
            // 
            // btnThemeSaveAs
            // 
            this.btnThemeSaveAs.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnThemeSaveAs.AutoSize = true;
            this.btnThemeSaveAs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnThemeSaveAs.Location = new System.Drawing.Point(188, 3);
            this.btnThemeSaveAs.Name = "btnThemeSaveAs";
            this.btnThemeSaveAs.Size = new System.Drawing.Size(102, 23);
            this.btnThemeSaveAs.TabIndex = 1;
            this.btnThemeSaveAs.Text = "btnThemeSaveAs";
            this.btnThemeSaveAs.UseVisualStyleBackColor = true;
            this.btnThemeSaveAs.Click += new System.EventHandler(this.btnThemeSaveAs_Click);
            // 
            // btnThemeDelete
            // 
            this.btnThemeDelete.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnThemeDelete.AutoSize = true;
            this.btnThemeDelete.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnThemeDelete.Location = new System.Drawing.Point(296, 3);
            this.btnThemeDelete.Name = "btnThemeDelete";
            this.btnThemeDelete.Size = new System.Drawing.Size(96, 23);
            this.btnThemeDelete.TabIndex = 2;
            this.btnThemeDelete.Text = "btnThemeDelete";
            this.btnThemeDelete.UseVisualStyleBackColor = true;
            this.btnThemeDelete.Click += new System.EventHandler(this.btnThemeDelete_Click);
            // 
            // grpLineStyle
            // 
            this.grpLineStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLineStyle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.grpLineStyle, 4);
            this.grpLineStyle.Controls.Add(this.tableLayoutPanel2);
            this.grpLineStyle.Location = new System.Drawing.Point(3, 32);
            this.grpLineStyle.Name = "grpLineStyle";
            this.grpLineStyle.Size = new System.Drawing.Size(389, 100);
            this.grpLineStyle.TabIndex = 3;
            this.grpLineStyle.TabStop = false;
            this.grpLineStyle.Text = "grpLineStyle";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.linePreview, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.gridLineStyle, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(383, 81);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // linePreview
            // 
            this.linePreview.BackColor = System.Drawing.SystemColors.Window;
            this.linePreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.linePreview.Location = new System.Drawing.Point(3, 3);
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
            this.gridLineStyle.LineColor = System.Drawing.SystemColors.Control;
            this.gridLineStyle.Location = new System.Drawing.Point(84, 3);
            this.gridLineStyle.Name = "gridLineStyle";
            this.gridLineStyle.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.gridLineStyle.Size = new System.Drawing.Size(296, 75);
            this.gridLineStyle.TabIndex = 1;
            this.gridLineStyle.ToolbarVisible = false;
            this.gridLineStyle.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.gridLineStyle_PropertyValueChanged);
            // 
            // grpLineBehavior
            // 
            this.grpLineBehavior.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLineBehavior.AutoSize = true;
            this.grpLineBehavior.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.grpLineBehavior, 4);
            this.grpLineBehavior.Controls.Add(this.tableLayoutPanel3);
            this.grpLineBehavior.Location = new System.Drawing.Point(3, 138);
            this.grpLineBehavior.Name = "grpLineBehavior";
            this.grpLineBehavior.Size = new System.Drawing.Size(389, 71);
            this.grpLineBehavior.TabIndex = 4;
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
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // chkLineAbove
            // 
            this.chkLineAbove.AutoSize = true;
            this.chkLineAbove.Location = new System.Drawing.Point(3, 3);
            this.chkLineAbove.Name = "chkLineAbove";
            this.chkLineAbove.Size = new System.Drawing.Size(94, 17);
            this.chkLineAbove.TabIndex = 2;
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
            this.chkLineBelow.TabIndex = 3;
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
            this.flowLayoutPanel1.TabIndex = 3;
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
            // lblContentType
            // 
            this.lblContentType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblContentType.AutoSize = true;
            this.lblContentType.Location = new System.Drawing.Point(3, 8);
            this.lblContentType.Name = "lblContentType";
            this.lblContentType.Size = new System.Drawing.Size(78, 13);
            this.lblContentType.TabIndex = 5;
            this.lblContentType.Text = "lblContentType";
            // 
            // grpCurrentContentType
            // 
            this.grpCurrentContentType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.grpCurrentContentType, 4);
            this.grpCurrentContentType.Controls.Add(this.tableLayoutPanel4);
            this.grpCurrentContentType.Location = new System.Drawing.Point(3, 215);
            this.grpCurrentContentType.Name = "grpCurrentContentType";
            this.grpCurrentContentType.Size = new System.Drawing.Size(389, 99);
            this.grpCurrentContentType.TabIndex = 6;
            this.grpCurrentContentType.TabStop = false;
            this.grpCurrentContentType.Text = "grpCurrentContentType";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel4.Controls.Add(this.lblCurrentContentTypeIs, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.lblCurrentContentType, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnCustomizeThisContentType, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(383, 80);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // lblCurrentContentTypeIs
            // 
            this.lblCurrentContentTypeIs.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblCurrentContentTypeIs.AutoSize = true;
            this.lblCurrentContentTypeIs.Location = new System.Drawing.Point(145, 16);
            this.lblCurrentContentTypeIs.Name = "lblCurrentContentTypeIs";
            this.lblCurrentContentTypeIs.Size = new System.Drawing.Size(120, 13);
            this.lblCurrentContentTypeIs.TabIndex = 0;
            this.lblCurrentContentTypeIs.Text = "lblCurrentContentTypeIs";
            // 
            // lblCurrentContentType
            // 
            this.lblCurrentContentType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCurrentContentType.AutoSize = true;
            this.lblCurrentContentType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentContentType.Location = new System.Drawing.Point(271, 9);
            this.lblCurrentContentType.Name = "lblCurrentContentType";
            this.lblCurrentContentType.Size = new System.Drawing.Size(105, 26);
            this.lblCurrentContentType.TabIndex = 1;
            this.lblCurrentContentType.Text = "lblCurrentContentType";
            // 
            // btnCustomizeThisContentType
            // 
            this.btnCustomizeThisContentType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCustomizeThisContentType.AutoSize = true;
            this.btnCustomizeThisContentType.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel4.SetColumnSpan(this.btnCustomizeThisContentType, 2);
            this.btnCustomizeThisContentType.Location = new System.Drawing.Point(108, 48);
            this.btnCustomizeThisContentType.Name = "btnCustomizeThisContentType";
            this.btnCustomizeThisContentType.Padding = new System.Windows.Forms.Padding(3);
            this.btnCustomizeThisContentType.Size = new System.Drawing.Size(167, 29);
            this.btnCustomizeThisContentType.TabIndex = 2;
            this.btnCustomizeThisContentType.Text = "btnCustomizeThisContentType";
            this.btnCustomizeThisContentType.UseVisualStyleBackColor = true;
            this.btnCustomizeThisContentType.Click += new System.EventHandler(this.btnCustomizeThisContentType_Click);
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
            this.grpLineBehavior.ResumeLayout(false);
            this.grpLineBehavior.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.grpCurrentContentType.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox cmbTheme;
        private System.Windows.Forms.Button btnThemeSaveAs;
        private System.Windows.Forms.Button btnThemeDelete;
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
        private System.Windows.Forms.Label lblContentType;
        private System.Windows.Forms.GroupBox grpCurrentContentType;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label lblCurrentContentTypeIs;
        private System.Windows.Forms.Label lblCurrentContentType;
        private System.Windows.Forms.Button btnCustomizeThisContentType;
    }
}
