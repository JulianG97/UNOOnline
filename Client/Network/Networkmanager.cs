using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    public class NetworkManager
    {
        private TcpClient client = new TcpClient();
        private NetworkStream stream;
        private IPEndPoint ipEndpoint;
        private Thread readThread;

        public event EventHandler<EventArgs> OnConnectionsLost;
        public event EventHandler<OnDataReceivedEventArgs> OnDataReceived;

        public NetworkManager(IPAddress ip, int port = 3000)
        {
            this.ipEndpoint = new IPEndPoint(ip, port);
        }

        public void Start()
        {
            try
            {
                this.client.Connect(this.ipEndpoint);
            }
            catch
            {
                Console.WriteLine("The server is unreachable! Please try again later...");
            }

            try
            {
                this.stream = client.GetStream();
                this.readThread = new Thread(this.Read);
                readThread.Start();
            }
            catch
            {
                this.FireOnConnectionLost();
            }
        }

        public void Send(Protocol protocol)
        {
            try
            {
                byte[] sendBytes = protocol.Create();

                this.stream.Write(sendBytes, 0, sendBytes.Length);
            }
            catch
            {
                this.FireOnConnectionLost();
            }
        }

        // Reads each byte from the stream until the stream is empty
        public void Read()
        {
            while (true)
            {
                try
                {
                    List<byte> receivedBytes = new List<byte>();
                    byte[] buffer = new byte[1];
                    int currentByte = 0;

                    while (Encoding.ASCII.GetString(buffer) != "\n")
                    {
                        currentByte = this.stream.Read(buffer, 0, 1);
                        receivedBytes.Add(buffer[0]);
                    }

                    this.OnDataReceived(this, new OnDataReceivedEventArgs(receivedBytes.ToArray()));
                }
                catch
                {
                    this.FireOnConnectionLost();
                }
            }
        }

        protected virtual void FireOnConnectionLost()
        {
            if (this.OnConnectionsLost != null)
            {
                this.OnConnectionsLost(this, new EventArgs());
            }
        }

        protected virtual void FireOnDataReceived(byte[] data)
        {
            if (this.OnDataReceived != null)
            {
                this.OnDataReceived(this, new OnDataReceivedEventArgs(data));
            }
        }
    }
}
