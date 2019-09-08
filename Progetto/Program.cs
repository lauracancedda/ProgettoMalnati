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
                string argsConcatenated = "";
                foreach (string arg in args)
                {
                    argsConcatenated += arg + " - \n";
                }
                //Per adesso puo inviare un solo file (cioe il primo che si seleziona args[1]
                using (NamedPipeServerStream namedPipeServer = new NamedPipeServerStream("pipe-project"))
                {
                    namedPipeServer.WaitForConnection();
                    string path = args[1];
                    byte[] bytes = Encoding.ASCII.GetBytes(argsConcatenated);
                    // Invio la dimensione del path del file
                    int length = bytes.Length;
                    namedPipeServer.WriteByte(BitConverter.GetBytes(length)[0]);
                    Thread.Sleep(100);
                    // Invio il path del file
                    namedPipeServer.Write(bytes, 0, bytes.Length);
                    Console.WriteLine("Path file inviato");
                    namedPipeServer.Close();
                }
            }

        }

    }
}
