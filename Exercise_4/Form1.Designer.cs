namespace Exercise_4
{
    partial class Form1
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
            this.tbProjectName = new System.Windows.Forms.TextBox();
            this.btFindOrCreate = new System.Windows.Forms.Button();
            this.btCreateShape = new System.Windows.Forms.Button();
            this.btCreateAssembly = new System.Windows.Forms.Button();
            this.btExport = new System.Windows.Forms.Button();
            this.btImport = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbProjectName
            // 
            this.tbProjectName.Location = new System.Drawing.Point(13, 13);
            this.tbProjectName.Name = "tbProjectName";
            this.tbProjectName.Size = new System.Drawing.Size(114, 20);
            this.tbProjectName.TabIndex = 0;
            // 
            // btFindOrCreate
            // 
            this.btFindOrCreate.Location = new System.Drawing.Point(133, 11);
            this.btFindOrCreate.Name = "btFindOrCreate";
            this.btFindOrCreate.Size = new System.Drawing.Size(163, 23);
            this.btFindOrCreate.TabIndex = 1;
            this.btFindOrCreate.Text = "Find or create project";
            this.btFindOrCreate.UseVisualStyleBackColor = true;
            this.btFindOrCreate.Click += new System.EventHandler(this.btFindOrCreate_Click);
            // 
            // btCreateShape
            // 
            this.btCreateShape.Location = new System.Drawing.Point(13, 55);
            this.btCreateShape.Name = "btCreateShape";
            this.btCreateShape.Size = new System.Drawing.Size(280, 36);
            this.btCreateShape.TabIndex = 2;
            this.btCreateShape.Text = "Create a shape";
            this.btCreateShape.UseVisualStyleBackColor = true;
            this.btCreateShape.Click += new System.EventHandler(this.btCreateShape_Click);
            // 
            // btCreateAssembly
            // 
            this.btCreateAssembly.Location = new System.Drawing.Point(12, 97);
            this.btCreateAssembly.Name = "btCreateAssembly";
            this.btCreateAssembly.Size = new System.Drawing.Size(280, 36);
            this.btCreateAssembly.TabIndex = 2;
            this.btCreateAssembly.Text = "Create an assembly";
            this.btCreateAssembly.UseVisualStyleBackColor = true;
            this.btCreateAssembly.Click += new System.EventHandler(this.btCreateAssembly_Click);
            // 
            // btExport
            // 
            this.btExport.Location = new System.Drawing.Point(12, 139);
            this.btExport.Name = "btExport";
            this.btExport.Size = new System.Drawing.Size(280, 36);
            this.btExport.TabIndex = 2;
            this.btExport.Text = "Export document";
            this.btExport.UseVisualStyleBackColor = true;
            this.btExport.Click += new System.EventHandler(this.btExport_Click);
            // 
            // btImport
            // 
            this.btImport.Location = new System.Drawing.Point(12, 181);
            this.btImport.Name = "btImport";
            this.btImport.Size = new System.Drawing.Size(280, 36);
            this.btImport.TabIndex = 2;
            this.btImport.Text = "Import";
            this.btImport.UseVisualStyleBackColor = true;
            this.btImport.Click += new System.EventHandler(this.btImport_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 227);
            this.Controls.Add(this.btImport);
            this.Controls.Add(this.btExport);
            this.Controls.Add(this.btCreateAssembly);
            this.Controls.Add(this.btCreateShape);
            this.Controls.Add(this.btFindOrCreate);
            this.Controls.Add(this.tbProjectName);
            this.Name = "Form1";
            this.Text = "Exercise 4";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbProjectName;
        private System.Windows.Forms.Button btFindOrCreate;
        private System.Windows.Forms.Button btCreateShape;
        private System.Windows.Forms.Button btCreateAssembly;
        private System.Windows.Forms.Button btExport;
        private System.Windows.Forms.Button btImport;
    }
}


