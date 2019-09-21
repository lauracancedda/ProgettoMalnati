using System;
using System.Windows.Forms;
using System.Threading;

namespace Progetto
{
    public partial class FormStatusFile : Form
    {
        private bool terminate;
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

        // evento generato quando la grafica è pronta, setta startUpdateProgress per iniziare l'update
        private void Form1_Shown(object sender, EventArgs e)
        {
            // è possibile iniziare il caricamento della barra
            startUpdateProgress.Set();
        }

        public void UpdateProgress(ref bool terminateRef, long actualReceived, long dimFile, String filename)
        {
            startUpdateProgress.WaitOne();
            if (terminate == true)
            {
                terminateRef = true;
                return;
            }

            // mostra solo il nome del file
            filename = filename.Substring(filename.LastIndexOf('\\') + 1);
            string format = filename.Substring(filename.LastIndexOf('.'));
            filename = filename.TrimEnd(format.ToCharArray());

            if (!TerminationHandler.Instance.isTerminationRequired())
            {
                progressBar.BeginInvoke(new Action(() =>
                {
                    progressBar.Value = ((int)((((long)progressBar.Maximum) * actualReceived) / dimFile));
                    if (dimFile / 1024 == 0)
                        label1.Text = " Invio in corso: " + actualReceived + "/" + dimFile + " KB" + "\n File: " + filename;
                    else
                        label1.Text = " Invio in corso: " + actualReceived / 1024 + "/" + dimFile / 1024 + " MB" + "\n File: " + filename;
                }));
            }
        }


        private void cancel_Click(object sender, EventArgs e)
        {
            terminate = true;
            this.Close();
        }

    }
}
