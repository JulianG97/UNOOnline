using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class OnDataReceivedEventArgs : EventArgs
    {
        public OnDataReceivedEventArgs(byte[] data)
        {
            this.Protocol = new Protocol(data);
        }

        public Protocol Protocol
        {
            get;
            set;
        }
    }
}