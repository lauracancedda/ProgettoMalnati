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
            this.SelectText.Text = "Seleziona la cartella di destinazione per " + filename;
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
