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
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.linePreviewHighlight = new IndentGuide.LinePreview();
            this.lstOverrides = new System.Windows.Forms.ListBox();
            this.linePreview = new IndentGuide.LinePreview();
            this.gridLineStyle = new System.Windows.Forms.PropertyGrid();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.tableLayoutPanel2.Controls.Add(this.linePreviewHighlight, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.lstOverrides, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.linePreview, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.gridLineStyle, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(455, 317);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // linePreviewHighlight
            // 
            this.linePreviewHighlight.BackColor = System.Drawing.SystemColors.Window;
            this.linePreviewHighlight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linePreviewHighlight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linePreviewHighlight.Location = new System.Drawing.Point(141, 161);
            this.linePreviewHighlight.Name = "linePreviewHighlight";
            this.linePreviewHighlight.Size = new System.Drawing.Size(34, 73);
            this.linePreviewHighlight.Style = IndentGuide.LineStyle.Solid;
            this.linePreviewHighlight.TabIndex = 5;
            // 
            // lstOverrides
            // 
            this.lstOverrides.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstOverrides.FormattingEnabled = true;
            this.lstOverrides.IntegralHeight = false;
            this.lstOverrides.Location = new System.Drawing.Point(3, 3);
            this.lstOverrides.Name = "lstOverrides";
            this.tableLayoutPanel2.SetRowSpan(this.lstOverrides, 4);
            this.lstOverrides.Size = new System.Drawing.Size(132, 311);
            this.lstOverrides.TabIndex = 0;
            this.lstOverrides.SelectedIndexChanged += new System.EventHandler(this.lstOverrides_SelectedIndexChanged);
            this.lstOverrides.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.lstOverrides_Format);
            // 
            // linePreview
            // 
            this.linePreview.BackColor = System.Drawing.SystemColors.Window;
            this.linePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linePreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linePreview.Location = new System.Drawing.Point(141, 82);
            this.linePreview.Name = "linePreview";
            this.linePreview.Size = new System.Drawing.Size(34, 73);
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
            this.gridLineStyle.Location = new System.Drawing.Point(181, 3);
            this.gridLineStyle.Name = "gridLineStyle";
            this.gridLineStyle.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.tableLayoutPanel2.SetRowSpan(this.gridLineStyle, 4);
            this.gridLineStyle.Size = new System.Drawing.Size(271, 311);
            this.gridLineStyle.TabIndex = 4;
            this.gridLineStyle.ToolbarVisible = false;
            this.gridLineStyle.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.gridLineStyle_PropertyValueChanged);
            // 
            // DisplayOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "DisplayOptionsControl";
            this.Size = new System.Drawing.Size(455, 317);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ListBox lstOverrides;
        private LinePreview linePreview;
        private System.Windows.Forms.PropertyGrid gridLineStyle;
        private LinePreview linePreviewHighlight;
    }
}
