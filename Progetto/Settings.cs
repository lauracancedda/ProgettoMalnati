using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using Microsoft.Win32;

namespace Progetto
{
    public class Settings
    {

        public string Name
        {
            get; set;
        }
        public Image Photo
        {
            get; set;
        }
        public string DefaultPath
        {
            get; set;
        }

        private bool _privateMode;
        public bool PrivateMode
        {
            get { return _privateMode; }
            set
            {
                _privateMode = value;
                if (value == true)
                    publicMode.Reset();
                else
                    publicMode.Set();
            }
        }
        public bool AutomaticReceive
        {
            get; set;
        }

        // FLAG
        public bool DefaultSelected
        {
            get; set;
        }
        public bool PhotoSelected
        {
            get; set;
        }

        // Mutex
        public Mutex mutex_setting;
        public ManualResetEvent publicMode;


        public Settings()
        {
            mutex_setting = new Mutex();
            publicMode = new ManualResetEvent(false);
        }

        public void SetKeyRegedit(string pathexecutable)
        {
            //Set the Path-process to the Registry. To be able to open a file or a directory
            try
            {
                //File
                if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\*\shell\Progetto", "Icon", null) == null)
                {
                    Registry.LocalMachine.OpenSubKey("SOFTWARE", true).OpenSubKey("Classes", true).OpenSubKey("*", true).
                    OpenSubKey("shell", true).CreateSubKey("Condividi con gli utenti online").SetValue("Icon", "\"" + pathexecutable + "\"");
                    Registry.LocalMachine.OpenSubKey("SOFTWARE", true).OpenSubKey("Classes", true).OpenSubKey("*", true).
                        OpenSubKey("shell", true).OpenSubKey("Condividi con gli utenti online", true).CreateSubKey("command").SetValue("", "\"" + pathexecutable + "\"" + " " + "\"" + "%1" + "\"");

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }
            //Directory
            try
            {
                if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Directory\shell\Progetto", "Icon", null) == null)
                {
                    Registry.LocalMachine.OpenSubKey("SOFTWARE", true).OpenSubKey("Classes", true).OpenSubKey("Directory", true).
                    OpenSubKey("shell", true).CreateSubKey("Condividi con gli utenti online").SetValue("Icon", "\"" + pathexecutable + "\"");
                    Registry.LocalMachine.OpenSubKey("SOFTWARE", true).OpenSubKey("Classes", true).OpenSubKey("Directory", true).
                        OpenSubKey("shell", true).OpenSubKey("Condividi con gli utenti online", true).CreateSubKey("command").SetValue("", "\"" + pathexecutable + "\"" + " " + "\"" + "%1" + "\"");

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }

        }

    }
}
