using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.IO.Compression;
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
        private static int TIMEOUT_SOCKET = 5000; // set 5 seconds of timeout

        public TCPClass()
        {

        }

        public int CreateListener(IPAddress address, int port)
        {
            int portUsed = port;
            listener = new TcpListener(address, port);
            listener.Server.ReceiveTimeout = TIMEOUT_SOCKET;
            listener.Start();

            if (port == 0)
                portUsed = ((IPEndPoint)listener.LocalEndpoint).Port;

            Console.WriteLine("Porta tcp usata: {0}", portUsed);
            return portUsed;
        }

        public void AcceptConnection()
        {
            // il timeout non funziona con accept
            // quindi controlliamo se ci sono connessioni da accettare
            // se non ci sono controllo se devo uscire
            while(!listener.Pending())
            {
                if (TerminationHandler.Instance.isTerminationRequired())
                    return;
                Thread.Sleep(1000);
            }
            connectedClient = listener.AcceptTcpClient();
            connectedClient.Client.ReceiveTimeout = TIMEOUT_SOCKET;
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
            requester.Client.ReceiveTimeout = TIMEOUT_SOCKET;
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
            FileAttributes attr = File.GetAttributes(filePath);
            string zipFolderPath = "";
            byte[] file = File.ReadAllBytes(filePath);
            byte[] buffer = new byte[BUFFER_SIZE];
            Int64 dim = file.LongLength;
            long left = file.LongLength;
            long offset = 0;
            Thread loadingBarThread = null;
            bool terminateSend = false;

            // lancia la progress bar su un thread separato
            FormStatusFile formStatusFile = new FormStatusFile();
            loadingBarThread = new Thread(() =>
            {
                Application.Run(formStatusFile);
            });
            loadingBarThread.Start();
            //Thread.Sleep(2000);
            try
            {
                //send size
                byte[] dimension = BitConverter.GetBytes(dim);
                if (BitConverter.IsLittleEndian == false)
                    Array.Reverse(dimension);
                stream.Write(dimension, 0, dimension.Length);
                Console.WriteLine("dimensione inviata su {0} byte: {1}", dimension.Length, dim);

                //Delete File Zip created
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    File.Delete(zipFolderPath);

                // Send File
                // iteriamo fintanto che :
                //l'utente non chiede di chiudere l'app
                // l'utente non chiede di terminare dal form di progress bar
                // rimangono byte da inviare
                while (!TerminationHandler.Instance.isTerminationRequired() && !terminateSend && left > 0)
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
                    if (offset > dim)
                        offset = dim;
                    left = left - BUFFER_SIZE;

                    //if (formStatusFile.InvokeRequired)
                    formStatusFile.UpdateProgress(ref terminateSend, offset / 1024, dim / 1024, filePath);
                }

            }
            catch (SocketException ex)
            {
                MessageBox.Show("Errore durante la ricezione del file- controlla la connessione \n\n " + ex.Message);
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show("Path del file/directory non trovato - riprova  \n\n " + ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Path del file non trovato per estrazione in directory - riprova  \n\n " + ex.Message);
            }

            if (loadingBarThread != null)
            {
                if(formStatusFile.InvokeRequired)
                    formStatusFile.BeginInvoke(new Action(() => { formStatusFile.Close(); }));
                loadingBarThread.Join();
            }
                
            return;
        }


        public void ReceiveFileBuffered(object f)
        {
            string[] fileProperties = f as String[];
            string filePath = fileProperties[0];
            string fileType = fileProperties[1];
            string zipName = "";
            byte[] buffer = new byte[BUFFER_SIZE];
            byte[] receivedDim = new byte[8];
            byte[] file;
            long received = 0;
            long nRead;
            Thread loadingBarThread = null;
            bool terminateReceive = false;

            // lancia la progress bar su un thread separato
            FormStatusFile formStatusFile = new FormStatusFile();
            loadingBarThread = new Thread(() =>
            {
                Application.Run(formStatusFile);
            });
            loadingBarThread.Start();
            Thread.Sleep(2000);
            try
            {
                if ((filePath.Substring(filePath.LastIndexOf(".")) == ".zip") && (fileType == "Directory"))
            {
                zipName = filePath.Substring(filePath.LastIndexOf("\\"));
                Directory.CreateDirectory(filePath.Replace(".zip", ""));
                filePath = filePath.Replace(".zip", "") + zipName;
            }
            // inizio un try catch qui perchè qualsiasi cosa accada alla connessione bisogna uscire
           
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
                // Send File
                // iteriamo fintanto che :
                //l'utente non chiede di chiudere l'app
                // l'utente non chiede di terminare dal form di progress bar
                // rimangono byte da ricevere
                while (!TerminationHandler.Instance.isTerminationRequired() && !terminateReceive && received < dim)
                {
                    nRead = stream.Read(buffer, 0, buffer.Length);
                    // se nRead viene restituita sbagliata in caso di ultimo blocco < 1024 provare soluzione commentata
                    /*if (nRead != 1024)
                        nRead = dim - received;*/
                    Array.Copy(buffer, 0, file, received, nRead);
                    received = received + nRead;
                    formStatusFile.UpdateProgress(ref terminateReceive, received / 1024, dim / 1024, filePath);
                }

                if (!terminateReceive && !TerminationHandler.Instance.isTerminationRequired())
                {
                    File.WriteAllBytes(filePath, file);
                    Console.WriteLine("file ricevuto");
                    if ((filePath.Substring(filePath.LastIndexOf(".")) == ".zip") && (fileType == "Directory"))
                    {
                        ZipFile.ExtractToDirectory(filePath, filePath.Replace(zipName, ""));
                        File.Delete(filePath);
                    }
                }

            }
            catch (SocketException ex)
            {
                MessageBox.Show("Errore durante la ricezione del file- controlla la connessione \n\n " + ex.Message);
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show("Path del directory non trovato - riprova  \n\n " + ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Path del file non trovato per estrazione in directory - riprova  \n\n " + ex.Message);
            }

            if (loadingBarThread != null)
            {
                if (formStatusFile.InvokeRequired)
                    formStatusFile.BeginInvoke(new Action(() => { formStatusFile.Close(); }));
                loadingBarThread.Join();
            }

            CloseConnection();
            StopListener();

            return;
        }
    }
}


