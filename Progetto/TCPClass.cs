using System;
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
        private static int TIMEOUT_SOCKET = 10000; // Timeout 10 secondi

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
            // il timeout non funziona con accept: se non ci sono connessioni pendenti non chiama accept
            while (!listener.Pending())
            {
                if (TerminationHandler.Instance.isTerminationRequired())
                    return;
                Thread.Sleep(1000);
            }
            connectedClient = listener.AcceptTcpClient();
            connectedClient.Client.ReceiveTimeout = TIMEOUT_SOCKET;
            stream = connectedClient.GetStream();
        }

        public bool AcceptConnection(int attempts)
        {
            // il timeout non funziona con accept: se non ci sono connessioni pendenti non viene chiamata
            // controlla le connessioni pendenti un numero finito di volte
            while (!listener.Pending() && attempts > 0)
            {
                if (TerminationHandler.Instance.isTerminationRequired())
                    return false;
                Thread.Sleep(1000);
                attempts--;
            }
            if (attempts == 0)
                return false;
            connectedClient = listener.AcceptTcpClient();
            connectedClient.Client.ReceiveTimeout = TIMEOUT_SOCKET;
            stream = connectedClient.GetStream();
            return true;
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

            // invio dimensione
            byte[] dimension = BitConverter.GetBytes(dim);
            if (BitConverter.IsLittleEndian == false)
                Array.Reverse(dimension);
            stream.Write(dimension, 0, dimension.Length);
            Console.WriteLine("dimensione inviata su {0} byte: {1}", dimension.Length, dim);

            // invio file
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

            // controllo se tutto il file è stato inviato
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

            // controllo se tutto il file è stato ricevuto
            if (received < dim)
                throw new Exception("Invio interrotto");

            Console.WriteLine("file ricevuto, dimensione {0}", received);
            return file;
        }

        public void SendFileBuffered(string filePath, bool isCompressedCopy)
        {
            FileAttributes attr = File.GetAttributes(filePath);
            byte[] file = File.ReadAllBytes(filePath);
            byte[] buffer = new byte[BUFFER_SIZE];
            Int64 dim = file.LongLength;
            long left = file.LongLength;
            long offset = 0;
            Thread loadingBarThread = null;
            bool terminateSend = false;
            String formTtitle = "Invio file";
            // lancia la progress bar su un thread separato
            FormStatusFile formStatusFile = new FormStatusFile(formTtitle);
            loadingBarThread = new Thread(() =>
            {
                Application.Run(formStatusFile);
            });
            loadingBarThread.Start();
            try
            {
                // invio dimensione
                byte[] dimension = BitConverter.GetBytes(dim);
                if (BitConverter.IsLittleEndian == false)
                    Array.Reverse(dimension);
                stream.Write(dimension, 0, dimension.Length);
                Console.WriteLine("dimensione inviata su {0} byte: {1}", dimension.Length, dim);

                // Continua l'invio fino a quando:
                // - l'utente non chiude l'app
                // - l'utente non termina l'invio dal FormStatusFile
                // - ci sono ancora byte da inviare
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

                    if (dim < 1024)
                    {
                        formStatusFile.UpdateProgress(ref terminateSend, offset, dim, filePath);
                    }
                    else {
                        formStatusFile.UpdateProgress(ref terminateSend, offset/1024, dim/1024, filePath);
                    }
                }
                // elimina la cartella compressa temporanea
                if (isCompressedCopy)
                    File.Delete(filePath);
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Errore durante la ricezione del file: controlla la tua connessione \n\n " + ex.Message);
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show("Path directory non trovato! Riprova  \n\n " + ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Path del file non trovato per estrazione in directory. Riprova  \n\n " + ex.Message);
            }
            catch (IOException ex)
            {
                MessageBox.Show("Errore durante l'invio del file. Riprova  \n\n " + ex.Message);
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
            string filename = fileProperties[2];
            string zipName = "";
            byte[] buffer = new byte[BUFFER_SIZE];
            byte[] receivedDim = new byte[8];
            byte[] file;
            long received = 0;
            long nRead;
            Thread loadingBarThread = null;
            bool terminateReceive = false;
            bool connectionAccepted = false;
            String formTitle = "Ricezione file";

            // lancia la progress bar su un thread separato
            FormStatusFile formStatusFile = new FormStatusFile(formTitle);
            loadingBarThread = new Thread(() =>
            {
                Application.Run(formStatusFile);
            });

            try
            {
                // connessione 5 tentativi
                connectionAccepted = AcceptConnection(5);
                if (connectionAccepted == true)
                {
                    loadingBarThread.Start();

                    // ricezione dimensione
                    stream.Read(receivedDim, 0, receivedDim.Length);
                    if (BitConverter.IsLittleEndian == false)
                        Array.Reverse(receivedDim);
                    Int64 dim = BitConverter.ToInt64(receivedDim, 0);
                    file = new byte[dim];
                    Console.WriteLine("dimensione ricevuta: {0}", dim);

                    // Continua la ricezione  fino a quando:
                    // - l'utente non chiude l'app
                    // - l'utente non termina la ricezione dal FormStatusFile
                    // - ci sono ancora byte da ricevere
                    while (!TerminationHandler.Instance.isTerminationRequired() && !terminateReceive && received < dim)
                    {
                        nRead = stream.Read(buffer, 0, buffer.Length);
                        Array.Copy(buffer, 0, file, received, nRead);
                        received = received + nRead;
                        if (dim < 1024)
                        {
                            formStatusFile.UpdateProgress(ref terminateReceive, received, dim, filePath + filename);
                        }
                        else
                        {
                            formStatusFile.UpdateProgress(ref terminateReceive, received / 1024, dim / 1024, filePath + filename);
                        }
                    }

                    if (!terminateReceive && !TerminationHandler.Instance.isTerminationRequired())
                    {
                        string format = filename.Substring(filename.LastIndexOf('.'));
                        filename = filename.TrimEnd(format.ToCharArray());
                        string modifiedFilename = filename;
                        //verifica unicità path ed eventualmente lo modifica
                        int count = 1;
                        if (fileType == "File")
                        {
                            while (File.Exists(filePath + modifiedFilename + format) == true)
                            {
                                modifiedFilename = filename + "(" + count + ")";
                                count++;
                            }
                        }
                        else
                        {
                            while (Directory.Exists(filePath + modifiedFilename) == true)
                            {
                                modifiedFilename = filename + "(" + count + ")";
                                count++;
                            }
                        }
                        filePath = filePath + modifiedFilename + format;
                        if ((filePath.Substring(filePath.LastIndexOf(".")) == ".zip") && (fileType == "Directory"))
                        {
                            zipName = filePath.Substring(filePath.LastIndexOf("\\"));
                            Directory.CreateDirectory(filePath.Replace(".zip", ""));
                            filePath = filePath.Replace(".zip", "") + zipName;
                        }

                        File.WriteAllBytes(filePath, file);
                        Console.WriteLine("file ricevuto");
                        if ((filePath.Substring(filePath.LastIndexOf(".")) == ".zip") && (fileType == "Directory"))
                        {
                            ZipFile.ExtractToDirectory(filePath, filePath.Replace(zipName, ""));
                            File.Delete(filePath);
                        }
                    }
                }
                else
                    MessageBox.Show("File non ricevuto! L'utente potrebbe essersi disconnesso");

            }
            catch (SocketException ex)
            {
                MessageBox.Show("Errore durante la ricezione del file: controlla la tua connessione. \n\n " + ex.Message);
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show("Cartella di destinazione non trovata! Se hai selezionato un percorso di ricezione di default assicurati che sia ancora valido.  \n\n " + ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Path del file non trovato per estrazione in directory. Riprova  \n\n " + ex.Message);
            }
            catch (IOException ex)
            {
                MessageBox.Show("Errore durante l'invio del file sulla rete. Riprova  \n\n " + ex.Message);
            }

            if (loadingBarThread != null && connectionAccepted)
            {
                if (formStatusFile.InvokeRequired)
                    formStatusFile.BeginInvoke(new Action(() => { formStatusFile.Close(); }));
                loadingBarThread.Join();
            }

            if(connectionAccepted)
                CloseConnection();
            StopListener();

            return;
        }
    }
}


