﻿namespace Exercise_1
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 45);
            this.Controls.Add(this.btFindOrCreate);
            this.Controls.Add(this.tbProjectName);
            this.Name = "Form1";
            this.Text = "Exercise 1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbProjectName;
        private System.Windows.Forms.Button btFindOrCreate;
    }
}

