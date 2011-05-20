namespace IndentGuide
{
    partial class ThemeOptionsControl
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
            this.tableContent = new System.Windows.Forms.TableLayoutPanel();
            this.tableTheme = new System.Windows.Forms.TableLayoutPanel();
            this.cmbTheme = new System.Windows.Forms.ComboBox();
            this.btnThemeDelete = new System.Windows.Forms.Button();
            this.lblContentType = new System.Windows.Forms.Label();
            this.btnThemeSaveAs = new System.Windows.Forms.Button();
            this.btnCustomizeThisContentType = new System.Windows.Forms.Button();
            this.tableContent.SuspendLayout();
            this.tableTheme.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableContent
            // 
            this.tableContent.ColumnCount = 1;
            this.tableContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableContent.Controls.Add(this.tableTheme, 0, 0);
            this.tableContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableContent.Location = new System.Drawing.Point(0, 0);
            this.tableContent.Name = "tableContent";
            this.tableContent.RowCount = 2;
            this.tableContent.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableContent.Size = new System.Drawing.Size(592, 352);
            this.tableContent.TabIndex = 0;
            // 
            // tableTheme
            // 
            this.tableTheme.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableTheme.AutoSize = true;
            this.tableTheme.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableTheme.ColumnCount = 5;
            this.tableTheme.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableTheme.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableTheme.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableTheme.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableTheme.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableTheme.Controls.Add(this.cmbTheme, 0, 0);
            this.tableTheme.Controls.Add(this.lblContentType, 0, 0);
            this.tableTheme.Controls.Add(this.btnThemeSaveAs, 3, 0);
            this.tableTheme.Controls.Add(this.btnThemeDelete, 4, 0);
            this.tableTheme.Controls.Add(this.btnCustomizeThisContentType, 2, 0);
            this.tableTheme.Location = new System.Drawing.Point(0, 0);
            this.tableTheme.Margin = new System.Windows.Forms.Padding(0);
            this.tableTheme.Name = "tableTheme";
            this.tableTheme.RowCount = 1;
            this.tableTheme.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableTheme.Size = new System.Drawing.Size(592, 29);
            this.tableTheme.TabIndex = 0;
            // 
            // cmbTheme
            // 
            this.cmbTheme.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTheme.FormattingEnabled = true;
            this.cmbTheme.Location = new System.Drawing.Point(87, 4);
            this.cmbTheme.Name = "cmbTheme";
            this.cmbTheme.Size = new System.Drawing.Size(125, 21);
            this.cmbTheme.TabIndex = 1;
            this.cmbTheme.SelectedIndexChanged += new System.EventHandler(this.cmbTheme_SelectedIndexChanged);
            this.cmbTheme.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.cmbTheme_Format);
            this.cmbTheme.TextChanged += new System.EventHandler(this.cmbTheme_TextChanged);
            // 
            // btnThemeDelete
            // 
            this.btnThemeDelete.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnThemeDelete.AutoSize = true;
            this.btnThemeDelete.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnThemeDelete.Location = new System.Drawing.Point(493, 3);
            this.btnThemeDelete.Name = "btnThemeDelete";
            this.btnThemeDelete.Size = new System.Drawing.Size(96, 23);
            this.btnThemeDelete.TabIndex = 4;
            this.btnThemeDelete.Text = "btnThemeDelete";
            this.btnThemeDelete.UseVisualStyleBackColor = true;
            this.btnThemeDelete.Click += new System.EventHandler(this.btnThemeDelete_Click);
            // 
            // lblContentType
            // 
            this.lblContentType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblContentType.AutoSize = true;
            this.lblContentType.Location = new System.Drawing.Point(3, 8);
            this.lblContentType.Name = "lblContentType";
            this.lblContentType.Size = new System.Drawing.Size(78, 13);
            this.lblContentType.TabIndex = 0;
            this.lblContentType.Text = "lblContentType";
            // 
            // btnThemeSaveAs
            // 
            this.btnThemeSaveAs.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnThemeSaveAs.AutoSize = true;
            this.btnThemeSaveAs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnThemeSaveAs.Location = new System.Drawing.Point(385, 3);
            this.btnThemeSaveAs.Name = "btnThemeSaveAs";
            this.btnThemeSaveAs.Size = new System.Drawing.Size(102, 23);
            this.btnThemeSaveAs.TabIndex = 3;
            this.btnThemeSaveAs.Text = "btnThemeSaveAs";
            this.btnThemeSaveAs.UseVisualStyleBackColor = true;
            this.btnThemeSaveAs.Click += new System.EventHandler(this.btnThemeSaveAs_Click);
            // 
            // btnCustomizeThisContentType
            // 
            this.btnCustomizeThisContentType.AutoSize = true;
            this.btnCustomizeThisContentType.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCustomizeThisContentType.Location = new System.Drawing.Point(218, 3);
            this.btnCustomizeThisContentType.Name = "btnCustomizeThisContentType";
            this.btnCustomizeThisContentType.Size = new System.Drawing.Size(161, 23);
            this.btnCustomizeThisContentType.TabIndex = 2;
            this.btnCustomizeThisContentType.Text = "btnCustomizeThisContentType";
            this.btnCustomizeThisContentType.UseVisualStyleBackColor = true;
            this.btnCustomizeThisContentType.Click += new System.EventHandler(this.btnCustomizeThisContentType_Click);
            // 
            // ThemeOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableContent);
            this.Name = "ThemeOptionsControl";
            this.Size = new System.Drawing.Size(592, 352);
            this.Load += new System.EventHandler(this.ThemeOptionsControl_Load);
            this.tableContent.ResumeLayout(false);
            this.tableContent.PerformLayout();
            this.tableTheme.ResumeLayout(false);
            this.tableTheme.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableContent;
        private System.Windows.Forms.TableLayoutPanel tableTheme;
        private System.Windows.Forms.ComboBox cmbTheme;
        private System.Windows.Forms.Button btnThemeDelete;
        private System.Windows.Forms.Label lblContentType;
        private System.Windows.Forms.Button btnThemeSaveAs;
        private System.Windows.Forms.Button btnCustomizeThisContentType;
    }
}
