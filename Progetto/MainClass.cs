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
using System.IO.Pipes;
using System.IO.Compression;
using System.Text.RegularExpressions;

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
        private string filePath;
        Regex rx = new Regex(@"\s(<version>)",
         RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public MainClass(ref Settings s)
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
            shareForm.Join();
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

            // set variabile d'ambiente
            System.Environment.SetEnvironmentVariable("envvar", "", EnvironmentVariableTarget.User);

            sendImageUnicast = new Thread(ProvidePhoto);
            receiveUnicast = new Thread(ReceiveConnections);
            sendMulticast = new Thread(SendPresentation);
            receiveMulticast = new Thread(ReceivePresentations);
            manageMap = new Thread(CheckMap);
            shareForm = new Thread(ShowFormSharing);

            sendImageUnicast.Start(imageSender);
            // per consentire al thread di gestire n form: i form coinvolti sono FormConfirmReceive e FormSelectPath
            receiveUnicast.SetApartmentState(ApartmentState.STA);
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
                foreach (KeyValuePair<IPAddress, Value> entry in usersMap)
                {
                    if ((DateTime.Now.Millisecond - entry.Value.time.Millisecond) > 5000)
                    {
                        mutex_map.WaitOne();
                        usersMap.Remove(entry.Key);
                        mutex_map.ReleaseMutex();
                    }
                }
                Thread.Sleep(4000);
            }
        }
        public void MapRefresh(IPAddress ip, Value val)
        {
            Value v;
            if (usersMap.ContainsKey(ip))
            {
                //utente presente, quindi aggiorno il suo timestamp nella mappa, il nome e le porte
                usersMap.TryGetValue(ip, out v);
                v.time = DateTime.Now;
                v.name = val.name;
                v.portImage = val.portImage;
                v.portRequest = val.portRequest;
            }
            else
            {
                //Utente non presente
                v.name = val.name;
                v.time = val.time;

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
                    v.photo = Image.FromStream(ms);
                    v.photo.Save(v.ip.ToString() + "_" + v.name);
                }
                else
                {
                    v.photo = null;
                }

                //aggiungo il nuovo utente
                usersMap.Add(ip, v);
                Console.WriteLine("Aggiunto alla mappa l'utente: {0}", v.name);
            }
        }

        // riceve i pacchetti di presentazione dal multicast
        public void ReceivePresentations()
        {
            UDPClass udp = new UDPClass();
            udp.MulticastSubscription();
            //Accettare i nuovi utenti (scartando se stesso) ed eventualmente aggiornare la Mappa
            while (true)
            {
                Value v = udp.ReceiveWrapPacket();
                if (v.name != setting.Name)
                {
                    mutex_map.WaitOne();
                    MapRefresh(v.ip, v);
                    mutex_map.ReleaseMutex();
                }
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
                string request = tcpImageSender.ReceiveMessage(14);
                if (setting.PhotoSelected == true)
                {
                    tcpImageSender.SendMessage("ok");
                    MemoryStream ms = new MemoryStream();
                    setting.Photo.Save(ms, setting.Photo.RawFormat);//System.Drawing.Imaging.ImageFormat.Jpeg); 
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
                    form1.ShowDialog();
                    if (form1.GetChoice() == false)
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
                    form2.ControlBox = false;
                    form2.ShowDialog();
                        path = form2.GetPath();
                    form2.Dispose();
                }
                else
                {
                    path = setting.DefaultPath;
                }

                //verifica unicità path ed eventualmente lo modifica
                string modifiedPath = path;
                path = path + "\\";
                int count = 1;
                string format = filename.Substring(filename.LastIndexOf('.'));
                filename = filename.TrimEnd(format.ToCharArray());
               

                filename = filename.TrimEnd(format.ToCharArray());
                string filenamefix = filename;
                string modifiedFilename = filename;
                if (type == "File")
                {
                    while (File.Exists(modifiedPath + filename + format) == true)
                    {
                        filename = filenamefix + "(" + count + ")";
                        count++;
                    }
                }
                else
                {

                    while (Directory.Exists(modifiedPath  + filename) == true)
                    {
                        filename = filenamefix + "(" + count + ")";
                        //modifiedPath = path + modifiedFilename;
                        count++;
                    }
                    //filename = modifiedFilename;
                }
                modifiedPath = modifiedPath + filename + format;

                //crea tcp receiver e invia la porta scelta con udp
                TCPClass tcpReceiver = new TCPClass();
                int tcpPort = tcpReceiver.CreateListener(IPAddress.Any, 0);
                udpConnectionsReceiver.SendPacket(tcpPort.ToString(), remote);

                //sgancia thread ricezione tcp
                ThreadPool.QueueUserWorkItem(tcpReceiver.ReceiveFileBuffered, new String[] { modifiedPath, type });
            }
        }


        // invia le richieste di connessione e lancia un thread per ogni destinatario
        public void SendConnection(object users)
        {
            String filename = filePath.Substring(filePath.LastIndexOf('\\')); //Name File or directory
            FileAttributes attributes = File.GetAttributes(filePath);
            List< Value> usersSelected = (List< Value>) users;
            string pathproject = Environment.CurrentDirectory;
            if (attributes.HasFlag(FileAttributes.Directory))
            {
                // TODO: manage of expression if there is some problem with zip
                String pathZipFile = pathproject + "\\" + filename + ".zip";
                if (File.Exists(pathZipFile))
                    File.Delete(pathZipFile);
                ZipFile.CreateFromDirectory(filePath, pathZipFile);
                filename = filename + ".zip";
                filePath = pathZipFile;
            }
               
            UDPClass udpClient = new UDPClass();
            udpClient.Bind();

            foreach (Value user in usersSelected)
            {
                //invio filename e tipo
               
                IPEndPoint udpEndPoint = new IPEndPoint(user.ip, user.portRequest);
                udpClient.SendConnectionRequest(udpEndPoint);
                string answer = udpClient.ReceivePacket(udpEndPoint);
                if (answer == "YES")
                {
                    
                    udpClient.SendPacket(filename, udpEndPoint);
                    if(attributes.HasFlag(FileAttributes.Directory))
                        udpClient.SendPacket("Directory", udpEndPoint);
                    else
                        udpClient.SendPacket("File", udpEndPoint);

                    // ricezione porta TCP da usare per inviare il file
                    string tcpPort = udpClient.ReceivePacket(udpEndPoint);

                    //Invio del file
                    TCPClass tcpSender = new TCPClass();
                    tcpSender.CreateRequester();
                    tcpSender.Connect(user.ip, Int32.Parse(tcpPort));

                    //sgancia thread invio tcp
                    ThreadPool.QueueUserWorkItem(tcpSender.SendFileBuffered, filePath);
                }
                else
                {
                    //Invio rifiutato
                    MessageBox.Show("Invio non riuscito: l'utente ha rifiutato la ricezione");
                }
            }
        }

        public void ShowFormSharing()
        {
            while (true)
            {
                using (NamedPipeClientStream namedPipeClient = new NamedPipeClientStream("pipe-project"))
                {
                    namedPipeClient.Connect();
                    int pathLength;
                    pathLength = namedPipeClient.ReadByte();
                    Thread.Sleep(100);
                    byte[] path = new byte[pathLength];
                    // la lettura da pipe e' bloccante, quindi si aspetta che l'altro processo invii il path
                    namedPipeClient.Read(path, 0, pathLength);
                    filePath = System.Text.Encoding.UTF8.GetString(path);
                    FormSharing formSharing = new FormSharing(ref usersMap, setting);
                    formSharing.ShowDialog(); // controllare corsa critica per show e get dei valori
                    if (formSharing.getSelectedUsers().Count > 0)
                        ThreadPool.QueueUserWorkItem(this.SendConnection, formSharing.getSelectedUsers());
                    Console.WriteLine("Ricevuto tramite pipe il path " + filePath);
                    namedPipeClient.Close();
                }
            }

        }
    }
}
