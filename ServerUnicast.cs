using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;


namespace Progetto
{
    class ServerUnicast
    {
        private Thread thread;
        Impostazioni impostazioni;
        ConfermaRicezione c;

        // costruttore
        public ServerUnicast(Impostazioni impostazioni)
        {
            this.impostazioni = impostazioni;
            // crea e lancia il thread
            thread = new Thread(start);
            thread.Start();
        }

        // funzione d'avvio
        private void start()
        {
            //Crea un UdpClient per leggere le richieste in arrivo
            UdpClient receivingUdpClient = new UdpClient(1500);

            //receivingUdpClient.JoinMulticastGroup(IPAddress.Parse("224.000.2.0")); sbagliato, lui ascolto l'unicast
            // Riceve richieste anicast da chiunque sulla porta 1500
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 1500);


            // ciclo infinito di lettura pacchetti udp
            while (true)
            {
                try
                {
                    // Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

                    string returnData = Encoding.ASCII.GetString(receiveBytes);

                    Console.WriteLine("This is the message you received " +
                                                 returnData.ToString());
                    Console.WriteLine("This message was sent from " +
                                                RemoteIpEndPoint.Address.ToString() +
                                                " on their port number " +
                                                RemoteIpEndPoint.Port.ToString());

                    // risposta di conferma
                    if(impostazioni.ricezione_automatica != true)
                    {   
                        // lancia il form che chiede conferma all'utente
                        c = new ConfermaRicezione(returnData.ToString());
                        Application.Run(c);
                    }

                    if (impostazioni.ricezione_automatica == true || c.getRisposta() == true)
                    {
                        // invio messaggio ok
                    }
                    else
                    {
                        // invio messaggio rifiuto
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
