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
        string path;
        public FormSelectPath()
        {
            InitializeComponent();
        }

        private void ChooseFolder_Click(object sender, EventArgs e)
        {
            if (Folder.ShowDialog() == DialogResult.OK)
            {
                path = Folder.SelectedPath;
            }
            this.Visible = false;
        }
    }
}
