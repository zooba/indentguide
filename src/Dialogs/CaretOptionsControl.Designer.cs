namespace IndentGuide
{
    partial class CaretOptionsControl
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
            this.lstNames = new System.Windows.Forms.ListBox();
            this.webDocumentation = new System.Windows.Forms.WebBrowser();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.tableLayoutPanel2.Controls.Add(this.lstNames, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.webDocumentation, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(607, 390);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // lstNames
            // 
            this.lstNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstNames.FormattingEnabled = true;
            this.lstNames.IntegralHeight = false;
            this.lstNames.ItemHeight = 16;
            this.lstNames.Location = new System.Drawing.Point(4, 4);
            this.lstNames.Margin = new System.Windows.Forms.Padding(4);
            this.lstNames.Name = "lstNames";
            this.lstNames.Size = new System.Drawing.Size(194, 382);
            this.lstNames.TabIndex = 0;
            this.lstNames.SelectedIndexChanged += new System.EventHandler(this.lstNames_SelectedIndexChanged);
            this.lstNames.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.lstNames_Format);
            // 
            // webDocumentation
            // 
            this.webDocumentation.AllowWebBrowserDrop = false;
            this.webDocumentation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webDocumentation.IsWebBrowserContextMenuEnabled = false;
            this.webDocumentation.Location = new System.Drawing.Point(205, 3);
            this.webDocumentation.MinimumSize = new System.Drawing.Size(20, 20);
            this.webDocumentation.Name = "webDocumentation";
            this.webDocumentation.ScriptErrorsSuppressed = true;
            this.webDocumentation.Size = new System.Drawing.Size(399, 384);
            this.webDocumentation.TabIndex = 1;
            this.webDocumentation.Url = new System.Uri("about:blank", System.UriKind.Absolute);
            this.webDocumentation.WebBrowserShortcutsEnabled = false;
            // 
            // CaretOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CaretOptionsControl";
            this.Size = new System.Drawing.Size(607, 390);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ListBox lstNames;
        private System.Windows.Forms.WebBrowser webDocumentation;
    }
}
