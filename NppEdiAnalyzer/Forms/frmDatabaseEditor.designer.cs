using System.Windows.Forms;

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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnUpdate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // cbDatabaseSelector
            // 
            this.cbDatabaseSelector.FormattingEnabled = true;
            this.cbDatabaseSelector.Items.AddRange(new object[] { "LNK_Elements_Components", "LNK_Segments_Elements", "MST_Components", "MST_Elements", "MST_RefTables", "MST_Segments" });
            this.cbDatabaseSelector.Location = new System.Drawing.Point(284, 10);
            this.cbDatabaseSelector.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbDatabaseSelector.Name = "cbDatabaseSelector";
            this.cbDatabaseSelector.Size = new System.Drawing.Size(233, 24);
            this.cbDatabaseSelector.TabIndex = 0;
            this.cbDatabaseSelector.SelectedIndexChanged += new System.EventHandler(this.cbDatabaseSelector_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(44, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(194, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select Which Database To Edit";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(16, 42);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(905, 863);
            this.dataGridView1.TabIndex = 2;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(785, 912);
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(136, 41);
            this.btnUpdate.TabIndex = 3;
            this.btnUpdate.Text = "Commit Changes";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // frmDatabaseEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(937, 967);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbDatabaseSelector);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmDatabaseEditor";
            this.Text = "Database Editor";
            this.VisibleChanged += new System.EventHandler(this.FrmDatabaseEditorVisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button btnUpdate;

        #endregion

        private System.Windows.Forms.ComboBox cbDatabaseSelector;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}