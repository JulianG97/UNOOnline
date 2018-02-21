using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class NetworkManager
    {
        private TcpClient playerClient;
        private NetworkStream playerStream;
        private Thread readThread;
        private bool isReading;

        public event EventHandler<EventArgs> OnConnectionsLost;
        public event EventHandler<OnDataReceivedEventArgs> OnDataReceived;

        public NetworkManager(TcpClient playerClient)
        {
            this.playerClient = playerClient;
        }

        public void Start()
        {
            try
            {
                this.playerStream = this.playerClient.GetStream();
                this.readThread = new Thread(this.Read);
                this.isReading = true;
                readThread.Start();
            }
            catch
            {
                this.FireOnConnectionLost();
            }
        }

        public void Stop()
        {
            try
            {
                this.isReading = false;
                this.playerStream.Close();
                this.playerClient.Close();
            }
            catch
            {

            }
        }

        public void Send(Protocol protocol)
        {
            try
            {
                byte[] sendBytes = protocol.ToByteArray();

                this.playerStream.Write(sendBytes, 0, sendBytes.Length);
            }
            catch
            {
                this.FireOnConnectionLost();
            }
        }

        // Reads each byte from the stream until the stream is empty
        public void Read()
        {
            while (this.isReading == true)
            {
                try
                {
                    if (!this.playerStream.DataAvailable)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    List<byte> receivedBytes = new List<byte>();
                    byte[] buffer = new byte[1];

                    while (this.playerStream.DataAvailable)
                    {
                        this.playerStream.Read(buffer, 0, 1);

                        // Checks if the current byte is the last byte of a protocol
                        if (buffer[0] == 33)
                        {
                            break;
                        }

                        receivedBytes.Add(buffer[0]);
                    }

                    if (receivedBytes != null)
                    {
                        this.FireOnDataReceived(receivedBytes.ToArray());
                    }
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
                this.OnDataReceived(this, new OnDataReceivedEventArgs(data, this));
            }
        }
    }
}
