using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows.Forms;

namespace Progetto
{
    public class TCPClass
    {
        private TcpListener listener;
        private TcpClient requester;
        private Byte[] request;
        private TcpClient connectedClient;
        private NetworkStream stream;
        private const int BUFFER_SIZE = 1024;


        public TCPClass()
        {

        }

        public int CreateListener(IPAddress address, int port)
        {
            int portUsed = port;
            listener = new TcpListener(address, port);
            listener.Start();

            if (port == 0)
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
            byte[] buffer = new byte[BUFFER_SIZE];
            Int64 dim = file.LongLength;
            long left = file.LongLength;
            long offset = 0;

            //send size
            byte[] dimension = BitConverter.GetBytes(dim);
            if (BitConverter.IsLittleEndian == false)
                Array.Reverse(dimension);
            stream.Write(dimension, 0, dimension.Length);
            Console.WriteLine("dimensione inviata su {0} byte: {1}", dimension.Length, dim);

            // Send File
            while (left > 0)
            {
                if (left >= BUFFER_SIZE)
                {
                    Array.Copy(file, offset, buffer, 0, BUFFER_SIZE);
                    stream.Write(buffer, 0, BUFFER_SIZE);
                }
                else
                {
                    Array.Copy(file, offset, buffer, 0, left);
                    stream.Write(buffer, 0, (int) left);
                }
                stream.Flush();
                offset = offset + BUFFER_SIZE;
                left = left - BUFFER_SIZE;
            }

            // Check if the file was sent correctly
            if (offset < dim)
                throw new Exception("Invio interrotto");
            return;
        }

        public byte[] ReceiveFile()
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            byte[] receivedDim = new byte[8];
            byte[] file;
            long received = 0;
            int nRead;

            // ricezione dimensione
            stream.Read(receivedDim, 0, receivedDim.Length);
            if (BitConverter.IsLittleEndian == false)
                Array.Reverse(receivedDim);
            Int64 dim = BitConverter.ToInt64(receivedDim, 0);
            Console.WriteLine("dimensione ricevuta: {0}", dim);

            // ricezione file
            file = new byte[dim];
            stream.ReadTimeout = 10000;
            while (received < dim)
            {
                nRead = stream.Read(buffer, 0, buffer.Length);
                Array.Copy(buffer, 0, file, received, nRead);
                received = received + nRead;
            }

            // controllo se tutto il file è stato inviato
            if (received < dim)
                throw new Exception("Invio interrotto");

            Console.WriteLine("file ricevuto, dimensione {0}", received);
            return file;
        }

        public void SendFileBuffered(object f)
        {
            string filePath = (string) f;
            byte[] file = File.ReadAllBytes(filePath);
            byte[] buffer = new byte[BUFFER_SIZE];
            Int64 dim = file.LongLength;
            long left = file.LongLength;
            long offset = 0;
          
            // a flag to determinate if we need to stop the sending
            bool terminateSend = false;
            // instance of the progress form
            FormStatusFile formStatusFile = new FormStatusFile(ref terminateSend);

            //send size
            byte[] dimension = BitConverter.GetBytes(dim);
            if (BitConverter.IsLittleEndian == false)
                Array.Reverse(dimension);
            stream.Write(dimension, 0, dimension.Length);
            Console.WriteLine("dimensione inviata su {0} byte: {1}", dimension.Length, dim);

            // Send File
            Thread thread = new Thread(() =>
            {
                Application.Run(formStatusFile);
            });
            thread.Start();
           
            while (!terminateSend && left>0)
            {
              
                    if (left >= BUFFER_SIZE)
                    {
                        Array.Copy(file, offset, buffer, 0, BUFFER_SIZE);
                        stream.Write(buffer, 0, BUFFER_SIZE);
                    }
                    else
                    {
                        Array.Copy(file, offset, buffer, 0, left);
                        stream.Write(buffer, 0, (int)left);
                    }
                    stream.Flush();
                    offset = offset + BUFFER_SIZE;
                    left = left - BUFFER_SIZE;
                    formStatusFile.UpdateProgress(dim, offset, filePath);
                }
            

            // Check if the file was sent correctly
            if (offset < dim)
                throw new Exception("Invio interrotto");
            return;
        }


        public void ReceiveFileBuffered(object f)
        {
            string filePath = (string) f;
            byte[] buffer = new byte[BUFFER_SIZE];
            byte[] receivedDim = new byte[8];
            byte[] file;
            long received = 0;
            long nRead;

            // connessione
            AcceptConnection();

            // ricezione dimensione
            stream.Read(receivedDim, 0, receivedDim.Length);
            if (BitConverter.IsLittleEndian == false)
                Array.Reverse(receivedDim);
            Int64 dim = BitConverter.ToInt64(receivedDim, 0);
            file = new byte[dim];
            Console.WriteLine("dimensione ricevuta: {0}", dim);

            // ricezione file
            stream.ReadTimeout = 1000;
            while (received < dim)
            {
                nRead = stream.Read(buffer, 0, buffer.Length);
                // se nRead viene restituita sbagliata in caso di ultimo blocco < 1024 provare soluzione commentata
                /*if (nRead != 1024)
                    nRead = dim - received;*/
                Array.Copy(buffer, 0, file, received, nRead);
                received = received + nRead;
            }

            // controllo se tutto il file è stato inviato
            if (received < dim)
                throw new Exception("Invio interrotto");
            else
            {
                File.WriteAllBytes(filePath, file);
                Console.WriteLine("file ricevuto");
            }

            CloseConnection();
            StopListener();

            return;
        }
    }
}


