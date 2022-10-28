namespace ExtractGeometry
{
    partial class SQLGeometryForm
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
            this.btnTableCreate = new System.Windows.Forms.Button();
            this.btnExportData = new System.Windows.Forms.Button();
            this.btnDropTable = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnTableCreate
            // 
            this.btnTableCreate.Location = new System.Drawing.Point(148, 55);
            this.btnTableCreate.Name = "btnTableCreate";
            this.btnTableCreate.Size = new System.Drawing.Size(226, 56);
            this.btnTableCreate.TabIndex = 0;
            this.btnTableCreate.Text = "Create SQL Table";
            this.btnTableCreate.UseVisualStyleBackColor = true;
            this.btnTableCreate.Click += new System.EventHandler(this.btnTableCreate_Click);
            // 
            // btnExportData
            // 
            this.btnExportData.Location = new System.Drawing.Point(148, 252);
            this.btnExportData.Name = "btnExportData";
            this.btnExportData.Size = new System.Drawing.Size(226, 62);
            this.btnExportData.TabIndex = 1;
            this.btnExportData.Text = "Export Data";
            this.btnExportData.UseVisualStyleBackColor = true;
            this.btnExportData.Click += new System.EventHandler(this.btnExportData_Click);
            // 
            // btnDropTable
            // 
            this.btnDropTable.Location = new System.Drawing.Point(148, 148);
            this.btnDropTable.Name = "btnDropTable";
            this.btnDropTable.Size = new System.Drawing.Size(226, 62);
            this.btnDropTable.TabIndex = 2;
            this.btnDropTable.Text = "Delete SQL Table";
            this.btnDropTable.UseVisualStyleBackColor = true;
            this.btnDropTable.Click += new System.EventHandler(this.btnDropTable_Click);
            // 
            // SQLGeometryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 383);
            this.Controls.Add(this.btnDropTable);
            this.Controls.Add(this.btnExportData);
            this.Controls.Add(this.btnTableCreate);
            this.Name = "SQLGeometryForm";
            this.Text = "SQLGeometryForm";
            this.Load += new System.EventHandler(this.SQLGeometryForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTableCreate;
        private System.Windows.Forms.Button btnExportData;
        private System.Windows.Forms.Button btnDropTable;
    }
}