using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Progetto
{
    public partial class FormSelectPath : Form
    {
        private string path;
        public FormSelectPath(string filename)
        {
            InitializeComponent();
            this.SelectText.AutoSize = true;
            this.SelectText.Text = "Seleziona la cartella di destinazione per " + filename.Replace("\\", "");
            this.SelectText.Dock = DockStyle.Fill;
            this.AutoSize = true;
            this.SetAutoSizeMode(AutoSizeMode.GrowAndShrink);
            this.ChooseFolder.Size = new Size(110, 30);
        }

        private void ChooseFolder_Click(object sender, EventArgs e)
        {
            if (Folder.ShowDialog() == DialogResult.OK)
            {
                path = Folder.SelectedPath;
            }
            this.Close();
        }

        public string GetPath()
        {
            return path;
        }
    }
}
