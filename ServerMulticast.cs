using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace Progetto
{
    class ServerMulticast
    {
        private Thread thread;
        Impostazioni impostazioni;

        // costruttore
        public ServerMulticast(Impostazioni impostazioni)
        {
            this.impostazioni = impostazioni;
            // crea e lancia il thread
            thread = new Thread(start);
            thread.Start();
        }

        // funzione d'avvio
        public static void start()
        {
            //Crea un UdpClient per leggere le richieste in arrivo
            UdpClient listener = new UdpClient(1500);
            listener.JoinMulticastGroup(IPAddress.Parse("224.000.2.0"));
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse("224.000.2.0"), 1500);
           // IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 1500);
            Console.WriteLine("thread partito");

            // ciclo infinito di lettura pacchetti udp
            while (true)
            {
                try
                {
                    // Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = listener.Receive(ref RemoteIpEndPoint);

                    string returnData = Encoding.ASCII.GetString(receiveBytes);

                    Console.WriteLine("This is the message you received " +
                                                 returnData.ToString());
                    Console.WriteLine("This message was sent from " +
                                                RemoteIpEndPoint.Address.ToString() +
                                                " on their port number " +
                                                RemoteIpEndPoint.Port.ToString());

                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
