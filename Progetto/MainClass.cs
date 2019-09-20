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
using System.Net.Sockets;
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
        public Mutex mutex_map;
        private Thread sendMulticast = null;
        private Thread receiveMulticast = null;
        private Thread manageMap = null;
        private Thread sendImageUnicast = null;
        private Thread receiveUnicast = null;
        private Thread shareForm = null;
        
        private static long ONE_GIG = 1073741824;
        public MainClass(ref Settings s)
        {
            setting = s;
            mutex_map = new Mutex();
            usersMap = new Dictionary<IPAddress, Value>();
        }

        ~MainClass()
        {
            if (sendImageUnicast != null)
            {
                sendImageUnicast.Join();
                sendImageUnicast = null;
            }
            if (receiveMulticast != null)
            {
                receiveUnicast.Join();
                receiveUnicast = null;
            }
            if (sendMulticast != null)
            {
                sendMulticast.Join();
                sendMulticast = null;
            }
            if (receiveMulticast != null)
            {
                receiveMulticast.Join();
                receiveMulticast = null;
            }
            if (manageMap != null)
            {
                manageMap.Join();
                manageMap = null;
            }
            if (shareForm != null)
            {
                shareForm.Join();
                shareForm = null;
            }

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
            while (!TerminationHandler.Instance.isTerminationRequired())
            {
                // ciclo inverso per modificare la mappa mentre viene attraversata
                for (int i = usersMap.Count - 1; i >= 0; i--)
                {
                    DateTime currentTime = usersMap.ElementAt(i).Value.time;
                    TimeSpan elapsedTime = DateTime.Now.Subtract(currentTime);
                    if (elapsedTime.TotalMilliseconds > 3000)
                    {
                        mutex_map.WaitOne();
                        usersMap.Remove(usersMap.ElementAt(i).Key);
                        mutex_map.ReleaseMutex();
                    }
                }
                Thread.Sleep(3000);
            }
            Console.WriteLine("CheckMap terminated");
        }
        public void MapRefresh(IPAddress ip, Value val)
        {
            Value v = new Value();
            v.name = val.name;
            v.time = DateTime.Now;
            v.portImage = val.portImage;
            v.portRequest = val.portRequest;
            v.ip = ip;

            if (usersMap.ContainsKey(ip))
            {
                //utente presente, lo aggiorno
                v.photo = usersMap[ip].photo;
                usersMap[ip] = v;
            }
            else
            {
                //Utente non presente, richiedo la foto
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
            while (!TerminationHandler.Instance.isTerminationRequired())
            {
                try
                {
                    Value v = udp.ReceiveWrapPacket();
                    if (v.name != setting.Name)
                    {
                        mutex_map.WaitOne();
                        MapRefresh(v.ip, v);
                        mutex_map.ReleaseMutex();
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            Console.WriteLine("ReceivePresentations terminated");
        }

        // invia in multicast la presentazione name_portImage_portRequest ogni 5 secondi
        // solo in modalità pubblica
        public void SendPresentation(object ports)
        {

            UDPClass udp = new UDPClass();
            udp.Bind();
            while (!TerminationHandler.Instance.isTerminationRequired())
            {
                try
                {
                    setting.publicMode.WaitOne();
                    udp.SendPacketMulticast(setting.Name.Trim('_') + "_" + (string)ports);
                    Thread.Sleep(2000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            Console.WriteLine("SendPresentation terminated");
        }


        // invia l'immagine a chi la richiede
        // solo in modalità pubblica
        void ProvidePhoto(object imageSender)
        {
            TCPClass tcpImageSender = (TCPClass)imageSender;
            while (!TerminationHandler.Instance.isTerminationRequired())
            {
                setting.publicMode.WaitOne();
                // accept connection si sblocca e ritorna automaticamente se la terminazione è richiesta
                tcpImageSender.AcceptConnection();
                if (!TerminationHandler.Instance.isTerminationRequired())
                {
                    string request = tcpImageSender.ReceiveMessage(14);
                    if (setting.PhotoSelected == true)
                    {
                        tcpImageSender.SendMessage("ok");
                        MemoryStream ms = new MemoryStream();
                        setting.Photo.Save(ms, setting.Photo.RawFormat);
                        tcpImageSender.SendFile(ms.ToArray());
                    }
                    else
                    {
                        tcpImageSender.SendMessage("no");
                    }
                    tcpImageSender.CloseConnection();
                }
            }
            Console.WriteLine("ProvidePhoto terminated");
        }

        // riceve le richieste di connessione e lancia un thread per ogni ricezione file
        // solo in modalità pubblica
        void ReceiveConnections(object receiver)
        {
            string path;
            UDPClass udpConnectionsReceiver = (UDPClass)receiver;
            // check if we need to terminate the loop
            while (!TerminationHandler.Instance.isTerminationRequired())
            {
                setting.publicMode.WaitOne();
                if (!TerminationHandler.Instance.isTerminationRequired())
                {
                    IPEndPoint remote = udpConnectionsReceiver.ReceiveConnectionRequest();
                    if (remote == null)
                        continue;
                    try
                    {
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
                        int nFileToReceive = Int32.Parse(udpConnectionsReceiver.ReceivePacket(remote));
                        for (int z = 0; z < nFileToReceive; z++)
                        {
                            string filename = udpConnectionsReceiver.ReceivePacket(remote);
                            string type = udpConnectionsReceiver.ReceivePacket(remote);

                            if (setting.DefaultSelected == false)
                            {
                                FormSelectPath form2 = new FormSelectPath(filename);
                                form2.ShowDialog();
                                path = form2.GetPath();
                                form2.Dispose();
                            }
                            else
                            {
                                path = setting.DefaultPath;
                            }


                            path = path + "\\";

                            //crea tcp receiver e invia la porta scelta con udp
                            TCPClass tcpReceiver = new TCPClass();
                            int tcpPort = tcpReceiver.CreateListener(IPAddress.Any, 0);
                            udpConnectionsReceiver.SendPacket(tcpPort.ToString(), remote);

                            //sgancia thread ricezione tcp
                            ThreadPool.QueueUserWorkItem(tcpReceiver.ReceiveFileBuffered, new String[] { path, type, filename });
                        }
                    }
                    catch (SocketException ex)
                    {
                        MessageBox.Show("Ricezione non riuscita: problema di connessione \n" + ex.Message);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        MessageBox.Show("Ricezione non riuscita: Non è stato possibile recuperare il nome del file da inviave \n" + ex.Message);
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show("Ricezione non riuscita: non è possibile ricevere file con caratteri speciali \n cambia il nome del file in invio e riprova" + ex.Message);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show("Ricezione non riuscita: il file che si sta provando a ricevere è in uso da un altro processo \n" + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ricezione non riuscita: non è stato possibile inviare il file" + ex.Message);
                    }
                }
            }
            Console.WriteLine("ReceiveConnection terminated");
        }


        // invia le richieste di connessione e lancia un thread per ogni destinatario
        public void SendConnection(List<Value> usersSelected, String []files)
        {
            String filename = null;
            FileAttributes attributes;
            long dimForCheck = 0;
            UDPClass udpClient = new UDPClass();
            udpClient.Bind();
            foreach (Value user in usersSelected)
            {
                try
                {   // ho bisogno di catchare nel caso in cui il ricevitore non risponsa entro il limite di tempo
                    IPEndPoint udpEndPoint = new IPEndPoint(user.ip, user.portRequest);
                    udpClient.SendConnectionRequest(udpEndPoint);
                    string answer = udpClient.ReceivePacket(udpEndPoint);
                    if (answer == "YES")
                    {
                        udpClient.SendPacket(files.Length.ToString(), udpEndPoint); // Dici quanti file devi inviare

                        for (int j = 0; j < files.Length; j++)
                        {
                            try
                            {
                                filename = files[j].Substring(files[j].LastIndexOf('\\')).Replace("\\", "");
                                attributes = File.GetAttributes(files[j]);
                                dimForCheck = 0;
                                // voglio avvisare che il file che l'utente sta per inviare è grande ne prendo la dimensione
                                if (attributes.HasFlag(FileAttributes.Directory))
                                {
                                    dimForCheck = Directory.GetFiles(files[j], "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                                }
                                else
                                {
                                    byte[] file = File.ReadAllBytes(files[j]);
                                    dimForCheck = file.LongLength;
                                }

                                //controllo se la dimensione è piu grande di un gigaByte
                                // ho provato a cercare un giga come variabile constante in c# ma non la trovo
                                // corretto ponerla int64?
                                if (dimForCheck > ONE_GIG)
                                {
                                    DialogResult dialogResult = MessageBox.Show("Il file/cartella che stai provando ad inviare è molto grande. " +
                                        "Sei sicuro? Potrebbe richiedere tempo la compressione prima dell'invio", "Invio File", MessageBoxButtons.YesNo);

                                    if (dialogResult == DialogResult.No)
                                    {
                                        return;
                                    }
                                }
                                // crea cartella zippata nella cartella di progetto e sostituisce filepath e filename
                                string projectPath = Environment.CurrentDirectory;
                                if (attributes.HasFlag(FileAttributes.Directory))
                                {
                                    // concateno l estension
                                    String zipFolderPath = projectPath + "\\" + filename + ".zip";
                                    // se il file esiste lo elimino
                                    if (File.Exists(zipFolderPath))
                                        File.Delete(zipFolderPath);
                                    // creo lo zip
                                    // questo potrebbe richiedere del tempo
                                    ZipFile.CreateFromDirectory(files[j], zipFolderPath);
                                    filename = filename + ".zip";
                                    files[j] = zipFolderPath;
                                }

                                udpClient.SendPacket(filename, udpEndPoint);
                                if (attributes.HasFlag(FileAttributes.Directory))
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
                                ThreadPool.QueueUserWorkItem(tcpSender.SendFileBuffered, files[j]);
                            }
                            catch (SocketException ex)
                            {
                                MessageBox.Show("Invio non riuscito del file " + filename + "\n controlla la connessione: \n" + ex.Message);
                            }
                            catch (ArgumentOutOfRangeException ex)
                            {
                                MessageBox.Show("Invio non riuscito:  del file " + filename + "\n Non è stato possibile recuperare il nome del file da inviave \n" + ex.Message);
                            }
                            catch (ArgumentException ex)
                            {
                                MessageBox.Show("Invio non riuscito del file " + filename + "\n : non è possibile inviare file con caratteri speciali \n cambia il nome del file e riprova\n" + ex.Message);
                            }
                            catch (IOException ex)
                            {
                                MessageBox.Show("Invio non riuscito  del file " + filename + "\n: il file che si sta provando a inviare è in uso da un altro processo \n" + ex.Message);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Ricezione non riuscita  del file " + filename + "\n: non è stato possibile inviare il file\n" + ex.Message);
                            }
                        }

                    }
                    else
                    {
                        //Invio rifiutato
                        MessageBox.Show("Invio non riuscito: l'utente ha rifiutato la ricezione");
                    }
                }
                catch (SocketException ex)
                {
                    MessageBox.Show("Risposta non ricevuto dall'host " + user.name + "riprova \n" + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Errore nell'invio dei file all'host " + user.name + ex.Message);
                }
            }
        }

        public void ShowFormSharing()
        {
            int pathLength;
            int numberOfFile = 0;
            String []filesToSend;
            // creo la pipe per ricevere i file da inviare
            while (!TerminationHandler.Instance.isTerminationRequired())
            {
                using (NamedPipeClientStream namedPipeClient = new NamedPipeClientStream("pipe-project"))
                {
               
                    try
                    {
                        // timeout di 5 secondi per connettere la pipe
                        namedPipeClient.Connect(5000);
                        // creo un array con i file da inviare con la dimensione che ricevo
                        numberOfFile = namedPipeClient.ReadByte();
                        filesToSend = new string[numberOfFile];
                        for (int i = 0; i < numberOfFile; i++)
                        {
                            // metto in ascolto del file
                            pathLength = namedPipeClient.ReadByte();
                            Thread.Sleep(100);
                            byte[] path = new byte[pathLength];
                            // la lettura da pipe e' bloccante, quindi si aspetta che l'altro processo invii il path
                            namedPipeClient.Read(path, 0, pathLength);
                            // prendo il nome del file
                            String filePath = System.Text.Encoding.UTF8.GetString(path);
                            // lo inserisco nell'array
                            filesToSend[i] = filePath;

                        }

                        FormSharing formSharing = new FormSharing(usersMap, mutex_map);
                        // controllare corsa critica per show e get dei valori
                        if (formSharing.ShowDialog() == DialogResult.OK)
                        {
                            if (formSharing.getSelectedUsers().Count > 0)
                                ThreadPool.QueueUserWorkItem(unused => this.SendConnection(formSharing.getSelectedUsers(), filesToSend));
                           
                            formSharing.Dispose();
                        }
                    }
                    catch (TimeoutException time)
                    {
                        Console.WriteLine("PipeTimeout" + time.Message);
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine("La formSHaring si è chiusa dalla x senza selezionare utenti");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Errore nella selezione dei file da inviare - " + ex.Message);
                    }

                    namedPipeClient.Close();
                }
            }
            Console.WriteLine("Thread ShowFormSharing terminated");
        }
    }
}
