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
        private bool terminate;
        public FormStatusFile()
        {
            InitializeComponent();
            terminate = false;
            progressBar.MarqueeAnimationSpeed = 0;
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Value = progressBar.Minimum;
            progressBar.MarqueeAnimationSpeed = 10;
            this.MaximizeBox = false;
        }


        public void UpdateProgress(ref bool terminateRef, long actualReceived, long dimFile, String FileName)
        {
            if (terminate == true)
            {
                terminateRef = true;
                return;
            }

            progressBar.BeginInvoke(new Action(() =>
            {
                progressBar.Value = ((int)((((long)progressBar.Maximum) * actualReceived) / dimFile));
                if (dimFile / 1024 == 0)
                    label1.Text = " Invio in corso: " + actualReceived + "/" + dimFile + " KB" + "\n File: " + FileName;
                else
                    label1.Text = " Invio in corso: " + actualReceived / 1024 + "/" + dimFile / 1024 + " MB" + "\n File: " + FileName;
            }));
        }


        private void cancel_Click(object sender, EventArgs e)
        {
            terminate = true;
            this.Close();
        }
    }
}
