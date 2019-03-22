﻿namespace Progetto
{
    partial class FormSelectPath
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
            this.SelectText = new System.Windows.Forms.Label();
            this.ChooseFolder = new System.Windows.Forms.Button();
            this.Folder = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // SelectText
            // 
            this.SelectText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectText.AutoSize = true;
            this.SelectText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectText.Location = new System.Drawing.Point(126, 34);
            this.SelectText.Name = "SelectText";
            this.SelectText.Size = new System.Drawing.Size(321, 25);
            this.SelectText.TabIndex = 0;
            this.SelectText.Text = "Seleziona la cartella di destinazione";
            this.SelectText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ChooseFolder
            // 
            this.ChooseFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChooseFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChooseFolder.Location = new System.Drawing.Point(214, 85);
            this.ChooseFolder.Name = "ChooseFolder";
            this.ChooseFolder.Size = new System.Drawing.Size(129, 39);
            this.ChooseFolder.TabIndex = 1;
            this.ChooseFolder.Text = "Scegli";
            this.ChooseFolder.UseVisualStyleBackColor = true;
            this.ChooseFolder.Click += new System.EventHandler(this.ChooseFolder_Click);
            // 
            // Folder
            // 
            this.Folder.Description = "Seleziona la cartella di ricezione";
            // 
            // FormSelectPath
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 162);
            this.Controls.Add(this.ChooseFolder);
            this.Controls.Add(this.SelectText);
            this.Name = "FormSelectPath";
            this.Text = "Ricezione file";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label SelectText;
        private System.Windows.Forms.Button ChooseFolder;
        private System.Windows.Forms.FolderBrowserDialog Folder;
    }
}