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
        private NetworkStream playerStream;
        private Thread readThread;
        private bool isReading;
        private Thread isAliveThread;
        private bool isAlive;

        public event EventHandler<OnConnectionLostEventArgs> OnConnectionsLost;
        public event EventHandler<OnDataReceivedEventArgs> OnDataReceived;

        public NetworkManager(TcpClient playerClient)
        {
            this.PlayerClient = playerClient;
        }

        public TcpClient PlayerClient
        {
            get;
            private set;
        }

        public void Start()
        {
            try
            {
                this.playerStream = this.PlayerClient.GetStream();

                this.readThread = new Thread(this.Read);
                this.isReading = true;
                this.readThread.Start();

                this.isAliveThread = new Thread(this.IsAlive);
                this.isAliveThread.Start();
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
                this.PlayerClient.Close();
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

                    if (receivedBytes.Count == 5)
                    {
                        if (receivedBytes[0] == 85 && receivedBytes[1] == 78 && receivedBytes[2] == 79 && receivedBytes[3] == 73 && receivedBytes[4] == 65)
                        {
                            this.isAlive = true;
                        }
                    }

                    this.FireOnDataReceived(receivedBytes.ToArray());
                }
                catch
                {
                    this.FireOnConnectionLost();
                }
            }
        }

        private void IsAlive()
        {
            /*while (this.isReading == true)
            {
                this.Send(ProtocolManager.IsAlive());

                Thread.Sleep(5000);

                if (this.isAlive == false)
                {
                    this.Stop();
                    this.FireOnConnectionLost();
                }
                else if (this.isAlive == true)
                {
                    this.isAlive = false;
                }
            }*/

            Thread sendIsAliveThread = new Thread(this.SendIsAlive);
            Thread checkIfIsAliveThread = new Thread(this.CheckIfIsAlive);

            sendIsAliveThread.Start();
            checkIfIsAliveThread.Start();
        }

        private void SendIsAlive()
        {
            while (this.isReading == true)
            {
                this.Send(ProtocolManager.IsAlive());

                Thread.Sleep(100);
            }
        }

        private void CheckIfIsAlive()
        {
            while (this.isReading == true)
            {
                Thread.Sleep(1000);

                if (this.isAlive == false)
                {
                    this.Stop();
                    this.FireOnConnectionLost();
                }
                else if (this.isAlive == true)
                {
                    this.isAlive = false;
                }
            }
        }

        protected virtual void FireOnConnectionLost()
        {
            if (this.OnConnectionsLost != null)
            {
                this.OnConnectionsLost(this, new OnConnectionLostEventArgs(this.PlayerClient));
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
