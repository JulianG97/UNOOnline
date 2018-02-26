using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    public class OnConnectionLostEventArgs : EventArgs
    {
        public OnConnectionLostEventArgs(TcpClient client)
        {
            this.Client = client;
        }

        public TcpClient Client
        {
            get;
            private set;
        }
    }
}
