Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new FormSettings());

            // Funzionaaaaa
            //void chekcreg = Registry.LocalMachine.OpenSubKey("SOFTWARE", true).OpenSubKey("Classes", true).OpenSubKey("*", true).OpenSubKey("shell", true).OpenSubKey("Test", true);
            //Farlo in una function e passar il path del eseguibile

            if (Registry.LocalMachine.OpenSubKey("SOFTWARE", true).OpenSubKey("Classes", true).OpenSubKey("*", true).OpenSubKey("shell", true).OpenSubKey("Test", true) == null )
            {
                Registry.LocalMachine.OpenSubKey("SOFTWARE", true).OpenSubKey("Classes", true).OpenSubKey("*", true).
                OpenSubKey("shell", true).CreateSubKey("Prova").SetValue("Icon", "C:\\Users\\lucio\\Documents\\ProgettoMalnati\\progetto\\progetto\\bin\\Debug\\Progetto.exe");
                Registry.LocalMachine.OpenSubKey("SOFTWARE", true).OpenSubKey("Classes", true).OpenSubKey("*", true).
                    OpenSubKey("shell", true).OpenSubKey("Prova", true).CreateSubKey("command").SetValue("dafult", "C:\\Users\\lucio\\Documents\\ProgettoMalnati\\progetto\\progetto\\bin\\Debug\\Progetto.exe%1");

            }