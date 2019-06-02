using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Data;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Progetto
{
    public struct Value
    {
        public DateTime time;
        public string name;
        public Image photo;
        public IPAddress ip;
        public Int32 portImage;
        public Int32 portRequest;
    }

    class MainClass
    {
        private Settings setting;
        private Dictionary<IPAddress, Value> usersMap;
        private Mutex mutex_map;
        private Thread sendMulticast;
        private Thread receiveMulticast;
        private Thread manageMap;
        private Thread sendImageUnicast;
        private Thread receiveUnicast;
        private Thread shareForm;
        private List<Value> userToSendFile;
        public MainClass(Settings s)
        {
            setting = s;
            mutex_map = new Mutex();
            usersMap = new Dictionary<IPAddress, Value>();
        }

        ~MainClass()
        {
            sendMulticast.Join();
            receiveMulticast.Join();
            manageMap.Join();
            sendImageUnicast.Join();
            receiveUnicast.Join();
        }

        // thread principale, lancia 5 thread statici
        public void Start()
        {
            // listener TCP utilizzato in ProvidePhoto
            TCPClass imageSender = new TCPClass();
            int imagePort = imageSender.CreateListener(IPAddress.Any, 0);

            // receiver UDP utilizzato in ReceiveConnections
            UDPClass connectionReceiver = new UDPClass();
            int requestPort = connectionReceiver.Bind();

            // porte utilizzate da comunicare in SendPresentation
            string ports = imagePort + "_" + requestPort;

            sendImageUnicast = new Thread(ProvidePhoto);
            receiveUnicast = new Thread(ReceiveConnections);
            sendMulticast = new Thread(SendPresentation);
            receiveMulticast = new Thread(ReceivePresentations);
            manageMap = new Thread(CheckMap);

            //
            shareForm = new Thread(showFormSharing);

            sendImageUnicast.Start(imageSender);
            receiveUnicast.Start(connectionReceiver);
            sendMulticast.Start(ports);
            receiveMulticast.Start();
            manageMap.Start();

            shareForm.Start();
        }


        //  controllo degli utenti ancora connessi
        public void CheckMap()
        {
            while (true)
            {
                mutex_map.WaitOne();
                foreach (KeyValuePair<IPAddress, Value> entry in usersMap)
                {
                    if ((DateTime.Now.Millisecond - entry.Value.time.Millisecond) > 5000)
                    {
                        usersMap.Remove(entry.Key);
                    }
                }
                mutex_map.ReleaseMutex();
                Thread.Sleep(4000);
            }
           
        }
        public void MapRefresh(IPAddress ip, Value val)
        {
            Value v;
            if (usersMap.ContainsKey(ip))
            {
                //utente presente, quindi aggiorno il suo timestamp nella mappa ed eventualmente il nome
                usersMap.TryGetValue(ip, out v);
                v.time = DateTime.Now;
                if (val.name != v.name)
                    v.name = val.name;
            }
            else
            {
                //Utente non presente
                v.name = val.name;
                v.time = val.time;
                v.photo = null;
                v.ip = ip;
                v.portImage = val.portImage;
                v.portRequest = val.portRequest;

                //richiedo la foto
                TCPClass tcpclass = new TCPClass();
                tcpclass.CreateRequester();
                tcpclass.Connect(ip, val.portImage);

                tcpclass.SendMessage("Richiesta Foto");

                string response = tcpclass.ReceiveMessage(2);
                if (response == "ok")
                {
                    // foto presente
                    MemoryStream ms = new MemoryStream(tcpclass.ReceiveFile());
                    val.photo = Image.FromStream(ms);
                }

                //aggiungo il nuovo utente
                usersMap.Add(ip, v);
            }
        }

        // riceve i pacchetti di presentazione dal multicast
        public void ReceivePresentations()
        {
            UDPClass udp = new UDPClass();
            udp.MulticastSubscription();
            //Accettare i nuovi utenti ed eventualmente aggiornare la Mappa
            while (true)
            {
                Value v=udp.ReceiveWrapPacket();
                mutex_map.WaitOne();
                MapRefresh(v.ip, v);
                mutex_map.ReleaseMutex();
            }
        }

        // invia in multicast la presentazione name_portImage_portRequest ogni 5 secondi
        // solo in modalità pubblica
        public void SendPresentation(object ports)
        {
            UDPClass udp = new UDPClass();
            udp.Bind();
            while (true)
            {
                setting.publicMode.WaitOne();
                udp.SendPacketMulticast(setting.Name.Trim('_') + "_" + (string)ports);
                Thread.Sleep(5000);
            }
        }


        // invia l'immagine a chi la richiede
        // solo in modalità pubblica
        void ProvidePhoto(object imageSender)
        {
            TCPClass tcpImageSender = (TCPClass)imageSender;
            while (true)
            {
                setting.publicMode.WaitOne();
                tcpImageSender.AcceptConnection();
                string messreceive = tcpImageSender.ReceiveMessage(14); //FORSE NON SERVE QUESTO!
                if (setting.PhotoSelected == true)
                {
                    tcpImageSender.SendMessage("ok");
                    MemoryStream ms = new MemoryStream();
                    setting.Photo.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    tcpImageSender.SendFile(ms.ToArray());
                }
                else
                {
                    tcpImageSender.SendMessage("no");
                }
                tcpImageSender.CloseConnection();
            }
        }

        // riceve le richieste di connessione e lancia un thread per ogni ricezione file
        // solo in modalità pubblica
        void ReceiveConnections(object receiver)
        {
            string path;
            UDPClass udpConnectionsReceiver = (UDPClass)receiver;

            while (true)
            {
                setting.publicMode.WaitOne();
                IPEndPoint remote = udpConnectionsReceiver.ReceiveConnectionRequest();
                if (remote == null)
                    continue;
                // cerca il nome nella mappa, presente se il richiedente è in publicMode
                Value requester;
                string reqName;
                if (usersMap.TryGetValue(remote.Address, out requester) == true)
                    reqName = requester.name;
                else
                    reqName = "Un utente privato";

                // chiede conferma di ricezione se non è automatica
                if (setting.AutomaticReceive == false)
                {
                    FormConfirmReceive form1 = new FormConfirmReceive(reqName);
                    form1.Show();
                    if(form1.GetChoice() == false)
                    {
                        udpConnectionsReceiver.SendPacket("NO", remote);
                        continue;
                    }
                }

                // ricezione automatica o accettata; riceve nome e tipo file
                udpConnectionsReceiver.SendPacket("YES", remote);
                string filename = udpConnectionsReceiver.ReceivePacket(remote);
                string type = udpConnectionsReceiver.ReceivePacket(remote);

                if (setting.DefaultSelected == false)
                {
                    FormSelectPath form2 = new FormSelectPath(filename);
                    path = form2.GetPath();
                }
                else
                {
                    path = setting.DefaultPath;
                }

                //verifica unicità path ed eventualmente lo modifica
                path = path + "/" + filename;
                int count = 0;
                string modifiedPath = path;
                if (type == "File")
                {
                    while (File.Exists(modifiedPath) == true)
                    {
                        modifiedPath = path + "(" + count + ")";
                        count++;
                    }
                }
                else
                {
                    while (Directory.Exists(modifiedPath) == true)
                    {
                        modifiedPath = path + "(" + count + ")";
                        count++;
                    }
                }

                //crea tcp receiver e invia la porta scelta con udp
                TCPClass tcpReceiver = new TCPClass();
                int tcpPort = tcpReceiver.CreateListener(IPAddress.Any, 0);
                udpConnectionsReceiver.SendPacket(tcpPort.ToString(), remote);

                //sgancia thread ricezione tcp
                ThreadPool.QueueUserWorkItem(tcpReceiver.ReceiveFileBuffered, filename);
            }
        }

        public void showFormSharing()
        {
            string pathbefore = System.Environment.GetEnvironmentVariable("envvar", EnvironmentVariableTarget.User);
            while (string.Compare(pathbefore, System.Environment.GetEnvironmentVariable("envvar", EnvironmentVariableTarget.User)) == 0)
            {
                Thread.Sleep(1);
            }
            string pathnew = System.Environment.GetEnvironmentVariable("envvar", EnvironmentVariableTarget.User);
            //MessageBox.Show(pathnew);
            System.Environment.SetEnvironmentVariable("envvar", "", EnvironmentVariableTarget.User);
            FormSharing share = new FormSharing(usersMap, pathnew, setting);
            Application.Run(share);

        }

        public void SendConnection(object sender)
        {
            //Send to each userselected the pathfile
            string filename = (string)sender;
            UDPClass udpConnectionsSender = new UDPClass();

            while (true)
            {
                Value user = userToSendFile.LastOrDefault();
                UDPClass udp = new UDPClass();
                udp.MulticastSubscription();

                IPEndPoint send = new IPEndPoint(user.ip, udp.Bind());
                udpConnectionsSender.SendPacket("SEND FILE", send);
                string receiveaccess = udpConnectionsSender.ReceivePacket(send);
                if (string.Compare(receiveaccess, "YES") == 0)
                {
                    //Invio del file
                    TCPClass tcp = new TCPClass();
                    tcp.CreateRequester();
                    tcp.Connect(send.Address, send.Port);
                    byte[] bytes = System.IO.File.ReadAllBytes(filename);
                    bool t = true;
                    tcp.SendFileBuffered(bytes, ref t);
                    //GESTIRE CON Exception la chiusura del thred in caso l invio del file è stato annullato!
                }
                else
                {
                    //Invio rifiutato
                    MessageBox.Show("User has rejected the File!");
                    continue;
                }

            }
        }

        public void SendFile(Dictionary<IPAddress, Value> UserToSend, string filename)
        {
            userToSendFile = new List<Value>();
            
            foreach (KeyValuePair<IPAddress, Value> entry in UserToSend)
            {
                //mutex
                userToSendFile.Add(entry.Value);
                ThreadPool.QueueUserWorkItem(this.SendConnection, filename);
                
            }
        }
    }
}
