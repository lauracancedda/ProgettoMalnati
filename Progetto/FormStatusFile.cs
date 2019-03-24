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
    public partial class FormStatusFile : Form
    {
        private bool status = true;
        public FormStatusFile(int statusFile)
        {
            InitializeComponent();
            progressBar1.Value = statusFile % 100;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void FormStatusFile_Load(object sender, EventArgs e)
        {
        }

        public int ChangeStatus(int x)
        {
            progressBar1.Value = x % 100;
            if (status == false)
            {
                //ANNULLA INVIO
                return -1;
            }
            return 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            status = false;
            this.Close();
        }
    }
}
