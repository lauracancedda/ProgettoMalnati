using System;
using System.Windows.Forms;
using System.Threading;
using System.IO.Pipes;
using System.Text;

namespace Progetto
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Environment.GetCommandLineArgs().Length == 1)
            {
                Application.Run(new FormSettings());
            }
            else
            {
                // FormSharing
                string[] args = Environment.GetCommandLineArgs();
                int numberOfArgs = args.Length;
                byte[] bytes;
                int length;
                using (NamedPipeServerStream namedPipeServer = new NamedPipeServerStream("pipe-project"))
                {
                    namedPipeServer.WaitForConnection();
                    // invia il numero di file selezionati
                    namedPipeServer.WriteByte(BitConverter.GetBytes(numberOfArgs-1)[0]);
                    // invia i nomi dei file
                    for(int i=1; i < args.Length; i++)
                    {
                        bytes = Encoding.ASCII.GetBytes(args[i]);
                        // invia la dimensione del path del file
                        length = bytes.Length;
                        namedPipeServer.WriteByte(BitConverter.GetBytes(length)[0]);
                        // invia il path del file
                        namedPipeServer.Write(bytes, 0, bytes.Length);
                        Console.WriteLine("Path file inviato");
                    }
                    namedPipeServer.Close();
                }
            }
        }
    }
}
