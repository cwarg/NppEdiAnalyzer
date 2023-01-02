namespace Kbg.Demo.Namespace
{
    partial class frmDatabaseEditor
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbDatabaseSelector = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbDatabaseSelector
            // 
            this.cbDatabaseSelector.FormattingEnabled = true;
            this.cbDatabaseSelector.Items.AddRange(new object[] {
            "LNK_Elements_Components",
            "LNK_Segments_Elements",
            "MST_Components",
            "MST_Elements",
            "MST_RefTables",
            "MST_Segments"});
            this.cbDatabaseSelector.Location = new System.Drawing.Point(319, 12);
            this.cbDatabaseSelector.Name = "cbDatabaseSelector";
            this.cbDatabaseSelector.Size = new System.Drawing.Size(262, 28);
            this.cbDatabaseSelector.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(50, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(230, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select Which Database To Edit";
            // 
            // frmDatabaseEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 1209);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbDatabaseSelector);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmDatabaseEditor";
            this.Text = "Database Editor";
            this.VisibleChanged += new System.EventHandler(this.FrmDatabaseEditorVisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbDatabaseSelector;
        private System.Windows.Forms.Label label1;
    }
}