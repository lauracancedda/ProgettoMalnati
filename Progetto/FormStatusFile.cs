using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Progetto
{
    public partial class FormStatusFile : Form
    {
        private bool terminate;
        // metto un evento sul quale update progress dorme
        // non appena la grafica viene caricata  posso fare update delle barra
        //Form1_shown viene chiamata quando la grafica è pronta
        // non appena la grafica è pronta setto l'evento sul quale updateprogress dorme
        public ManualResetEvent startUpdateProgress;
        public FormStatusFile(String formTitle)
        {
            InitializeComponent();
            this.Text = formTitle;
            terminate = false;
            progressBar.MarqueeAnimationSpeed = 0;
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Value = progressBar.Minimum;
            progressBar.MarqueeAnimationSpeed = 10;
            this.MaximizeBox = false;
            this.Shown += new System.EventHandler(this.Form1_Shown);
            startUpdateProgress = new ManualResetEvent(false);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            // segnalo che è possibile iniziare il caricamento della barra
            startUpdateProgress.Set();
        }

        public void UpdateProgress(ref bool terminateRef, long actualReceived, long dimFile, String FileName)
        {
            startUpdateProgress.WaitOne();
            if (terminate == true)
            {
                terminateRef = true;
                return;
            }
            if (!TerminationHandler.Instance.isTerminationRequired())
            {
                progressBar.BeginInvoke(new Action(() =>
                {
                    progressBar.Value = ((int)((((long)progressBar.Maximum) * actualReceived) / dimFile));
                    if (dimFile / 1024 == 0)
                        label1.Text = " Invio in corso: " + actualReceived + "/" + dimFile + " KB" + "\n File: " + FileName;
                    else
                        label1.Text = " Invio in corso: " + actualReceived / 1024 + "/" + dimFile / 1024 + " MB" + "\n File: " + FileName;
                }));
            }
        }


        private void cancel_Click(object sender, EventArgs e)
        {
            terminate = true;
            this.Close();
        }

        private void FormStatusFile_Load(object sender, EventArgs e)
        {

        }

        private void FormStatusFile_Load_1(object sender, EventArgs e)
        {

        }
    }
}
