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
            this.Send = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // Send
            // 
            this.Send.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Send.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Send.Location = new System.Drawing.Point(312, 466);
            this.Send.Margin = new System.Windows.Forms.Padding(30, 60, 30, 30);
            this.Send.MaximumSize = new System.Drawing.Size(140, 40);
            this.Send.MinimumSize = new System.Drawing.Size(140, 40);
            this.Send.Name = "Send";
            this.Send.Size = new System.Drawing.Size(140, 40);
            this.Send.TabIndex = 2;
            this.Send.Text = "Invia";
            this.Send.UseVisualStyleBackColor = true;
            this.Send.Click += new System.EventHandler(this.Send_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // FormSharing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(773, 554);
            this.Controls.Add(this.Send);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormSharing";
            this.Text = "Seleziona i destinatari tra gli utenti online";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button Send;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}