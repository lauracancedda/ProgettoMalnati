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
                    stream.Write(buffer, 0, (int)left);
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

            Console.WriteLine("file ricevuto, dimensione {0}", received);
            return file;
        }

        public void SendFileBuffered(object f)
        {
            string filePath = (string)f;

            //NEW
            FileAttributes attr = File.GetAttributes(filePath);
            string pathfile = "";
            //detect whether its a directory or file

            byte[] file = File.ReadAllBytes(filePath);
            byte[] buffer = new byte[BUFFER_SIZE];
            Int64 dim = file.LongLength;
            long left = file.LongLength;
            long offset = 0;
            Thread loadingBarThread = null;

            // a flag to determinate if we need to stop the sending
            bool terminateSend = false;
            // instance of the progress form
            FormStatusFile formStatusFile = new FormStatusFile();
            try
            {
                // allocate a separate thread to run the loading bar
                loadingBarThread = new Thread(() =>
                {
                    Application.Run(formStatusFile);
                });
                loadingBarThread.Start();

                //send size of the file
                byte[] dimension = BitConverter.GetBytes(dim);
                if (BitConverter.IsLittleEndian == false)
                    Array.Reverse(dimension);

                stream.Write(dimension, 0, dimension.Length);
                Console.WriteLine("dimensione inviata su {0} byte: {1}", dimension.Length, dim);

                //Delete File Zip created
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    File.Delete(pathfile);

                while (!terminateSend && left > 0)
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
                    if (formStatusFile.InvokeRequired)
                        formStatusFile.UpdateProgress(ref terminateSend, offset / 1024, dim / 1024, filePath);
                }


                // Check if the file was sent correctly
                if (offset < dim)
                    throw new Exception("Invio interrotto");
                if (!terminateSend && formStatusFile.InvokeRequired)
                    formStatusFile.BeginInvoke(new Action(() => { formStatusFile.showdialogset(true); formStatusFile.Close(); formStatusFile.showdialogset(false); }));
            }
            catch (Exception ex)
            {
                if (formStatusFile.InvokeRequired)
                    formStatusFile.BeginInvoke(new Action(() => { formStatusFile.showdialogset(true); formStatusFile.Close(); formStatusFile.showdialogset(false); }));

                MessageBox.Show("FIle Non inviato : \n " + ex.Message);
            }
            if (loadingBarThread != null)
                loadingBarThread.Join();
            return;
        }


        public void ReceiveFileBuffered(object stateFile)
        {
            String[] filePropArray = stateFile as String[];

            string filePath = filePropArray[0];
            string fileType = filePropArray[1];
            String zipName = "";
            byte[] buffer = new byte[BUFFER_SIZE];
            byte[] receivedDim = new byte[8];
            byte[] file;
            // set timeout of 5 seconds for readig stream
            stream.ReadTimeout = 5000;
            long received = 0;
            long nRead;
            // a flag to determinate if we need to stop the sending
            bool terminateSend = false;
            // instance of the progress form
            FormStatusFile formStatusFile = new FormStatusFile();
            // declare the thread to load the for for progress bar
            Thread progressBarThread = null;
            try
            {
                if ((filePath.Substring(filePath.LastIndexOf(".")) == ".zip") && (fileType == "Directory"))
                {
                    zipName = filePath.Substring(filePath.LastIndexOf("\\"));
                    Directory.CreateDirectory(filePath.Replace(".zip", ""));
                    filePath = filePath.Replace(".zip", "") + zipName;
                }
                // accept connection and wait for file dimension
                AcceptConnection();
                stream.Read(receivedDim, 0, receivedDim.Length);
                if (BitConverter.IsLittleEndian == false)
                    Array.Reverse(receivedDim);
                Int64 dim = BitConverter.ToInt64(receivedDim, 0);
                file = new byte[dim];
                Console.WriteLine("dimensione ricevuta: {0}", dim);

                // if we don't use a form the form will get stuck
                progressBarThread = new Thread(() =>
                {
                    Application.Run(formStatusFile);
                });
                progressBarThread.Start();

                while (!terminateSend && received < dim)
                {
                    nRead = stream.Read(buffer, 0, buffer.Length);
                    // se nRead viene restituita sbagliata in caso di ultimo blocco < 1024 provare soluzione commentata
                    /*if (nRead != 1024)
                        nRead = dim - received;*/
                    Array.Copy(buffer, 0, file, received, nRead);
                    received = received + nRead;
                    if (formStatusFile.InvokeRequired)
                        formStatusFile.UpdateProgress(ref terminateSend, received / 1024, dim / 1024, filePath);
                }

                if (!terminateSend)
                {
                    File.WriteAllBytes(filePath, file);
                    Console.WriteLine("file ricevuto");

                    if ((filePath.Substring(filePath.LastIndexOf(".")) == ".zip") && (fileType == "Directory"))
                    {
                        ZipFile.ExtractToDirectory(filePath, filePath.Replace(zipName, ""));
                        File.Delete(filePath);
                    }

                    if (formStatusFile.InvokeRequired)
                        formStatusFile.BeginInvoke(new Action(() => { formStatusFile.showdialogset(true); formStatusFile.Close(); formStatusFile.showdialogset(false); }));
                }

            }
            catch (Exception ex)
            {
                if (formStatusFile.InvokeRequired)
                    formStatusFile.BeginInvoke(new Action(() => { formStatusFile.showdialogset(true); formStatusFile.Close(); formStatusFile.showdialogset(false); }));
                MessageBox.Show("FIle Non Ricevuto : \n " + ex.Message);
                // TODO: check if we need to make some other stuff in case of exception
                // TODO: better to catch the specific exception for socket closed ? 
            }
            if (progressBarThread != null)
                progressBarThread.Join();

            CloseConnection();
            StopListener();
            return;
        }
    }
}


