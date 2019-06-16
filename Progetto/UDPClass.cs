using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Progetto
{
    public class UDPClass
    {
        private IPAddress ipMulticast;          //Ip Gruppo Multicast
        private UdpClient client;               //client che si occupa di inviare/ricevere la presentazione al gruppo
        private IPEndPoint remoteEndPoint;      //Ip e porta per inviare la presentazione
        private IPEndPoint anyEndPoint;         //Ip e porta per ricevere la presentazione

        public UDPClass()
        {
            ipMulticast = IPAddress.Parse("224.000.2.0");
            remoteEndPoint = new IPEndPoint(ipMulticast, 1500);
            anyEndPoint = new IPEndPoint(IPAddress.Any, 1500);
        }

        public void MulticastSubscription()
        {
            client = new UdpClient(1500);    // porta del multicast per poter ricevere
            client.JoinMulticastGroup(ipMulticast);
        }

        // Crea un UDPClient associato a una porta libera e ritorna la porta usata
        public int Bind()
        {
            client = new UdpClient(0);
            int portUsed = ((IPEndPoint)client.Client.LocalEndPoint).Port;
            return portUsed;
        }

        // Invia al gruppo multicast la stringa nome_portImage_portRequest
        public void SendPacketMulticast(string s)
        {
            Byte[] data = Encoding.ASCII.GetBytes(s);
            client.Send(data, data.Length, remoteEndPoint);
        }

        // Invia il messaggio al destinatario specificato
        public void SendPacket(string s, IPEndPoint dest)
        {
            Byte[] data = Encoding.ASCII.GetBytes(s);
            client.Send(data, data.Length, dest);
        }

        // Riceve un messaggio dal mittente
        public string ReceivePacket(IPEndPoint remote)
        {
            Byte[] received = client.Receive(ref remote);
            string s = Encoding.ASCII.GetString(received);
            return s;
        }

        // Riceve le informazioni da chiunque sia iscritto al gruppo multicast e le inserisce nella struct valore
        public Value ReceiveWrapPacket()
        {
            Value val;
            Byte[] received = client.Receive(ref anyEndPoint);
            string s = Encoding.ASCII.GetString(received);
            string[] values = s.Split('_');

            val.time = DateTime.Now;
            val.name = values[0];
            val.ip = anyEndPoint.Address;      //non so se sia l'indirizzo del mittente o del multicast
            val.photo = null;
            val.portImage = Int32.Parse(values[1]);
            val.portRequest = Int32.Parse(values[2]);

            Console.WriteLine("Nome: {0}, portImage: {1}, portRequest: {2}", val.name, val.portImage, val.portRequest);

            return val;
        }

        public IPEndPoint ReceiveConnectionRequest()
        {
            //permette di ricevere datagram da ogni sorgente
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            //bloccante finché non arriva un messaggio dall'host remoto
            Byte[] receivedBytes = client.Receive(ref RemoteIpEndPoint);
            string received = Encoding.ASCII.GetString(receivedBytes);

            if (received == "Richiesta Invio")
            {
                return RemoteIpEndPoint;
            }
            else
                return null;
        }

        public void SendConnectionRequest(IPEndPoint dest)
        {
            string s = "Richiesta Invio";
            SendPacket(s, dest);
            return;
        }
    }
}


