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

namespace Progetto
{
    public struct value
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
        private Dictionary<IPAddress, value> usersMap;
        private Mutex mutex_map;
        private Thread sendMulticast;
        private Thread receiveMulticast;
        private Thread manageMap;
        private Thread sendImageUnicast;
        private Thread receiveUnicast;

        public MainClass(Settings s)
        {
            setting = s;
            mutex_map = new Mutex();
            usersMap = new Dictionary<IPAddress, value>();
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

            sendImageUnicast.Start(imageSender);
            receiveUnicast.Start(connectionReceiver);
            sendMulticast.Start(ports);
            receiveMulticast.Start();
            manageMap.Start();
        }


        //  controllo degli utenti ancora connessi
        public void CheckMap()
        {
            while (true)
            {
                mutex_map.WaitOne();
                foreach (KeyValuePair<IPAddress, value> entry in usersMap)
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
        public void MapRefresh(IPAddress ip, value val)
        {
            value v;
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
                value v=udp.ReceiveWrapPacket();
                mutex_map.WaitOne();
                MapRefresh(v.ip, v);
                mutex_map.ReleaseMutex();
            }
        }

        // invia in multicast la presentazione name_portImage_portRequest ogni 5 secondi
        public void SendPresentation(object ports)
        {
            UDPClass udp = new UDPClass();
            udp.Bind();
            while (true)
            {
                if (setting.PrivateMode == false)
                {
                    udp.SendPacketMulticast(setting.Name.Trim('_') + "_" + (string)ports);
                }
                Thread.Sleep(5000);
            }
        }


        // invia l'immagine a chi la richiede
        void ProvidePhoto(object imageSender)
        {
            // questo thread dovrebbe lavorare solo in modalità pubblica, come sendPresentation
            TCPClass tcpImageSender = (TCPClass)imageSender;
            while (true)
            {
                tcpImageSender.AcceptConnection();
                tcpImageSender.ReceiveMessage(14);
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
        void ReceiveConnections(object receiver)
        {
            // questo thread dovrebbe lavorare solo in modalità pubblica, come sendPresentation
            string path;
            UDPClass udpConnectionsReceiver = (UDPClass)receiver;

            while (true)
            {
                udpConnectionsReceiver.ReceiveConnectionRequest();
                if (setting.AutomaticReceive == false)
                {
                    //mostrare form confirmreceive
                    // in caso negativo inviare NO e poi continue
                }
                //inviare OK

                if (setting.DefaultSelected == false)
                {
                    //mostrare form selezionaPath
                }
                else
                {
                    path = setting.DefaultPath;
                }

                //verifica unicità path

                //sgancia thread ricezione tcp

            }
        }
    }
}
