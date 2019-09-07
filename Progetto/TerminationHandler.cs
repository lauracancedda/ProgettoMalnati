using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Progetto
{
    class TerminationHandler
    {
        //membro privato che rappresenta l'instanza della classe
    private static TerminationHandler _instance;

        //membro privato per la sincronizzaz dei thread
        private static readonly Object _sync = new Object();

        //costruttore privato non accessibile dall'esterno della classe
        private TerminationHandler() {
            m = new Mutex();
            termination = false;
        }

        //Mutex for accessing termination variable
        private Mutex m;

        // bool to check if termination is requested
        private bool termination;

        public bool isTerminationRequired()
        {
            bool terminationReturn;
            m.WaitOne();
            terminationReturn = termination;
            m.ReleaseMutex();
            return terminationReturn;
        }

        public void setTermination()
        {
            m.WaitOne();
            termination = true;
            m.ReleaseMutex();

        }

        //Entry-Point: proprietà esterna che ritorna l'istanza della classe
        public static TerminationHandler Instance
        {
            get
            {
                //per evitare richieste di lock successive alla prima istanza
                if (_instance == null)
                {
                    lock (_sync) //area critica per la sincronizz dei thread
                    {
                        //vale sempre per la prima istanza
                        if (_instance == null)
                        {
                            _instance = new TerminationHandler();
                        }
                    }
                }
                return _instance;
            }
        }
    }
    
}
