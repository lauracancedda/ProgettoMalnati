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

        private int dimfile;
        public FormStatusFile(int statusFile,int dimension)
        {
            InitializeComponent();
            progressBar1.Value = statusFile ;
            dimfile = dimension;

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

        public int ChangeStatus(int sizefilesend)
        {
            //Normalize better this value LUCIO! 
            progressBar1.Value = (progressBar1.Maximum*sizefilesend)/dimfile;
            if (status == false)
            {
                //Send Cancel
                MessageBox.Show("Invio Cancel!");
                //this.Close();
                return -1;
            }
            return 0;
        }

  

        private void button1_Click_1(object sender, EventArgs e)
        {
            status = false;
            this.Close();
        }
    }
}
