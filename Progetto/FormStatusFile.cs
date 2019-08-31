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
        private bool sendCanceled = false;
        private int dimFile;
        public FormStatusFile(int progress, int dimension)
        {
            InitializeComponent();
            progressBar.Value = progress;
            dimFile = dimension;

        }

        private void FormStatusFile_Load(object sender, EventArgs e)
        {
        }

        public void updateProgress(int sizeFileSent)
        {
            //Normalize better this value LUCIO! 
            //progressBar.Value = (progressBar.Maximum * sizeFileSent) / dimFile;
        }



        private void cancel_Click(object sender, EventArgs e)
        {
            sendCanceled = true;
            MessageBox.Show("Invio annullato!");
            this.Close();
        }

        public bool isSendingCanceled()
        {
            return sendCanceled;
        }
    }
}
