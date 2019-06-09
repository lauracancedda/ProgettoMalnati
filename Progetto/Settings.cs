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
            try
            {
                if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\*\shell\ShareApplication2", "Icon", null) == null)
                {
                    Registry.LocalMachine.OpenSubKey("SOFTWARE", true).OpenSubKey("Classes", true).OpenSubKey("*", true).
                    OpenSubKey("shell", true).CreateSubKey("ShareApplication2").SetValue("Icon", "\"" + pathexecutable + "\"");
                    Registry.LocalMachine.OpenSubKey("SOFTWARE", true).OpenSubKey("Classes", true).OpenSubKey("*", true).
                        OpenSubKey("shell", true).OpenSubKey("ShareApplication2", true).CreateSubKey("command").SetValue("", "\"" + pathexecutable + "\"" + " " + "\"" + "%1" + "\"");

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }

        }

    }
}
