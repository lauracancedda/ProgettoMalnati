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
            sendMulticast = new Thread(SendPresentation);
            receiveMulticast = new Thread(ReceivePresentations);
            manageMap = new Thread(CheckMap);
            sendImageUnicast = new Thread(ProvidePhoto);
            // aggiungere ricezione

            sendMulticast.Start();
            receiveMulticast.Start();
            manageMap.Start();
            sendImageUnicast.Start();
            //ricezione.Start();
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
                //utente presente, quindi aggiorno il suo timestamp nella mappa
                usersMap.TryGetValue(ip, out v);
                v.time = DateTime.Now;
            }
            else
            {
                //Utente non presente       // val è un riferimento???
                v.name = val.name;
                v.time = val.time;
                v.photo = null;
                v.ip = ip;

                //richiedo la foto
                TCPClass tcpclass = new TCPClass();
                tcpclass.CreateRequester();
                tcpclass.Connect(ip, 1600);

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

        // invia in multicast il proprio nome ogni 5 secondi
        public void SendPresentation()
        {
            UDPClass udp = new UDPClass();
            udp.Bind();
            while (true)
            {
                if (setting.get_private_mode() == false)
                {
                    udp.SendPacket(setting.get_name());
                }
                Thread.Sleep(5000);
            }
        }


        // invia l'immagine a chi la richiede
        void ProvidePhoto()
        {
            TCPClass tcp = new TCPClass();
            tcp.CreateListener(IPAddress.Any, 1600);
            while (true)
            {
                tcp.AcceptConnection();
                tcp.ReceiveMessage(14);
                if (setting.get_foto_selected() == true)
                {
                    tcp.SendMessage("ok");
                    MemoryStream ms = new MemoryStream();
                    setting.get_foto().Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    tcp.SendFile(ms.ToArray());
                }
                else
                {
                    tcp.SendMessage("no");
                }
                tcp.CloseConnection();
            }
        }
    }
}
