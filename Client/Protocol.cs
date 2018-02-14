using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Protocol
    {
        private byte[] header = Encoding.ASCII.GetBytes("UNO");

        public Protocol(byte[] type, byte[] content)
        {
            this.Type = type;
            this.Content = content;
        }

        public byte[] Type
        {
            get;
            set;
        }

        public byte[] Content
        {
            get;
            set;
        }

        public byte[] Create()
        {
            List<byte> protocol = new List<byte>();

            protocol.AddRange(this.header);
            protocol.AddRange(this.Type);
            protocol.AddRange(this.Content);

            return protocol.ToArray();
        }
    }
}
