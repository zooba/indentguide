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
            this.gridLineStyle = new System.Windows.Forms.PropertyGrid();
            this.linePreview = new IndentGuide.LinePreview();
            this.tableLayoutPanel1.SuspendLayout();
            this.grpLineStyle.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.cmbTheme, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnThemeSaveAs, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnThemeDelete, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.grpLineStyle, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(395, 317);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // cmbTheme
            // 
            this.cmbTheme.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTheme.FormattingEnabled = true;
            this.cmbTheme.Location = new System.Drawing.Point(3, 4);
            this.cmbTheme.Name = "cmbTheme";
            this.cmbTheme.Size = new System.Drawing.Size(179, 21);
            this.cmbTheme.TabIndex = 0;
            this.cmbTheme.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.cmbTheme_Format);
            // 
            // btnThemeSaveAs
            // 
            this.btnThemeSaveAs.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnThemeSaveAs.AutoSize = true;
            this.btnThemeSaveAs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnThemeSaveAs.Enabled = false;
            this.btnThemeSaveAs.Location = new System.Drawing.Point(188, 3);
            this.btnThemeSaveAs.Name = "btnThemeSaveAs";
            this.btnThemeSaveAs.Size = new System.Drawing.Size(102, 23);
            this.btnThemeSaveAs.TabIndex = 1;
            this.btnThemeSaveAs.Text = "btnThemeSaveAs";
            this.btnThemeSaveAs.UseVisualStyleBackColor = true;
            // 
            // btnThemeDelete
            // 
            this.btnThemeDelete.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnThemeDelete.AutoSize = true;
            this.btnThemeDelete.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnThemeDelete.Enabled = false;
            this.btnThemeDelete.Location = new System.Drawing.Point(296, 3);
            this.btnThemeDelete.Name = "btnThemeDelete";
            this.btnThemeDelete.Size = new System.Drawing.Size(96, 23);
            this.btnThemeDelete.TabIndex = 2;
            this.btnThemeDelete.Text = "btnThemeDelete";
            this.btnThemeDelete.UseVisualStyleBackColor = true;
            // 
            // grpLineStyle
            // 
            this.grpLineStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLineStyle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.grpLineStyle, 3);
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
            // gridLineStyle
            // 
            this.gridLineStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridLineStyle.CommandsVisibleIfAvailable = false;
            this.gridLineStyle.HelpVisible = false;
            this.gridLineStyle.LineColor = System.Drawing.Color.Transparent;
            this.gridLineStyle.Location = new System.Drawing.Point(84, 3);
            this.gridLineStyle.Name = "gridLineStyle";
            this.gridLineStyle.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.gridLineStyle.Size = new System.Drawing.Size(296, 75);
            this.gridLineStyle.TabIndex = 1;
            this.gridLineStyle.ToolbarVisible = false;
            this.gridLineStyle.ViewBackColor = System.Drawing.SystemColors.Control;
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
        internal System.Windows.Forms.PropertyGrid gridLineStyle;
    }
}
