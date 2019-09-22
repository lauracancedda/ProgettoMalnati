using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.IO.Pipes;
using System.Net.Sockets;
using System.IO.Compression;

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
        public Mutex mutexMap;
        private Thread sendMulticast = null;
        private Thread receiveMulticast = null;
        private Thread manageMap = null;
        private Thread sendImageUnicast = null;
        private Thread receiveUnicast = null;
        private Thread shareFiles = null;
        
        public MainClass(ref Settings s)
        {
            setting = s;
            mutexMap = new Mutex();
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
            if (shareFiles != null)
            {
                shareFiles.Join();
                shareFiles = null;
            }

        }

        // thread principale, lancia 6 thread statici
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
            shareFiles = new Thread(ShowFormSharing);

            sendImageUnicast.Start(imageSender);
            receiveUnicast.SetApartmentState(ApartmentState.STA); // consente al thread di gestire n form: FormConfirmReceive e FormSelectPath
            receiveUnicast.Start(connectionReceiver);
            sendMulticast.Start(ports);
            receiveMulticast.Start();
            manageMap.Start();
            shareFiles.Start();
        }


        //  rimuove dalla mappa gli utenti non più connessi
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
                        mutexMap.WaitOne();
                        usersMap.Remove(usersMap.ElementAt(i).Key);
                        mutexMap.ReleaseMutex();
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
            v.photo = null;

            if (usersMap.ContainsKey(ip))
            {
                //utente presente, lo aggiorno
                v.photo = usersMap[ip].photo;
                usersMap[ip] = v;
            }
            else
            {
                //Utente non presente, richiedo la foto
                try
                {
                    TCPClass tcpclass = new TCPClass();
                    tcpclass.CreateRequester();
                    tcpclass.Connect(ip, val.portImage);
                    tcpclass.SendMessage("Richiesta Foto");

                    string response = tcpclass.ReceiveMessage(2);
                    // foto presente
                    if (response == "ok")
                    {
                        MemoryStream ms = new MemoryStream(tcpclass.ReceiveFile());
                        v.photo = Image.FromStream(ms);
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("SocketException in MapRefresh: " + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception in MapRefresh: " + ex.Message);
                }

                //aggiungo il nuovo utente
                usersMap.Add(ip, v);
                Console.WriteLine("Aggiunto alla mappa l'utente: {0}", v.name);
            }
        }

        public String GetLocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        // riceve i pacchetti di presentazione dal multicast
        public void ReceivePresentations()
        {
            UDPClass udp = new UDPClass();
            udp.MulticastSubscription();
            // aggiorna la mappa con gli utenti online (ignorando l'host locale)
            while (!TerminationHandler.Instance.isTerminationRequired())
            {
                try
                {
                    Value v = udp.ReceiveWrapPacket();
                    if (v.ip.ToString() != GetLocalIP())
                    {
                        mutexMap.WaitOne();
                        MapRefresh(v.ip, v);
                        mutexMap.ReleaseMutex();
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Socket Exception in ReceivePresentations: " + ex.Message);
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
                    try
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
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine("SocketException in ProvidePhoto: " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception in ProvidePhoto: " + ex.Message);
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
            UDPClass udpConnectionsReceiver = (UDPClass) receiver;

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
                            reqName = "un utente privato";

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

                            // path non selezionato
                            if (path == null)
                                continue;
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
                        MessageBox.Show("Ricezione non riuscita: si è verificato un problema di connessione. \n\n" + ex.Message);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        MessageBox.Show("Ricezione non riuscita: non è stato possibile recuperare il nome del file. \n\n" + ex.Message);
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show("Ricezione non riuscita: non è possibile ricevere file con caratteri speciali. \n\n" + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ricezione non riuscita. \n\n" + ex.Message);
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
            const long oneGigabyte = 1073741824;
            UDPClass udpClient = new UDPClass();
            udpClient.Bind();
            foreach (Value user in usersSelected)
            {
                try
                {   // eccezione nel caso in cui il ricevitore non risponda entro il limite di tempo
                    IPEndPoint udpEndPoint = new IPEndPoint(user.ip, user.portRequest);
                    udpClient.SendConnectionRequest(udpEndPoint);
                    string answer = udpClient.ReceivePacket(udpEndPoint);
                    if (answer == "YES")
                    {
                        udpClient.SendPacket(files.Length.ToString(), udpEndPoint); // quanti file sta per inviare

                        for (int j = 0; j < files.Length; j++)
                        {
                            try
                            {
                                filename = files[j].Substring(files[j].LastIndexOf('\\')).Replace("\\", "");
                                attributes = File.GetAttributes(files[j]);
                                dimForCheck = 0;
                                bool isCompressedCopy = false;
                                // controllo dimensione file che l'utente sta per inviare
                                if (attributes.HasFlag(FileAttributes.Directory))
                                {
                                    dimForCheck = Directory.GetFiles(files[j], "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                                }
                                else
                                {
                                    byte[] file = File.ReadAllBytes(files[j]);
                                    dimForCheck = file.LongLength;
                                }

                                // avviso se la dimensione è piu grande di un GB
                                if (dimForCheck > oneGigabyte)
                                {
                                    DialogResult dialogResult = MessageBox.Show("Il file che stai provando ad inviare è molto grande: " +
                                        "potrebbe richiedere alcuni istanti per effettuare la compressione prima dell'invio. Vuoi continuare?",
                                        "Invio File", MessageBoxButtons.YesNo);

                                    if (dialogResult == DialogResult.No)
                                    {
                                        continue;
                                    }
                                }
                                // crea cartella zippata nella cartella di progetto e sostituisce filepath e filename
                                string projectPath = Environment.CurrentDirectory;
                                if (attributes.HasFlag(FileAttributes.Directory))
                                {
                                    String zipFolderPath = projectPath + "\\" + filename + ".zip";
                                    // elimina il file zip se presente (inviato precedentemente)
                                    if (File.Exists(zipFolderPath))
                                        File.Delete(zipFolderPath);
                                    // crea lo zip
                                    ZipFile.CreateFromDirectory(files[j], zipFolderPath);
                                    filename = filename + ".zip";
                                    files[j] = zipFolderPath;
                                    isCompressedCopy = true;
                                }

                                // invio nome e tipo
                                udpClient.SendPacket(filename, udpEndPoint);
                                if (attributes.HasFlag(FileAttributes.Directory))
                                    udpClient.SendPacket("Directory", udpEndPoint);
                                else
                                    udpClient.SendPacket("File", udpEndPoint);

                                // ricezione porta TCP da usare per inviare il file
                                string tcpPort = udpClient.ReceivePacket(udpEndPoint);

                                // invio file
                                TCPClass tcpSender = new TCPClass();
                                tcpSender.CreateRequester();
                                tcpSender.Connect(user.ip, Int32.Parse(tcpPort));

                                // sgancia thread invio tcp
                                string name = files[j];
                                ThreadPool.QueueUserWorkItem(unused => tcpSender.SendFileBuffered(name, isCompressedCopy));
                            }
                            catch (SocketException ex)
                            {
                                MessageBox.Show("Invio del file '" + filename + "' non riuscito: controlla la tua connessione: \n\n" + ex.Message);
                            }
                            catch (ArgumentOutOfRangeException ex)
                            {
                                MessageBox.Show("Invio del file '" + filename + "' non riuscito: non è stato possibile recuperare il nome del file. \n\n" + ex.Message);
                            }
                            catch (ArgumentException ex)
                            {
                                MessageBox.Show("Invio del file '" + filename + "' non riuscito: non è possibile inviare file con caratteri speciali.\nModifica il nome del file e riprova.\n\n" + ex.Message);
                            }
                            catch (IOException ex)
                            {
                                MessageBox.Show("Invio del file '" + filename + "' non riuscito: il file che stai provando a inviare è in uso da un altro processo. \n\n" + ex.Message);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Invio del file '" + filename + "' non riuscito.\n\n" + ex.Message);
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
                    MessageBox.Show("Nessuna risposta ricevuta dall'utente " + user.name + ". Riprova. \n\n" + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Errore durante l'invio dei file all'utente " + user.name + ".\n\n" + ex.Message);
                }
            }
        }

        // Attende sulla pipe e mostra gli utenti online quando si condividono i file
        public void ShowFormSharing()
        {
            int pathLength;
            int numberOfFile = 0;
            String []filesToSend;
            // creo la pipe per ricevere i nomi dei file da inviare
            while (!TerminationHandler.Instance.isTerminationRequired())
            {
                using (NamedPipeClientStream namedPipeClient = new NamedPipeClientStream("pipe-project"))
                {
               
                    try
                    {
                        namedPipeClient.Connect(5000); // timeout di 5 secondi
                        // riceve il numero di file
                        numberOfFile = namedPipeClient.ReadByte();
                        filesToSend = new string[numberOfFile];
                        // riceve un path alla volta e lo inserisce nell'array
                        for (int i = 0; i < numberOfFile; i++)
                        {
                            pathLength = namedPipeClient.ReadByte();
                            byte[] path = new byte[pathLength];
                            // la lettura da pipe e' bloccante, quindi si aspetta che l'altro processo invii i path
                            namedPipeClient.Read(path, 0, pathLength);
                            String filePath = System.Text.Encoding.UTF8.GetString(path);
                            filesToSend[i] = filePath;

                        }

                        FormSharing formSharing = new FormSharing(usersMap, mutexMap);
                        if (formSharing.ShowDialog() == DialogResult.OK)
                        {
                            if (formSharing.getSelectedUsers().Count > 0)
                                ThreadPool.QueueUserWorkItem(unused => this.SendConnection(formSharing.getSelectedUsers(), filesToSend));
                            formSharing.Dispose();
                        }
                    }
                    catch (TimeoutException time)
                    {
                        Console.WriteLine("Pipe timeout. " + time.Message);
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine("FormSHaring chiuso senza utenti selezionati. " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Errore nella selezione dei file da inviare. \n\n " + ex.Message);
                    }

                    namedPipeClient.Close();
                }
            }
            Console.WriteLine("Thread ShowFormSharing terminated");
        }
    }
}
