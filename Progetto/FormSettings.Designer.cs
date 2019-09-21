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
            this.Username = new System.Windows.Forms.TextBox();
            this.Photo = new System.Windows.Forms.PictureBox();
            this.AutomaticReceive = new System.Windows.Forms.CheckBox();
            this.PrivateMode = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ChooseFolder = new System.Windows.Forms.Button();
            this.Save = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.Folder = new System.Windows.Forms.FolderBrowserDialog();
            this.Path = new System.Windows.Forms.Label();
            this.NotificationIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.AppMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectStatus = new System.Windows.Forms.ToolStripComboBox();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteImage = new System.Windows.Forms.Button();
            this.deletePath = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Photo)).BeginInit();
            this.AppMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(28, 34);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nome utente:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(28, 89);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Foto profilo:";
            // 
            // Browse
            // 
            this.Browse.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.Browse.Location = new System.Drawing.Point(334, 84);
            this.Browse.Margin = new System.Windows.Forms.Padding(2);
            this.Browse.Name = "Browse";
            this.Browse.Size = new System.Drawing.Size(87, 29);
            this.Browse.TabIndex = 2;
            this.Browse.Text = "Sfoglia";
            this.Browse.UseVisualStyleBackColor = false;
            this.Browse.Click += new System.EventHandler(this.Browse_Click);
            // 
            // Username
            // 
            this.Username.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Username.Location = new System.Drawing.Point(146, 34);
            this.Username.Margin = new System.Windows.Forms.Padding(2);
            this.Username.Name = "Username";
            this.Username.Size = new System.Drawing.Size(176, 23);
            this.Username.TabIndex = 3;
            this.Username.TextChanged += new System.EventHandler(this.Username_TextChanged);
            // 
            // Photo
            // 
            this.Photo.Location = new System.Drawing.Point(146, 84);
            this.Photo.Margin = new System.Windows.Forms.Padding(2);
            this.Photo.Name = "Photo";
            this.Photo.Size = new System.Drawing.Size(126, 110);
            this.Photo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Photo.TabIndex = 4;
            this.Photo.TabStop = false;
            // 
            // AutomaticReceive
            // 
            this.AutomaticReceive.AutoSize = true;
            this.AutomaticReceive.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AutomaticReceive.Location = new System.Drawing.Point(31, 264);
            this.AutomaticReceive.Margin = new System.Windows.Forms.Padding(2);
            this.AutomaticReceive.Name = "AutomaticReceive";
            this.AutomaticReceive.Size = new System.Drawing.Size(270, 21);
            this.AutomaticReceive.TabIndex = 5;
            this.AutomaticReceive.Text = "Accetta automaticamente i file in arrivo";
            this.AutomaticReceive.UseVisualStyleBackColor = true;
            this.AutomaticReceive.CheckedChanged += new System.EventHandler(this.AutomaticReceive_CheckedChanged);
            // 
            // PrivateMode
            // 
            this.PrivateMode.AutoSize = true;
            this.PrivateMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PrivateMode.Location = new System.Drawing.Point(31, 297);
            this.PrivateMode.Margin = new System.Windows.Forms.Padding(2);
            this.PrivateMode.Name = "PrivateMode";
            this.PrivateMode.Size = new System.Drawing.Size(303, 21);
            this.PrivateMode.TabIndex = 6;
            this.PrivateMode.Text = "Modalità privata (non visibile agli altri utenti)";
            this.PrivateMode.UseVisualStyleBackColor = true;
            this.PrivateMode.CheckedChanged += new System.EventHandler(this.PrivateMode_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(28, 219);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(291, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Percorso di ricezione predefinito (opzionale):";
            // 
            // ChooseFolder
            // 
            this.ChooseFolder.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ChooseFolder.Location = new System.Drawing.Point(334, 212);
            this.ChooseFolder.Margin = new System.Windows.Forms.Padding(2);
            this.ChooseFolder.Name = "ChooseFolder";
            this.ChooseFolder.Size = new System.Drawing.Size(86, 32);
            this.ChooseFolder.TabIndex = 8;
            this.ChooseFolder.Text = "Scegli";
            this.ChooseFolder.UseVisualStyleBackColor = false;
            this.ChooseFolder.Click += new System.EventHandler(this.ChooseFolder_Click);
            // 
            // Save
            // 
            this.Save.Enabled = false;
            this.Save.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Save.Location = new System.Drawing.Point(184, 364);
            this.Save.Margin = new System.Windows.Forms.Padding(2);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(102, 33);
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
            this.Path.Location = new System.Drawing.Point(34, 236);
            this.Path.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Path.MinimumSize = new System.Drawing.Size(262, 0);
            this.Path.Name = "Path";
            this.Path.Size = new System.Drawing.Size(262, 13);
            this.Path.TabIndex = 10;
            // 
            // NotificationIcon
            // 
            this.NotificationIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("NotificationIcon.Icon")));
            this.NotificationIcon.Text = "Sharing";
            this.NotificationIcon.Visible = true;
            // 
            // AppMenu
            // 
            this.AppMenu.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.AppMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.AppMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.AppMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectStatus,
            this.openToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.AppMenu.Name = "contextMenuStrip1";
            this.AppMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.AppMenu.Size = new System.Drawing.Size(182, 75);
            // 
            // selectStatus
            // 
            this.selectStatus.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.selectStatus.Items.AddRange(new object[] {
            "Online",
            "Offline"});
            this.selectStatus.Name = "selectStatus";
            this.selectStatus.Size = new System.Drawing.Size(121, 23);
            this.selectStatus.Text = "Stato";
            this.selectStatus.SelectedIndexChanged += new System.EventHandler(this.SelectStatus_SelectedIndexChanged);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.openToolStripMenuItem.Text = "Impostazioni";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.exitToolStripMenuItem.Text = "Esci";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // deleteImage
            // 
            this.deleteImage.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.deleteImage.Font = new System.Drawing.Font("Comic Sans MS", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteImage.ForeColor = System.Drawing.Color.Red;
            this.deleteImage.Location = new System.Drawing.Point(427, 84);
            this.deleteImage.Margin = new System.Windows.Forms.Padding(2, 2, 2, 5);
            this.deleteImage.Name = "deleteImage";
            this.deleteImage.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.deleteImage.Size = new System.Drawing.Size(28, 29);
            this.deleteImage.TabIndex = 11;
            this.deleteImage.Text = "X";
            this.deleteImage.UseVisualStyleBackColor = false;
            this.deleteImage.Click += new System.EventHandler(this.DeleteImage_Click);
            // 
            // deletePath
            // 
            this.deletePath.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.deletePath.Font = new System.Drawing.Font("Comic Sans MS", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deletePath.ForeColor = System.Drawing.Color.Red;
            this.deletePath.Location = new System.Drawing.Point(427, 213);
            this.deletePath.Margin = new System.Windows.Forms.Padding(2, 2, 2, 5);
            this.deletePath.Name = "deletePath";
            this.deletePath.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.deletePath.Size = new System.Drawing.Size(28, 29);
            this.deletePath.TabIndex = 12;
            this.deletePath.Text = "X";
            this.deletePath.UseVisualStyleBackColor = false;
            this.deletePath.Click += new System.EventHandler(this.DeletePath_Click);
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 418);
            this.ControlBox = false;
            this.Controls.Add(this.deletePath);
            this.Controls.Add(this.deleteImage);
            this.Controls.Add(this.Path);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.ChooseFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.PrivateMode);
            this.Controls.Add(this.AutomaticReceive);
            this.Controls.Add(this.Photo);
            this.Controls.Add(this.Username);
            this.Controls.Add(this.Browse);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Impostazioni";
            this.Load += new System.EventHandler(this.FormSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Photo)).EndInit();
            this.AppMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Browse;
        private System.Windows.Forms.TextBox Username;
        private System.Windows.Forms.PictureBox Photo;
        private System.Windows.Forms.CheckBox AutomaticReceive;
        private System.Windows.Forms.CheckBox PrivateMode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button ChooseFolder;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.FolderBrowserDialog Folder;
        private System.Windows.Forms.Label Path;
        private System.Windows.Forms.NotifyIcon NotificationIcon;
        private System.Windows.Forms.ContextMenuStrip AppMenu;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox selectStatus;
        private System.Windows.Forms.Button deleteImage;
        private System.Windows.Forms.Button deletePath;
    }
}