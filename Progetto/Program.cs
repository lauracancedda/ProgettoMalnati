using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Drawing;
using System.IO;

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
                System.Environment.SetEnvironmentVariable("envvar", args[1], EnvironmentVariableTarget.User);
                return;
            }


            //Test invio Photo
            //read ip and porta and scriverlo in test ricezione photo
            /*TCPClass tcpImageSender = new TCPClass();
            IPAddress ipinvio = IPAddress.Parse("169.254.42.90");
            //IPAddress ipinvio = IPAddress.Loopback;
            int imagePort = tcpImageSender.CreateListener(ipinvio, 0);

            tcpImageSender.AcceptConnection();
            string messreceive = tcpImageSender.ReceiveMessage(14);
            tcpImageSender.SendMessage("ok");
            MemoryStream ms = new MemoryStream();
            Image image = Image.FromFile("C:\\Users\\lucio\\Documents\\Desktop\\foto2.png");
            //setting.Photo.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            tcpImageSender.SendFile(ms.ToArray());
            bool  g = true;
            //tcpImageSender.SendFileBuffered(ms.ToArray(),ref g);
            tcpImageSender.CloseConnection();
            */


            //Test ricezione photo
            /*IPAddress ip= ipinvio;
            int port = imagePort;
            TCPClass tcpimagereceive = new TCPClass();
            tcpimagereceive.CreateRequester();
            tcpimagereceive.Connect(ip, port);
            tcpimagereceive.SendMessage("Richiesta Foto");
            string response = tcpimagereceive.ReceiveMessage(2);
            if (response == "ok")
            {
                MemoryStream msric = new MemoryStream(tcpimagereceive.ReceiveFile());
                Image ricphoto = Image.FromStream(msric);
                ricphoto.Save("C:\\Users\\Lucio.Ciraci\\Desktop\\ProgettoMalnati5\\Progetto\\bin\\Debug\\immagineRIC.jpg");
            }
            tcpimagereceive.CloseConnection();
            */





            /*MessageBox.Show("HELLO");
            IPAddress ip1 = IPAddress.Parse("192.168.1.35");
            IPAddress ip2 = IPAddress.Parse("10.0.0.2");
            IPAddress ip3 = IPAddress.Parse("10.0.1.1");
            Value v1 = new Value();
            v1.ip = ip1;
            v1.name = "Test";
            v1.photo = Image.FromFile("C:\\Users\\lucio\\Documents\\ProgettoMalnati\\ProgettoMalnati-4f5c72be9b12aacff7e2158fbcf915f2a2443742\\Progetto\\bin\\Debug\\t_logo.png");
            v1.time = DateTime.Now;
            Value v2 = new Value();
            v2.ip = ip2;
            v2.name = "Lucio";
            v2.photo = Image.FromFile("C:\\Users\\lucio\\Documents\\ProgettoMalnati\\ProgettoMalnati-4f5c72be9b12aacff7e2158fbcf915f2a2443742\\Progetto\\bin\\Debug\\immagine.jpg");
            v2.time = DateTime.Now;
            Value v3 = new Value();
            v3.ip = ip3;
            v3.name = "Laura";
            v3.photo = null;
            v3.time = DateTime.Now;
            Dictionary<IPAddress, Value> test = new Dictionary<IPAddress, Value>();
            test.Add(ip1, v1); test.Add(ip2, v2); test.Add(ip3, v3);
            FormSharing share = new FormSharing(test, "TEST", null);
            Application.Run(share);
            */

        }

    }
}
