using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;

namespace Progetto
{
    public class Settings
    {

        private string name;
        private Image photo;
        private string defaultPath;
        private bool privateMode;
        private bool automaticReceive;

        // FLAG
        private bool defaultSelected;
        private bool photoSelected;

        // Mutex
        public Mutex mutex_setting;

        public void set_name(string n)
        {
            name = n;
        }

        public string get_name()
        {
            return name; 
        }

        public void set_photo(Image i)
        {
            photo = i;
        }

        public Image get_foto()
        {
            return photo;
        }

        public void set_default_path(string n)
        {
            defaultPath = n;
        }

        public string get_default_path()
        {
            return defaultPath;
        }

        public void set_private_mode(bool p)
        {
            privateMode = p;
        }

        public bool get_private_mode()
        {
            return privateMode;
        }

        public void set_automatic_receive(bool p)
        {
            automaticReceive = p;
        }

        public bool get_automatic_receive()
        {
            return automaticReceive;
        }

        public void set_default_selected(bool p)
        {
            defaultSelected = p;
        }

        public bool get_default_selected()
        {
            return defaultSelected;
        }

        public void set_photo_selected(bool p)
        {
            photoSelected = p;
        }

        public bool get_foto_selected()
        {
            return photoSelected;
        }


        public Settings()
        {
            mutex_setting = new Mutex();
        }

    }
}
