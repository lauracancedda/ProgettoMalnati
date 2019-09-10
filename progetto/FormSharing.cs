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
    static class Constants
    {
        public const int xInit = 20;
        public const int yInit = 20;
    }

    public partial class FormSharing : Form
    {
        private Dictionary<IPAddress, Value> onlineUsers;
        private Mutex mutexMap;
        private List<Card> displayedUsers;
        private List<Value> selectedUsers;
        private int x;
        private int y;
        private Label empty = new Label
        {
            Text = "Non ci sono utenti online",
            Name = "empty",
            Location = new Point(10, 10),
            AutoSize = true
        };

        public FormSharing(Dictionary<IPAddress, Value> onlineUsers, Mutex mutexMap)
        {
            InitializeComponent();
            this.onlineUsers = onlineUsers;
            this.mutexMap = mutexMap;
            displayedUsers = new List<Card>();
            selectedUsers = new List<Value>();
            this.DialogResult = DialogResult.Cancel;
            // Form layout
            this.Padding = new Padding(5);
            this.BackColor = Color.LightBlue;
            drawUsers();
        }

        private void drawUsers()
        {
            x = Constants.xInit;
            y = Constants.yInit;

            mutexMap.WaitOne();
            if (onlineUsers.Count == 0)
            {
                this.Controls.Add(empty);
                this.Send.Visible = false;
            }
            else
                this.Send.Visible = true;

            foreach (KeyValuePair<IPAddress, Value> entry in onlineUsers)
            {
                Card userCard;

                //Image Utente
                userCard.picture = new PictureBox
                {
                    Name = "photo",
                    Image = entry.Value.photo,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Size = new Size(125, 125),
                    Margin = new Padding(5, 0, 5, 0)
                };
                if (userCard.picture.Image == null)
                    userCard.picture.Image = Progetto.Properties.Resources.user;

                //Checkbox Utente
                userCard.checkbox = new CheckBox
                {
                    Name = entry.Value.name,
                    AutoSize = true
                };

                //Nome UTente
                userCard.label = new Label
                {
                    Text = entry.Value.name,
                    Name = entry.Value.name,
                    AutoSize = true,
                    Padding = new Padding(0, 3, 0, 0)
                };


                //Layout
                userCard.layout = new FlowLayoutPanel
                {
                    Location = new Point(x, y),
                    Size = new Size(150, 150),
                    Padding = new Padding(5),
                };

                FlowLayoutPanel bottomPanel = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = false,
                    Size = new Size(150, 25),
                };

                bottomPanel.SuspendLayout();
                bottomPanel.Controls.Add(userCard.checkbox);
                bottomPanel.Controls.Add(userCard.label);
                bottomPanel.ResumeLayout();

                userCard.layout.SuspendLayout();
                userCard.layout.Controls.Add(userCard.picture);
                userCard.layout.Controls.Add(bottomPanel);
                userCard.layout.ResumeLayout();

                //value Utente
                userCard.val = entry.Value;
                displayedUsers.Add(userCard);

                this.Controls.Add(userCard.layout);
                // aggiorna coordinate prossimo utente
                x += 170;
                if (x >= 600)
                {
                    x = Constants.xInit;
                    y += 170;
                }
            }
            mutexMap.ReleaseMutex();
            this.Send.Top = y + 250;
        }

        public List<Value> getSelectedUsers()
        {
            return selectedUsers;
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
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void FormSharing_Load(object sender, EventArgs e)
        {
            // aggiorna il form ogni 2 secondi
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = (2 * 1000);
            timer.Tick += new EventHandler(updateForm);
            timer.Start();
        }

        private void updateForm(object sender, EventArgs e)
        {
            // controlla se il contenuto del form coincide con il contenuto della mappa
            Boolean contentChanged = false;
            mutexMap.WaitOne();
            if (onlineUsers.Count != displayedUsers.Count)
                contentChanged = true;
            if(contentChanged == false)
            {
                foreach (Card userCard in displayedUsers)
                {
                    if (!onlineUsers.ContainsKey(userCard.val.ip))
                        contentChanged = true;
                }
            }
            mutexMap.ReleaseMutex();

            // contenuti diversi, ridisegna il form
            if (contentChanged)
            {
                // le card vengono eliminate dalla memoria, il label empty solo nascosto
                foreach (Card c in displayedUsers)
                {
                    FlowLayoutPanel u = c.layout;
                    u.Dispose();
                }
                this.Controls.Remove(empty);
                displayedUsers.Clear();
                drawUsers();
            }
        }
    }
}
