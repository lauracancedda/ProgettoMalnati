using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;


namespace Progetto
{
    public class TCPClass
    {
        private TcpListener listener;
        private TcpClient requester;
        private Byte[] request;
        private TcpClient connectedClient;
        private NetworkStream stream;

        public TCPClass()
        {

        }

        public int CreateListener(IPAddress address, int port)
        {
            int portUsed = port;
            listener = new TcpListener(address, port);
            listener.Start();

            if(port == 0)
                portUsed = ((IPEndPoint)listener.LocalEndpoint).Port;

            Console.WriteLine("Porta tcp usata: {0}", portUsed);
            return portUsed;
        }

        public void AcceptConnection()
        {
            connectedClient = listener.AcceptTcpClient();
            stream = connectedClient.GetStream();
        }

        public void CloseConnection()
        {
            stream.Close();
            connectedClient.Close();
        }

        public void StopListener()
        {
            listener.Stop();
        }

        public void CreateRequester()
        {
            requester = new TcpClient();
        }

        public void Connect(IPAddress address, int port)
        {
            requester.Connect(address, port);
            stream = requester.GetStream();
        }

        public void SendMessage(string message)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Console.WriteLine("Messaggio inviato: {0}", message);
        }

        public string ReceiveMessage(int dimension)
        {
            request = new Byte[dimension];
            stream.Read(request, 0, request.Length);
            string data = System.Text.Encoding.ASCII.GetString(request, 0, request.Length);
            Console.WriteLine("Messaggio ricevuto: {0}", data);
            return data;
        }

        public void SendFile(byte[] file)
        {
            //Understand why the tcp connection fail 
            Int64 dim = file.Length;
            byte[] dimension = BitConverter.GetBytes(dim);
            if (BitConverter.IsLittleEndian == false)
                Array.Reverse(dimension);
            stream.Write(dimension, 0, dimension.Length);
            stream.Write(file, 0, file.Length);
            stream.Flush();
            return;
        }

        public byte[] ReceiveFile()
        {
            //Understand why the tcp connection fail 
            byte[] receivedDim = new byte[8];
            stream.Read(receivedDim, 0, receivedDim.Length);
            if (BitConverter.IsLittleEndian == false)
                Array.Reverse(receivedDim);
            Console.WriteLine("dimensione ricevuta: {0}", BitConverter.ToInt64(receivedDim, 0));

            byte[] file = new byte[BitConverter.ToInt64(receivedDim, 0)];
            stream.Read(file, 0, file.Length);
            //File.WriteAllBytes("newFile.jpg", file);
            Console.WriteLine("file ricevuto");
   
            return file;
        }

        public void SendFileBuffered(byte[] file, ref bool flag)
        {
            byte[] buffer = new byte[1024];
            int dim = file.Length;
            int left = file.Length;
            int offset = 0;

            //send size
            byte[] dimension = BitConverter.GetBytes(dim);
            if (BitConverter.IsLittleEndian == false)
                Array.Reverse(dimension);
            stream.Write(dimension, 0, dimension.Length);

            // Send File - Manage send view
            FormStatusFile formstatusfile = new FormStatusFile(0, dim);
            while (flag == true && left > 0)
            {
                Array.ConstrainedCopy(file, offset, buffer, 0, 1024);
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
                offset = offset + 1024;
                left = left - 1024;
                if (formstatusfile.ChangeStatus(offset) == -1)
                {
                    //Cancel Send File
                    flag = false;
                }
            }

            // Check if the file is upload
            if (offset < dim)
                throw new Exception("Invio interrotto");
            return;/**/
        }


        public void ReceiveFileBuffered(object f)
        {
            string filename = (string)f;
            byte[] buffer = new byte[1024];
            byte[] receivedDim = new byte[8];
            byte[] file;
            Int64 dim;
            int received = 0;
            int nRead;

            // connessione
            AcceptConnection();

            // ricezione dimensione
            stream.Read(receivedDim, 0, receivedDim.Length);
            if (BitConverter.IsLittleEndian == false)
                Array.Reverse(receivedDim);
            dim = BitConverter.ToInt64(receivedDim, 0);
            file = new byte[dim];
            Console.WriteLine("dimensione ricevuta: {0}", dim);
            
            // ricezione file
            stream.ReadTimeout = 1000;
            while (received < dim && stream.DataAvailable)
            {
                nRead = stream.Read(buffer, 0, buffer.Length);
                Array.ConstrainedCopy(buffer, 0, file, received, nRead);
                received = received + nRead;
            }

            // controllo se tutto il file è stato inviato
            if (received < dim)
                throw new Exception("Invio interrotto");
            else
            {
                File.WriteAllBytes(filename, file);
                Console.WriteLine("file ricevuto");
            }

            CloseConnection();
            StopListener();

            return;
        }

    }
}


