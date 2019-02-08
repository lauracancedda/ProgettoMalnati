namespace Progetto
{
    partial class FormSettings
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Browse = new System.Windows.Forms.Button();
            this.Name = new System.Windows.Forms.TextBox();
            this.Photo = new System.Windows.Forms.PictureBox();
            this.AutomaticReceive = new System.Windows.Forms.CheckBox();
            this.PrivateMode = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ChooseFolder = new System.Windows.Forms.Button();
            this.Save = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.Folder = new System.Windows.Forms.FolderBrowserDialog();
            this.Path = new System.Windows.Forms.Label();
            this.Icon = new System.Windows.Forms.NotifyIcon(this.components);
            this.Menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.Photo)).BeginInit();
            this.Menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(37, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nome utente:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(38, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Foto profilo:";
            // 
            // Browse
            // 
            this.Browse.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.Browse.Location = new System.Drawing.Point(429, 103);
            this.Browse.Name = "Browse";
            this.Browse.Size = new System.Drawing.Size(116, 36);
            this.Browse.TabIndex = 2;
            this.Browse.Text = "Sfoglia";
            this.Browse.UseVisualStyleBackColor = false;
            this.Browse.Click += new System.EventHandler(this.Browse_Click);
            // 
            // Name
            // 
            this.Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name.Location = new System.Drawing.Point(195, 42);
            this.Name.Name = "Name";
            this.Name.Size = new System.Drawing.Size(234, 27);
            this.Name.TabIndex = 3;
            this.Name.TextChanged += new System.EventHandler(this.Name_TextChanged);
            // 
            // Photo
            // 
            this.Photo.Location = new System.Drawing.Point(195, 103);
            this.Photo.Name = "Photo";
            this.Photo.Size = new System.Drawing.Size(168, 135);
            this.Photo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Photo.TabIndex = 4;
            this.Photo.TabStop = false;
            // 
            // AutomaticReceive
            // 
            this.AutomaticReceive.AutoSize = true;
            this.AutomaticReceive.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AutomaticReceive.Location = new System.Drawing.Point(41, 325);
            this.AutomaticReceive.Name = "AutomaticReceive";
            this.AutomaticReceive.Size = new System.Drawing.Size(322, 24);
            this.AutomaticReceive.TabIndex = 5;
            this.AutomaticReceive.Text = "Accetta automaticamente i file in arrivo";
            this.AutomaticReceive.UseVisualStyleBackColor = true;
            this.AutomaticReceive.CheckedChanged += new System.EventHandler(this.AutomaticReceive_CheckedChanged);
            // 
            // PrivateMode
            // 
            this.PrivateMode.AutoSize = true;
            this.PrivateMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PrivateMode.Location = new System.Drawing.Point(41, 365);
            this.PrivateMode.Name = "PrivateMode";
            this.PrivateMode.Size = new System.Drawing.Size(359, 24);
            this.PrivateMode.TabIndex = 6;
            this.PrivateMode.Text = "Modalità privata (non visibile agli altri utenti)";
            this.PrivateMode.UseVisualStyleBackColor = true;
            this.PrivateMode.CheckedChanged += new System.EventHandler(this.PrivateMode_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(41, 269);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(344, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Percorso di ricezione predefinito (opzionale):";
            // 
            // ChooseFolder
            // 
            this.ChooseFolder.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ChooseFolder.Location = new System.Drawing.Point(429, 269);
            this.ChooseFolder.Name = "ChooseFolder";
            this.ChooseFolder.Size = new System.Drawing.Size(115, 40);
            this.ChooseFolder.TabIndex = 8;
            this.ChooseFolder.Text = "Scegli";
            this.ChooseFolder.UseVisualStyleBackColor = false;
            this.ChooseFolder.Click += new System.EventHandler(this.ChooseFolder_Click);
            // 
            // Save
            // 
            this.Save.Enabled = false;
            this.Save.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Save.Location = new System.Drawing.Point(237, 448);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(136, 41);
            this.Save.TabIndex = 9;
            this.Save.Text = "Salva";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            this.openFileDialog.Filter = "JPEG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|BMP Files (*.bmp)|*.bmp|All file" +
    "s (*.*)|*.*  ";
            this.openFileDialog.Title = "Selezionare un file di immagine";
            // 
            // Folder
            // 
            this.Folder.Description = "Seleziona la cartella che vuoi usare come default";
            // 
            // Path
            // 
            this.Path.AutoSize = true;
            this.Path.Location = new System.Drawing.Point(45, 291);
            this.Path.MinimumSize = new System.Drawing.Size(350, 0);
            this.Path.Name = "Path";
            this.Path.Size = new System.Drawing.Size(350, 17);
            this.Path.TabIndex = 10;
            // 
            // Icon
            // 
            this.Icon.Icon = ((System.Drawing.Icon)(resources.GetObject("Icon.Icon")));
            this.Icon.Text = "Sharing";
            this.Icon.Visible = true;
            // 
            // Menu
            // 
            this.Menu.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Menu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.Menu.Name = "contextMenuStrip1";
            this.Menu.Size = new System.Drawing.Size(113, 56);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(112, 26);
            this.openToolStripMenuItem.Text = "Apri";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(112, 26);
            this.exitToolStripMenuItem.Text = "Esci";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 501);
            this.ControlBox = false;
            this.Controls.Add(this.Path);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.ChooseFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.PrivateMode);
            this.Controls.Add(this.AutomaticReceive);
            this.Controls.Add(this.Photo);
            this.Controls.Add(this.Name);
            this.Controls.Add(this.Browse);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Text = "Impostazioni";
            this.Load += new System.EventHandler(this.FormSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Photo)).EndInit();
            this.Menu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Browse;
        private System.Windows.Forms.TextBox Name;
        private System.Windows.Forms.PictureBox Photo;
        private System.Windows.Forms.CheckBox AutomaticReceive;
        private System.Windows.Forms.CheckBox PrivateMode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button ChooseFolder;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.FolderBrowserDialog Folder;
        private System.Windows.Forms.Label Path;
        private System.Windows.Forms.NotifyIcon Icon;
        private System.Windows.Forms.ContextMenuStrip Menu;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    }
}