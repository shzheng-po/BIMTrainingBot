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
            this.buttonImportDataFromDatabase = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnTableCreate
            // 
            this.btnTableCreate.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnTableCreate.Location = new System.Drawing.Point(148, 55);
            this.btnTableCreate.Name = "btnTableCreate";
            this.btnTableCreate.Size = new System.Drawing.Size(226, 56);
            this.btnTableCreate.TabIndex = 0;
            this.btnTableCreate.Text = "Create SQL Table";
            this.btnTableCreate.UseVisualStyleBackColor = false;
            this.btnTableCreate.Click += new System.EventHandler(this.btnTableCreate_Click);
            // 
            // btnExportData
            // 
            this.btnExportData.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnExportData.Location = new System.Drawing.Point(148, 252);
            this.btnExportData.Name = "btnExportData";
            this.btnExportData.Size = new System.Drawing.Size(226, 62);
            this.btnExportData.TabIndex = 1;
            this.btnExportData.Text = "Export Data";
            this.btnExportData.UseVisualStyleBackColor = false;
            this.btnExportData.Click += new System.EventHandler(this.btnExportData_Click);
            // 
            // btnDropTable
            // 
            this.btnDropTable.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnDropTable.Location = new System.Drawing.Point(148, 148);
            this.btnDropTable.Name = "btnDropTable";
            this.btnDropTable.Size = new System.Drawing.Size(226, 62);
            this.btnDropTable.TabIndex = 2;
            this.btnDropTable.Text = "Delete SQL Table";
            this.btnDropTable.UseVisualStyleBackColor = false;
            this.btnDropTable.Click += new System.EventHandler(this.btnDropTable_Click);
            // 
            // buttonImportDataFromDatabase
            // 
            this.buttonImportDataFromDatabase.BackColor = System.Drawing.Color.Red;
            this.buttonImportDataFromDatabase.Location = new System.Drawing.Point(148, 366);
            this.buttonImportDataFromDatabase.Name = "buttonImportDataFromDatabase";
            this.buttonImportDataFromDatabase.Size = new System.Drawing.Size(226, 93);
            this.buttonImportDataFromDatabase.TabIndex = 3;
            this.buttonImportDataFromDatabase.Text = "Import Data from Database (DANGER!!!)";
            this.buttonImportDataFromDatabase.UseVisualStyleBackColor = false;
            this.buttonImportDataFromDatabase.Click += new System.EventHandler(this.buttonImportDataFromDatabase_Click);
            // 
            // SQLGeometryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 526);
            this.Controls.Add(this.buttonImportDataFromDatabase);
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
        private System.Windows.Forms.Button buttonImportDataFromDatabase;
    }
}