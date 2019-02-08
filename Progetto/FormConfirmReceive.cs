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
    public partial class FormConfirmReceive : Form
    {
        bool choice;

        public FormConfirmReceive(string name)
        {
            InitializeComponent();
            this.RequestText.Text = name + " vorrebbe inviarti un file:";

        }

        public bool GetChoice()
        {
            return choice;
        }

        private void Accept_Click(object sender, EventArgs e)
        {
            choice = true;
        }

        private void Refuse_Click(object sender, EventArgs e)
        {
            choice = false;
        }
    }
}
