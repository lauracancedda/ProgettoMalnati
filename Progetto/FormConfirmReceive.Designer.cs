namespace Progetto
{
    partial class FormConfirmReceive
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
            this.RequestText = new System.Windows.Forms.Label();
            this.Accept = new System.Windows.Forms.Button();
            this.Refuse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RequestText
            // 
            this.RequestText.AccessibleName = "testoRichiesta";
            this.RequestText.AutoSize = true;
            this.RequestText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RequestText.Location = new System.Drawing.Point(106, 32);
            this.RequestText.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.RequestText.Name = "RequestText";
            this.RequestText.Size = new System.Drawing.Size(223, 18);
            this.RequestText.TabIndex = 0;
            this.RequestText.Text = "L\'utente x vorrebbe inviarti un file:";
            // 
            // Accept
            // 
            this.Accept.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Accept.Location = new System.Drawing.Point(76, 88);
            this.Accept.Margin = new System.Windows.Forms.Padding(2);
            this.Accept.Name = "Accept";
            this.Accept.Size = new System.Drawing.Size(71, 28);
            this.Accept.TabIndex = 2;
            this.Accept.Text = "Accetta";
            this.Accept.UseVisualStyleBackColor = true;
            this.Accept.Click += new System.EventHandler(this.Accept_Click);
            // 
            // Refuse
            // 
            this.Refuse.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Refuse.Location = new System.Drawing.Point(275, 88);
            this.Refuse.Margin = new System.Windows.Forms.Padding(2);
            this.Refuse.Name = "Refuse";
            this.Refuse.Size = new System.Drawing.Size(71, 28);
            this.Refuse.TabIndex = 3;
            this.Refuse.Text = "Rifiuta";
            this.Refuse.UseVisualStyleBackColor = true;
            this.Refuse.Click += new System.EventHandler(this.Refuse_Click);
            // 
            // FormConfirmReceive
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 155);
            this.Controls.Add(this.Refuse);
            this.Controls.Add(this.Accept);
            this.Controls.Add(this.RequestText);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormConfirmReceive";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ricezione File";
            this.Load += new System.EventHandler(this.FormConfirmReceive_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public void setTesto(string testo)
        {
            return;
        }

        private System.Windows.Forms.Label RequestText;
        private System.Windows.Forms.Button Accept;
        private System.Windows.Forms.Button Refuse;
    }
}