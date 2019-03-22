using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Progetto
{
    public partial class FormSharing : Form
    {
        private Dictionary<IPAddress, Value> OnlineUsers;
        //private Dictionary<IPAddress, value> SelectedUsers;
        private List<visualization> userselected;
        struct visualization
        {
            public FlowLayoutPanel layout;
            public PictureBox picture;
            public Label label;
            public CheckBox checkbox;
            public Value val;

        }
        public FormSharing(Dictionary<IPAddress, Value> OnlineUsers1, string file, Settings setting)
        {
            InitializeComponent();
            OnlineUsers = OnlineUsers1;
            userselected = new List<visualization>();
            // Lock Map - Read Map - Create form for each element into the map
            foreach (KeyValuePair<IPAddress, Value> entry in OnlineUsers1)
            {
                //entry.Value...
                // visualizzare gli users, quindi crere il form adatto
                Console.WriteLine(entry.Value.name.ToString());
                FlowLayoutPanel flp1 = new FlowLayoutPanel();
                CheckBox cb1 = new CheckBox();
                cb1.Text = entry.Value.name.ToString();
                cb1.Enabled = true;
                cb1.Visible = true;


                flp1.Controls.Add(cb1); // checksbox1
                flp1.TabIndex = 1;
            }
        }

        private void FormSharing_Load(object sender, EventArgs e)
        {
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int x, y;
            x = 1; y = 1;//Per poter disporre i vari layout utente
            foreach (KeyValuePair<IPAddress, Value> entry in OnlineUsers)
            {
                visualization v1;

                //Nome UTente
                v1.label = new Label();
                v1.label.Text = entry.Value.name;
                v1.label.Name = entry.Value.name;

                //Image Utente
                v1.picture = new PictureBox();
                v1.picture.Name = "Im1";
                v1.picture.Image = entry.Value.photo;
                v1.picture.SizeMode = PictureBoxSizeMode.StretchImage;
                v1.picture.Size = new Size(75, 75);

                //Checkbox Utente
                v1.checkbox = new CheckBox();
                v1.checkbox.Name = entry.Value.name;

                //Layout
                v1.layout = new FlowLayoutPanel();
                v1.layout.Location = new Point(x, y);
                v1.layout.Size = new Size(100, 150);
                v1.layout.BorderStyle = BorderStyle.Fixed3D;
                v1.layout.Controls.Add(v1.label);
                v1.layout.Controls.Add(v1.picture);
                v1.layout.Controls.Add(v1.checkbox);

                //value Utente
                v1.val = entry.Value;
                userselected.Add(v1);
                //this.Controls.Add(v1.label);
                //this.Controls.Add(v1.picture);
                //this.Controls.Add(v1.checkbox);
                this.Controls.Add(v1.layout);

                x += 100;
                if (x >= 1000)
                {
                    x = 0;
                    y += 100;
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s = "";
            foreach (visualization x in userselected)
            {
                if (x.checkbox.Checked)
                {
                    s += x.val.name + x.val.ip.ToString() + " - ";
                    int porta = 150;
                    bool r = true;
                    //SGANGIARE THREDD PER OGNI INVIO FILE!!!
                    TCPClass tcp = new TCPClass();
                    //tcp.Connect(x.val.ip, porta);
                    //openFileDialog1.ShowDialog();
                    //var result = dialog.ShowDialog();
                    //if (result != DialogResult.OK)
                    //    return;//
                    //byte[] file = System.IO.File.ReadAllBytes(openFileDialog1.FileName);
                    //tcp.SendFileBuffered(file,ref r);
                    //CREARE FORM PER SAPERE STATO CONNESSIONE usando il metodo di TCPCLASS StatusSendFile()
                }
            }
            label1.Text = s;
            
        }
    }
}
