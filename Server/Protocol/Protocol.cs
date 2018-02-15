using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Protocol
    {
        private byte[] header = Encoding.ASCII.GetBytes("UNO");

        public Protocol(byte[] type, byte[] content)
        {
            this.Type = type;
            this.Content = content;
        }

        public Protocol(byte[] bytes)
        {
            if (bytes[0] == header[0] && bytes[1] == header[1] && bytes[2] == header[2])
            {
                this.Type = new byte[] { bytes[3], bytes[4] };

                List<byte> contentBytes = new List<byte>();

                for (int i = 5; i < bytes.Length; i++)
                {
                    contentBytes.Add(bytes[i]);
                }

                this.Content = contentBytes.ToArray();
            }
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

        public byte[] ToByteArray()
        {
            List<byte> protocol = new List<byte>();

            protocol.AddRange(this.header);
            protocol.AddRange(this.Type);
            protocol.AddRange(this.Content);

            return protocol.ToArray();
        }
    }
}
