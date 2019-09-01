using System;
using System.Windows.Forms;
using System.Threading;

namespace Progetto
{
    public partial class FormStatusFile : Form
    {
        public delegate void updateBar(int value, int tot, string nomefile);
        public updateBar delegateUpdateBar;
        private bool terminate;
        private int perc;
        private bool showdialog = false;

        public void showdialogset(bool s)
        {

            this.showdialog = s;
        }
        public void UpdateProgress(long sizeFileSent, long dimFile, String FileName)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.BeginInvoke(new Action(() =>
                {
                    progressBar.Value += (int)((((long)progressBar.Maximum )* sizeFileSent) / dimFile);
                    perc += (int)sizeFileSent;
                    label1.Text = " UPLoad File " + perc / (1024) + "/ " + dimFile / (1024) + " KB" + "\n File: " + FileName;  
                }));
            }
            
        }

        public FormStatusFile(ref bool terminateSend)
        {
            InitializeComponent();
            terminate = terminateSend;
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.MarqueeAnimationSpeed = 10;
            this.MaximizeBox = false;
        }

        private void FormStatusFile_Load(object sender, EventArgs e)
        {
          
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            switch (MessageBox.Show(this, "Are you sure you want to stop file sending?", "Closing", MessageBoxButtons.YesNo))
            {
                case DialogResult.No:
                    break;
                default:
                    terminate = true;
                    this.Close();
                    break;
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            //  base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.ApplicationExitCall) return;
            if (e.CloseReason == CloseReason.FormOwnerClosing) return;

            if (e.CloseReason == CloseReason.UserClosing && !showdialog)
            // Confirm user wants to close
            {
                switch (MessageBox.Show(this, "Are you sure you want to close?", "Closing", MessageBoxButtons.YesNo))
                {
                    case DialogResult.No:
                        e.Cancel = true;
                        break;
                    default:
                        this.DialogResult = DialogResult.OK;
                        terminate = true;
                        break;
                }

            }

        }

        private void Label1_Click_1(object sender, EventArgs e)
        {

        }
    }

}
