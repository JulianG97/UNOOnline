using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class OnDataReceivedEventArgs : EventArgs
    {
        public OnDataReceivedEventArgs(byte[] data, NetworkManager networkManager)
        {
            this.Protocol = new Protocol(data);
            this.NetworkManager = networkManager;
        }

        public Protocol Protocol
        {
            get;
            set;
        }

        public NetworkManager NetworkManager
        {
            get;
            set;
        }
    }
}