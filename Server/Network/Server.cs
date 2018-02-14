using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Server
    {
        public bool isRunning
        {
            get;
            private set;
        }

        public void Start()
        {
            this.isRunning = true;
            Console.WriteLine("The server has been successfully started!");
        }

        public void Stop()
        {
            this.isRunning = false;
            Console.WriteLine("The server has been successfully stopped!");
        }
    }
}