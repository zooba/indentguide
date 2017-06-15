namespace IndentGuide
{
    partial class PageWidthOptionsControl
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
            this.lstLocations = new System.Windows.Forms.ListBox();
            this.linePreview = new IndentGuide.LinePreview();
            this.gridLineStyle = new System.Windows.Forms.PropertyGrid();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnAddLocation = new System.Windows.Forms.Button();
            this.btnRemoveLocation = new System.Windows.Forms.Button();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 53F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.tableLayoutPanel2.Controls.Add(this.linePreviewHighlight, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.lstLocations, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.linePreview, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.gridLineStyle, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 4);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(607, 390);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // linePreviewHighlight
            // 
            this.linePreviewHighlight.BackColor = System.Drawing.SystemColors.Window;
            this.linePreviewHighlight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linePreviewHighlight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linePreviewHighlight.Location = new System.Drawing.Point(188, 172);
            this.linePreviewHighlight.Margin = new System.Windows.Forms.Padding(4);
            this.linePreviewHighlight.Name = "linePreviewHighlight";
            this.linePreviewHighlight.Size = new System.Drawing.Size(45, 76);
            this.linePreviewHighlight.Style = IndentGuide.LineStyle.Solid;
            this.linePreviewHighlight.TabIndex = 5;
            // 
            // lstLocations
            // 
            this.lstLocations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstLocations.FormattingEnabled = true;
            this.lstLocations.IntegralHeight = false;
            this.lstLocations.ItemHeight = 16;
            this.lstLocations.Location = new System.Drawing.Point(4, 4);
            this.lstLocations.Margin = new System.Windows.Forms.Padding(4);
            this.lstLocations.Name = "lstLocations";
            this.tableLayoutPanel2.SetRowSpan(this.lstLocations, 4);
            this.lstLocations.Size = new System.Drawing.Size(176, 328);
            this.lstLocations.TabIndex = 0;
            this.lstLocations.SelectedIndexChanged += new System.EventHandler(this.lstLocations_SelectedIndexChanged);
            this.lstLocations.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.lstLocations_Format);
            // 
            // linePreview
            // 
            this.linePreview.BackColor = System.Drawing.SystemColors.Window;
            this.linePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linePreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linePreview.Location = new System.Drawing.Point(188, 88);
            this.linePreview.Margin = new System.Windows.Forms.Padding(4);
            this.linePreview.Name = "linePreview";
            this.linePreview.Size = new System.Drawing.Size(45, 76);
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
            this.gridLineStyle.Location = new System.Drawing.Point(241, 4);
            this.gridLineStyle.Margin = new System.Windows.Forms.Padding(4);
            this.gridLineStyle.Name = "gridLineStyle";
            this.gridLineStyle.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.tableLayoutPanel2.SetRowSpan(this.gridLineStyle, 4);
            this.gridLineStyle.Size = new System.Drawing.Size(362, 328);
            this.gridLineStyle.TabIndex = 4;
            this.gridLineStyle.ToolbarVisible = false;
            this.gridLineStyle.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.gridLineStyle_PropertyValueChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnAddLocation, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnRemoveLocation, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 339);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(178, 48);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // btnAddLocation
            // 
            this.btnAddLocation.AutoSize = true;
            this.btnAddLocation.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddLocation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddLocation.Font = new System.Drawing.Font("Segoe UI Symbol", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddLocation.Location = new System.Drawing.Point(20, 3);
            this.btnAddLocation.Name = "btnAddLocation";
            this.btnAddLocation.Padding = new System.Windows.Forms.Padding(3);
            this.btnAddLocation.Size = new System.Drawing.Size(57, 42);
            this.btnAddLocation.TabIndex = 1;
            this.btnAddLocation.Text = "&Add";
            this.btnAddLocation.UseVisualStyleBackColor = true;
            this.btnAddLocation.Click += new System.EventHandler(this.btnAddLocation_Click);
            // 
            // btnRemoveLocation
            // 
            this.btnRemoveLocation.AutoSize = true;
            this.btnRemoveLocation.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRemoveLocation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRemoveLocation.Font = new System.Drawing.Font("Segoe UI Symbol", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemoveLocation.Location = new System.Drawing.Point(83, 3);
            this.btnRemoveLocation.Name = "btnRemoveLocation";
            this.btnRemoveLocation.Padding = new System.Windows.Forms.Padding(3);
            this.btnRemoveLocation.Size = new System.Drawing.Size(75, 42);
            this.btnRemoveLocation.TabIndex = 0;
            this.btnRemoveLocation.Text = "&Delete";
            this.btnRemoveLocation.UseVisualStyleBackColor = true;
            this.btnRemoveLocation.Click += new System.EventHandler(this.btnRemoveLocation_Click);
            // 
            // PageWidthOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PageWidthOptionsControl";
            this.Size = new System.Drawing.Size(607, 390);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private LinePreview linePreview;
        private System.Windows.Forms.PropertyGrid gridLineStyle;
        private LinePreview linePreviewHighlight;
        private System.Windows.Forms.ListBox lstLocations;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnAddLocation;
        private System.Windows.Forms.Button btnRemoveLocation;
    }
}
