namespace IndentGuide
{
    partial class BehaviorOptionsControl
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
            this.tableLineBehavior = new System.Windows.Forms.TableLayoutPanel();
            this.chkNoGuides = new System.Windows.Forms.RadioButton();
            this.chkSameAsAboveActual = new System.Windows.Forms.RadioButton();
            this.chkSameAsAboveLogical = new System.Windows.Forms.RadioButton();
            this.chkSameAsBelowActual = new System.Windows.Forms.RadioButton();
            this.chkSameAsBelowLogical = new System.Windows.Forms.RadioButton();
            this.grpLineBehavior = new System.Windows.Forms.GroupBox();
            this.grpEndOfLineBehavior = new System.Windows.Forms.GroupBox();
            this.tableEndOfLineBehavior = new System.Windows.Forms.TableLayoutPanel();
            this.chkEndOfLineVisible = new System.Windows.Forms.RadioButton();
            this.chkEndOfLineHidden = new System.Windows.Forms.RadioButton();
            this.lineTextPreview = new IndentGuide.LineTextPreview();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLineBehavior.SuspendLayout();
            this.grpLineBehavior.SuspendLayout();
            this.grpEndOfLineBehavior.SuspendLayout();
            this.tableEndOfLineBehavior.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.Controls.Add(this.lineTextPreview, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.grpLineBehavior, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.grpEndOfLineBehavior, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(563, 375);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // tableLineBehavior
            // 
            this.tableLineBehavior.AutoSize = true;
            this.tableLineBehavior.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLineBehavior.ColumnCount = 1;
            this.tableLineBehavior.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLineBehavior.Controls.Add(this.chkNoGuides, 0, 0);
            this.tableLineBehavior.Controls.Add(this.chkSameAsAboveActual, 0, 1);
            this.tableLineBehavior.Controls.Add(this.chkSameAsAboveLogical, 0, 2);
            this.tableLineBehavior.Controls.Add(this.chkSameAsBelowActual, 0, 3);
            this.tableLineBehavior.Controls.Add(this.chkSameAsBelowLogical, 0, 4);
            this.tableLineBehavior.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLineBehavior.Location = new System.Drawing.Point(3, 16);
            this.tableLineBehavior.Name = "tableLineBehavior";
            this.tableLineBehavior.RowCount = 5;
            this.tableLineBehavior.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLineBehavior.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLineBehavior.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLineBehavior.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLineBehavior.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLineBehavior.Size = new System.Drawing.Size(153, 90);
            this.tableLineBehavior.TabIndex = 1;
            // 
            // chkNoGuides
            // 
            this.chkNoGuides.AutoSize = true;
            this.chkNoGuides.Location = new System.Drawing.Point(3, 1);
            this.chkNoGuides.Margin = new System.Windows.Forms.Padding(3, 1, 3, 0);
            this.chkNoGuides.Name = "chkNoGuides";
            this.chkNoGuides.Size = new System.Drawing.Size(90, 17);
            this.chkNoGuides.TabIndex = 0;
            this.chkNoGuides.TabStop = true;
            this.chkNoGuides.Text = "chkNoGuides";
            this.chkNoGuides.UseVisualStyleBackColor = true;
            this.chkNoGuides.CheckedChanged += new System.EventHandler(this.chkLineBehavior_CheckedChanged);
            // 
            // chkSameAsAboveActual
            // 
            this.chkSameAsAboveActual.AutoSize = true;
            this.chkSameAsAboveActual.Location = new System.Drawing.Point(3, 19);
            this.chkSameAsAboveActual.Margin = new System.Windows.Forms.Padding(3, 1, 3, 0);
            this.chkSameAsAboveActual.Name = "chkSameAsAboveActual";
            this.chkSameAsAboveActual.Size = new System.Drawing.Size(143, 17);
            this.chkSameAsAboveActual.TabIndex = 1;
            this.chkSameAsAboveActual.TabStop = true;
            this.chkSameAsAboveActual.Text = "chkSameAsAboveActual";
            this.chkSameAsAboveActual.UseVisualStyleBackColor = true;
            this.chkSameAsAboveActual.CheckedChanged += new System.EventHandler(this.chkLineBehavior_CheckedChanged);
            // 
            // chkSameAsAboveLogical
            // 
            this.chkSameAsAboveLogical.AutoSize = true;
            this.chkSameAsAboveLogical.Location = new System.Drawing.Point(3, 37);
            this.chkSameAsAboveLogical.Margin = new System.Windows.Forms.Padding(3, 1, 3, 0);
            this.chkSameAsAboveLogical.Name = "chkSameAsAboveLogical";
            this.chkSameAsAboveLogical.Size = new System.Drawing.Size(147, 17);
            this.chkSameAsAboveLogical.TabIndex = 2;
            this.chkSameAsAboveLogical.TabStop = true;
            this.chkSameAsAboveLogical.Text = "chkSameAsAboveLogical";
            this.chkSameAsAboveLogical.UseVisualStyleBackColor = true;
            this.chkSameAsAboveLogical.CheckedChanged += new System.EventHandler(this.chkLineBehavior_CheckedChanged);
            // 
            // chkSameAsBelowActual
            // 
            this.chkSameAsBelowActual.AutoSize = true;
            this.chkSameAsBelowActual.Location = new System.Drawing.Point(3, 55);
            this.chkSameAsBelowActual.Margin = new System.Windows.Forms.Padding(3, 1, 3, 0);
            this.chkSameAsBelowActual.Name = "chkSameAsBelowActual";
            this.chkSameAsBelowActual.Size = new System.Drawing.Size(141, 17);
            this.chkSameAsBelowActual.TabIndex = 3;
            this.chkSameAsBelowActual.TabStop = true;
            this.chkSameAsBelowActual.Text = "chkSameAsBelowActual";
            this.chkSameAsBelowActual.UseVisualStyleBackColor = true;
            this.chkSameAsBelowActual.CheckedChanged += new System.EventHandler(this.chkLineBehavior_CheckedChanged);
            // 
            // chkSameAsBelowLogical
            // 
            this.chkSameAsBelowLogical.AutoSize = true;
            this.chkSameAsBelowLogical.Location = new System.Drawing.Point(3, 73);
            this.chkSameAsBelowLogical.Margin = new System.Windows.Forms.Padding(3, 1, 3, 0);
            this.chkSameAsBelowLogical.Name = "chkSameAsBelowLogical";
            this.chkSameAsBelowLogical.Size = new System.Drawing.Size(145, 17);
            this.chkSameAsBelowLogical.TabIndex = 4;
            this.chkSameAsBelowLogical.TabStop = true;
            this.chkSameAsBelowLogical.Text = "chkSameAsBelowLogical";
            this.chkSameAsBelowLogical.UseVisualStyleBackColor = true;
            this.chkSameAsBelowLogical.CheckedChanged += new System.EventHandler(this.chkLineBehavior_CheckedChanged);
            // 
            // grpLineBehavior
            // 
            this.grpLineBehavior.AutoSize = true;
            this.grpLineBehavior.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpLineBehavior.Controls.Add(this.tableLineBehavior);
            this.grpLineBehavior.Location = new System.Drawing.Point(3, 3);
            this.grpLineBehavior.Name = "grpLineBehavior";
            this.grpLineBehavior.Padding = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.grpLineBehavior.Size = new System.Drawing.Size(159, 112);
            this.grpLineBehavior.TabIndex = 2;
            this.grpLineBehavior.TabStop = false;
            this.grpLineBehavior.Text = "grpLineBehavior";
            // 
            // grpEndOfLineBehavior
            // 
            this.grpEndOfLineBehavior.AutoSize = true;
            this.grpEndOfLineBehavior.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpEndOfLineBehavior.Controls.Add(this.tableEndOfLineBehavior);
            this.grpEndOfLineBehavior.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpEndOfLineBehavior.Location = new System.Drawing.Point(3, 121);
            this.grpEndOfLineBehavior.Name = "grpEndOfLineBehavior";
            this.grpEndOfLineBehavior.Padding = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.grpEndOfLineBehavior.Size = new System.Drawing.Size(159, 58);
            this.grpEndOfLineBehavior.TabIndex = 3;
            this.grpEndOfLineBehavior.TabStop = false;
            this.grpEndOfLineBehavior.Text = "grpEndOfLineBehavior";
            // 
            // tableEndOfLineBehavior
            // 
            this.tableEndOfLineBehavior.AutoSize = true;
            this.tableEndOfLineBehavior.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableEndOfLineBehavior.ColumnCount = 1;
            this.tableEndOfLineBehavior.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableEndOfLineBehavior.Controls.Add(this.chkEndOfLineVisible, 0, 0);
            this.tableEndOfLineBehavior.Controls.Add(this.chkEndOfLineHidden, 0, 1);
            this.tableEndOfLineBehavior.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableEndOfLineBehavior.Location = new System.Drawing.Point(3, 16);
            this.tableEndOfLineBehavior.Name = "tableEndOfLineBehavior";
            this.tableEndOfLineBehavior.RowCount = 2;
            this.tableEndOfLineBehavior.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableEndOfLineBehavior.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableEndOfLineBehavior.Size = new System.Drawing.Size(153, 36);
            this.tableEndOfLineBehavior.TabIndex = 0;
            // 
            // chkEndOfLineVisible
            // 
            this.chkEndOfLineVisible.AutoSize = true;
            this.chkEndOfLineVisible.Location = new System.Drawing.Point(3, 1);
            this.chkEndOfLineVisible.Margin = new System.Windows.Forms.Padding(3, 1, 3, 0);
            this.chkEndOfLineVisible.Name = "chkEndOfLineVisible";
            this.chkEndOfLineVisible.Size = new System.Drawing.Size(123, 17);
            this.chkEndOfLineVisible.TabIndex = 0;
            this.chkEndOfLineVisible.TabStop = true;
            this.chkEndOfLineVisible.Text = "chkEndOfLineVisible";
            this.chkEndOfLineVisible.UseVisualStyleBackColor = true;
            this.chkEndOfLineVisible.CheckedChanged += new System.EventHandler(this.chkEndOfLine_CheckedChanged);
            // 
            // chkEndOfLineHidden
            // 
            this.chkEndOfLineHidden.AutoSize = true;
            this.chkEndOfLineHidden.Location = new System.Drawing.Point(3, 19);
            this.chkEndOfLineHidden.Margin = new System.Windows.Forms.Padding(3, 1, 3, 0);
            this.chkEndOfLineHidden.Name = "chkEndOfLineHidden";
            this.chkEndOfLineHidden.Size = new System.Drawing.Size(127, 17);
            this.chkEndOfLineHidden.TabIndex = 1;
            this.chkEndOfLineHidden.TabStop = true;
            this.chkEndOfLineHidden.Text = "chkEndOfLineHidden";
            this.chkEndOfLineHidden.UseVisualStyleBackColor = true;
            this.chkEndOfLineHidden.CheckedChanged += new System.EventHandler(this.chkEndOfLine_CheckedChanged);
            // 
            // lineTextPreview
            // 
            this.lineTextPreview.BackColor = System.Drawing.SystemColors.Window;
            this.lineTextPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lineTextPreview.EmptyLineMode = IndentGuide.EmptyLineMode.NoGuides;
            this.lineTextPreview.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lineTextPreview.ForeColor = System.Drawing.Color.Teal;
            this.lineTextPreview.Location = new System.Drawing.Point(169, 4);
            this.lineTextPreview.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lineTextPreview.Name = "lineTextPreview";
            this.tableLayoutPanel1.SetRowSpan(this.lineTextPreview, 3);
            this.lineTextPreview.ShowAtText = false;
            this.lineTextPreview.Size = new System.Drawing.Size(390, 275);
            this.lineTextPreview.Style = IndentGuide.LineStyle.Dotted;
            this.lineTextPreview.TabIndex = 0;
            this.lineTextPreview.TabSize = 4;
            this.lineTextPreview.Text = "lineTextPreview";
            // 
            // BehaviorOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "BehaviorOptionsControl";
            this.Size = new System.Drawing.Size(563, 375);
            this.Load += new System.EventHandler(this.BehaviorOptionsControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLineBehavior.ResumeLayout(false);
            this.tableLineBehavior.PerformLayout();
            this.grpLineBehavior.ResumeLayout(false);
            this.grpLineBehavior.PerformLayout();
            this.grpEndOfLineBehavior.ResumeLayout(false);
            this.grpEndOfLineBehavior.PerformLayout();
            this.tableEndOfLineBehavior.ResumeLayout(false);
            this.tableEndOfLineBehavior.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private LineTextPreview lineTextPreview;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLineBehavior;
        private System.Windows.Forms.RadioButton chkNoGuides;
        private System.Windows.Forms.RadioButton chkSameAsAboveActual;
        private System.Windows.Forms.RadioButton chkSameAsAboveLogical;
        private System.Windows.Forms.RadioButton chkSameAsBelowActual;
        private System.Windows.Forms.RadioButton chkSameAsBelowLogical;
        private System.Windows.Forms.GroupBox grpLineBehavior;
        private System.Windows.Forms.GroupBox grpEndOfLineBehavior;
        private System.Windows.Forms.TableLayoutPanel tableEndOfLineBehavior;
        private System.Windows.Forms.RadioButton chkEndOfLineVisible;
        private System.Windows.Forms.RadioButton chkEndOfLineHidden;
    }
}
