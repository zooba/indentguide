namespace IndentGuide {
    partial class CustomBehaviorOptionsControl {
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
            this.panelLineOverrides = new System.Windows.Forms.TableLayoutPanel();
            this.lineTextPreview = new IndentGuide.LineTextPreview();
            this.gridLineMode = new IndentGuide.CheckedPropertyGrid();
            this.panelLineOverrides.SuspendLayout();
            this.SuspendLayout();
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
            this.panelLineOverrides.Location = new System.Drawing.Point(0, 0);
            this.panelLineOverrides.Name = "panelLineOverrides";
            this.panelLineOverrides.RowCount = 1;
            this.panelLineOverrides.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelLineOverrides.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 272F));
            this.panelLineOverrides.Size = new System.Drawing.Size(473, 326);
            this.panelLineOverrides.TabIndex = 0;
            // 
            // lineTextPreview
            // 
            this.lineTextPreview.BackColor = System.Drawing.SystemColors.Window;
            this.lineTextPreview.Checked = false;
            this.lineTextPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lineTextPreview.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lineTextPreview.HighlightBackColor = System.Drawing.Color.Empty;
            this.lineTextPreview.HighlightForeColor = System.Drawing.Color.Empty;
            this.lineTextPreview.IndentSize = 4;
            this.lineTextPreview.Location = new System.Drawing.Point(240, 4);
            this.lineTextPreview.Margin = new System.Windows.Forms.Padding(4);
            this.lineTextPreview.Name = "lineTextPreview";
            this.lineTextPreview.Size = new System.Drawing.Size(229, 318);
            this.lineTextPreview.TabIndex = 1;
            this.lineTextPreview.Text = "lineTextPreview";
            this.lineTextPreview.VisibleWhitespace = false;
            // 
            // gridLineMode
            // 
            this.gridLineMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLineMode.Location = new System.Drawing.Point(3, 3);
            this.gridLineMode.Name = "gridLineMode";
            this.gridLineMode.SelectableType = null;
            this.gridLineMode.SelectedObject = null;
            this.gridLineMode.Size = new System.Drawing.Size(230, 320);
            this.gridLineMode.TabIndex = 6;
            this.gridLineMode.PropertyValueChanged += new System.EventHandler(this.gridLineMode_PropertyValueChanged);
            // 
            // CustomBehaviorOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelLineOverrides);
            this.DoubleBuffered = true;
            this.Name = "CustomBehaviorOptionsControl";
            this.Size = new System.Drawing.Size(473, 326);
            this.panelLineOverrides.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel panelLineOverrides;
        private LineTextPreview lineTextPreview;
        private CheckedPropertyGrid gridLineMode;
    }
}
