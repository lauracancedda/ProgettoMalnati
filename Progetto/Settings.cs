using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.IO;

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

        public void SetKeyRegedit(string pathExecutable)
        {
            string nameExecutable = Regex.Replace(pathExecutable.Substring(pathExecutable.LastIndexOf('\\')), @"\\", "");
            File.Copy(pathExecutable, Environment.GetFolderPath(Environment.SpecialFolder.SendTo) + "\\" + nameExecutable, true);
        }

        public void UnSetKeyRegedit(string pathExecutable)
        {
            string nameExecutable = Regex.Replace(pathExecutable.Substring(pathExecutable.LastIndexOf('\\')), @"\\", "");
            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.SendTo) + "\\" + nameExecutable);
        }
    }
}
