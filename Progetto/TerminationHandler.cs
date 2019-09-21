using System.Threading;

namespace Progetto
{
    class TerminationHandler
    {
        private static TerminationHandler _instance;  // membro privato che rappresenta l'instanza della classe
        private static Mutex mutex = new Mutex();
        private bool termination;

        //costruttore privato non accessibile dall'esterno della classe
        private TerminationHandler()
        {
            termination = false;
        }

        // proprietà che ritorna l'istanza della classe
        public static TerminationHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    mutex.WaitOne();
                    if (_instance == null)
                        _instance = new TerminationHandler();
                    mutex.ReleaseMutex();
                }
                return _instance;
            }
        }

        public bool isTerminationRequired()
        {
            bool terminationRequested;
            mutex.WaitOne();
            terminationRequested = termination;
            mutex.ReleaseMutex();
            return terminationRequested;
        }

        public void setTermination()
        {
            mutex.WaitOne();
            termination = true;
            mutex.ReleaseMutex();
        }

    }
    
}
