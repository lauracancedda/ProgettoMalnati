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
        private bool showdialog = false;

        public void showdialogset(bool s)
        {

            this.showdialog = s;
        }
        public void UpdateProgress(ref bool terminateRef, long actualReceived, long dimFile, String FileName)
        {
            if (terminate == true)
            {
                this.showdialogset(true);
                terminateRef = true;
            }
               
            if (progressBar.InvokeRequired)
            {
                progressBar.BeginInvoke(new Action(() =>
                {

                    progressBar.Value = ((int)((((long)progressBar.Maximum )* actualReceived) / dimFile));
                   if(dimFile / 1024 == 0)
                        label1.Text = " UPLoad File " + actualReceived + "/" + dimFile + " KB" + "\n File: " + FileName;
                    else
                        label1.Text = " UPLoad File " + actualReceived/1024 + "/" + dimFile/1024 + " MB" + "\n File: " + FileName;
                }));
            }
            
        }

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
                    this.showdialogset(true);
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
                        terminate = true;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        break;
                }

            }

        }

        private void Label1_Click_1(object sender, EventArgs e)
        {

        }
    }

}
