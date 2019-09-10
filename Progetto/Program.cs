using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Drawing;
using System.IO;
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
                //Setting Starts
                Application.Run(new FormSettings());
            }
            else
            {
                //FormSharing
                string[] args = Environment.GetCommandLineArgs();
                int numberOfArgs = args.Length;
                byte[] bytes;
                int length;
                //Per adesso puo inviare un solo file (cioe il primo che si seleziona args[1]
                using (NamedPipeServerStream namedPipeServer = new NamedPipeServerStream("pipe-project"))
                {
                    namedPipeServer.WaitForConnection();
                    // invio il numero di file che sto per inviare
                    namedPipeServer.WriteByte(BitConverter.GetBytes(numberOfArgs-1)[0]);
                    // salto il primo argomento tanto è il nome del exec
                    for(int i=1; i < args.Length; i++)
                    {
                        bytes = Encoding.ASCII.GetBytes(args[i]);
                        // Invio la dimensione del path del file
                        length = bytes.Length;
                        namedPipeServer.WriteByte(BitConverter.GetBytes(length)[0]);
                        // perchè questa sleep?
                        Thread.Sleep(100);
                        // Invio il path del file
                        namedPipeServer.Write(bytes, 0, bytes.Length);
                        Console.WriteLine("Path file inviato");
                    }
                    
                    namedPipeServer.Close();
                }
            }

        }

    }
}
