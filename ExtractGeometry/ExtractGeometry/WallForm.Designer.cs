namespace ExtractGeometry
{
    partial class WallForm
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
            this.WallCount = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // WallCount
            // 
            this.WallCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WallCount.Location = new System.Drawing.Point(95, 36);
            this.WallCount.Name = "WallCount";
            this.WallCount.Size = new System.Drawing.Size(247, 74);
            this.WallCount.TabIndex = 0;
            this.WallCount.Text = "Wall Count";
            this.WallCount.UseVisualStyleBackColor = true;
            this.WallCount.Click += new System.EventHandler(this.WallCount_Click);
            // 
            // WallForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 274);
            this.Controls.Add(this.WallCount);
            this.Name = "WallForm";
            this.Text = "WallForm";
            this.Load += new System.EventHandler(this.WallForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button WallCount;
    }
}