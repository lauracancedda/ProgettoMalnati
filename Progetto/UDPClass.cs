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
        private UdpClient client;               //client che si occupa di inviare/ricevere il nome al gruppo
        private IPEndPoint remoteEndPoint;      //Ip e porta per inviare il nome
        private IPEndPoint anyEndPoint;         //Ip e porta per ricevere il nome


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

        public void Bind()
        {
            client = new UdpClient(0);      // porta casuale
        }

        //Invia al gruppo multicast la stringa nome
        public void SendPacket(string name)
        {
            Byte[] data = Encoding.ASCII.GetBytes(name);
            client.Send(data, data.Length, remoteEndPoint);
        }


        // Riceve le informazioni da chiunque sia iscritto al gruppo multicast e le inserisce nella struct valore
        public value ReceiveWrapPacket()
        {
            Byte[] received;
            value val;
            received = client.Receive(ref anyEndPoint);
            val.name = Encoding.ASCII.GetString(received);
            val.time = DateTime.Now;
            val.ip = anyEndPoint.Address;      //non so se sia l'indirizzo del mittente o del multicast
            val.photo = null;

            return val;
        }

        public void receiveConnectionRequest()
        {

        }
    }
}


