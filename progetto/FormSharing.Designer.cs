namespace Progetto
{
    partial class FormSharing
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
            this.OnlineUser = new System.Windows.Forms.Button();
            this.ButtonSendFile = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // OnlineUser
            // 
            this.OnlineUser.Location = new System.Drawing.Point(24, 233);
            this.OnlineUser.Name = "OnlineUser";
            this.OnlineUser.Size = new System.Drawing.Size(75, 23);
            this.OnlineUser.TabIndex = 1;
            this.OnlineUser.Text = "Users Online";
            this.OnlineUser.UseVisualStyleBackColor = true;
            this.OnlineUser.Click += new System.EventHandler(this.button1_Click);
            // 
            // ButtonSendFile
            // 
            this.ButtonSendFile.Location = new System.Drawing.Point(27, 281);
            this.ButtonSendFile.Name = "ButtonSendFile";
            this.ButtonSendFile.Size = new System.Drawing.Size(75, 23);
            this.ButtonSendFile.TabIndex = 2;
            this.ButtonSendFile.Text = "Send File";
            this.ButtonSendFile.UseVisualStyleBackColor = true;
            this.ButtonSendFile.Click += new System.EventHandler(this.button2_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // FormSharing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 450);
            this.Controls.Add(this.ButtonSendFile);
            this.Controls.Add(this.OnlineUser);
            this.Name = "FormSharing";
            this.Text = " ";
            this.Load += new System.EventHandler(this.FormSharing_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button OnlineUser;
        private System.Windows.Forms.Button ButtonSendFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}