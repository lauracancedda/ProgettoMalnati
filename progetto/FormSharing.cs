using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Progetto
{
    struct Card
    {
        public FlowLayoutPanel layout;
        public PictureBox picture;
        public Label label;
        public CheckBox checkbox;
        public Value val;
    }

    public partial class FormSharing : Form
    {
        private List<Value> selectedUsers;
        private List<Card> displayedUsers;
        
        public FormSharing(ref Dictionary<IPAddress, Value> onlineUsers, Settings setting)
        {
            InitializeComponent();
            displayedUsers = new List<Card>();
            selectedUsers = new List<Value>();

            int x, y;
            x = 1; y = 1;
            
            if(onlineUsers.Count == 0)
            {
                Label empty = new Label
                {
                    Text = "Non ci sono utenti online",
                    Name = "empty"
                };
                this.Controls.Add(empty);
                this.Send.Visible = false;
            }

            foreach (KeyValuePair<IPAddress, Value> entry in onlineUsers)
            {
                Card userCard;

                //Nome UTente
                userCard.label = new Label
                {
                    Text = entry.Value.name,
                    Name = entry.Value.name
                };
                //Image Utente
                userCard.picture = new PictureBox
                {
                    Name = "photo",
                    Image = entry.Value.photo,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Size = new Size(75, 75)
                };

                //Checkbox Utente
                userCard.checkbox = new CheckBox
                {
                    Name = entry.Value.name
                };

                //Layout
                userCard.layout = new FlowLayoutPanel
                {
                    Location = new Point(x, y),
                    Size = new Size(100, 150),
                    BorderStyle = BorderStyle.Fixed3D
                };
                userCard.layout.Controls.Add(userCard.label);
                userCard.layout.Controls.Add(userCard.picture);
                userCard.layout.Controls.Add(userCard.checkbox);

                //value Utente
                userCard.val = entry.Value;
                displayedUsers.Add(userCard);

                this.Controls.Add(userCard.layout);

                // aggiorna coordinate prossimo utente
                x += 110;
                if (x >= 1000)
                {
                    x = 0;
                    y += 150;
                }
            }
        }

        public List<Value> getSelectedUsers()
        {
            return selectedUsers;
        }

        private void FormSharing_Load(object sender, EventArgs e)
        {

        }

        
        private void Send_Click(object sender, EventArgs e)
        {
            foreach (Card userCard in displayedUsers)
            {
                if (userCard.checkbox.Checked)
                {
                    selectedUsers.Add(userCard.val);
                }
            }
            this.Close();
        }
    }
}
