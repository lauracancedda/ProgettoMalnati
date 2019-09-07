using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Progetto
{
    public partial class FormSettings : Form
    {
        Settings setting;
        MainClass main;
        bool file_modified;
        bool photo_modified;
        Thread tStartApp = null;

        public FormSettings()
        {
            InitializeComponent();
            NotificationIcon.ContextMenuStrip = AppMenu;
            setting = new Settings();
        }

        ~FormSettings()
        {
            if(tStartApp!=null)
            {
                tStartApp.Join();
                tStartApp = null;
            }         
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            if (File.Exists("configurazione.txt"))
                LoadFromFile();
            file_modified = false;
            photo_modified = false;
        }

        private void Username_TextChanged(object sender, EventArgs e)
        {
            // se il nome non è stato inserito non si può salvare
            if (Username.TextLength > 0)
                Save.Enabled = true;
            else
                Save.Enabled = false;

            file_modified = true;
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (Photo.Image != null)
                    Photo.Image.Dispose();
                Photo.Load(openFileDialog.FileName);
                photo_modified = true;
            }
        }


        private void ChooseFolder_Click(object sender, EventArgs e)
        {
            if (Folder.ShowDialog() == DialogResult.OK)
            {
                Path.Text = Folder.SelectedPath;
                file_modified = true;
            }
        }


        private void AutomaticReceive_CheckedChanged(object sender, EventArgs e)
        {
            file_modified = true;
        }

        private void PrivateMode_CheckedChanged(object sender, EventArgs e)
        {
            file_modified = true;
        }

        // salva i campi del form nelle impostazioni e se necessario anche su file
        private void Save_Click(object sender, EventArgs e)
        {
            setting.mutex_setting.WaitOne(); //lock

            // nome
            setting.Name = Username.Text;
            // foto
            setting.Photo = Photo.Image;
            if (photo_modified == true)
                setting.PhotoSelected = true;
            // percorso di default
            setting.DefaultPath = Path.Text;
            if (Path.Text != "")
                setting.DefaultSelected = true;
            else
                setting.DefaultSelected = false;
            // ricezione automatica
            if (AutomaticReceive.Checked)
                setting.AutomaticReceive = true;
            else
                setting.AutomaticReceive = false;
            // modalità 
            if (PrivateMode.Checked)
                setting.PrivateMode = true;
            else
                setting.PrivateMode = false;

            // scrittura su file se qualcosa è cambiato 
            if (file_modified == true || photo_modified == true)
                SaveOnFile();

            setting.mutex_setting.ReleaseMutex(); // unlock


            // lancia il thread principale
            if (tStartApp == null)
            {
                main = new MainClass(ref setting);
                tStartApp = new Thread(main.Start);
                tStartApp.Start();
            }

            // Read the Path exe and send
            var process = Process.GetCurrentProcess();
            string fullPath = process.MainModule.FileName;
            setting.SetKeyRegedit(fullPath);
            this.Visible = false;
        }

        // riempie i campi del form  e aggiorna i flag leggendoli dal file
        public void LoadFromFile()
        {
            StreamReader reader = File.OpenText("configurazione.txt");
            // il nome c'è sicuramente
            string line = reader.ReadLine();
            Username.Text = line;
            // percorso di default
            line = reader.ReadLine();
            Path.Text = line;
            // ricezione automatica
            line = reader.ReadLine();
            if (line == "yes")
                AutomaticReceive.Checked = true;
            else
                AutomaticReceive.Checked = false;
            // modalità 
            line = reader.ReadLine();
            if (line == "yes")
                PrivateMode.Checked = true;
            else
                PrivateMode.Checked = false;

            reader.Close();

            // carica l'immagine se presente
            if (File.Exists("immagine.jpg"))
            {
                Photo.Image = new Bitmap(".\\immagine.jpg");
                setting.mutex_setting.WaitOne();
                setting.PhotoSelected = true;
                setting.mutex_setting.ReleaseMutex();
                photo_modified = true;
            }

        }

        public void SaveOnFile()
        {
            if (file_modified == true)
            {
                // scrive su file i campi salvati, uno in ogni riga;
                // in ordine: nome, default, ricezione automatica("yes"/""), modalità("yes"/"")
                File.Create("configurazione.txt").Close();
                StreamWriter writer = File.AppendText("configurazione.txt"); ;
                writer.WriteLine(setting.Name);
                writer.WriteLine(setting.DefaultPath);
                if (setting.AutomaticReceive == true)
                    writer.WriteLine("yes");
                else
                    writer.WriteLine("");
                if (setting.PrivateMode == true)
                    writer.WriteLine("yes");
                else
                    writer.WriteLine("");
                writer.Close();
            }

            // salva l'immagine
            if (photo_modified == true)
                Photo.Image.Save("immagine01.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = true;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TerminationHandler.Instance.setTermination();
            setting.publicMode.Set();
            this.NotificationIcon.Dispose();
            // termina tutti i thread e chiude l'applicazione
            Environment.Exit(0);

        }
    }
}
