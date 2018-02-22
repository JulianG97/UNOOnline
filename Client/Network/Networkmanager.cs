using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Client
{
    public class NetworkManager
    {
        private TcpClient client;
        private NetworkStream stream;
        private IPEndPoint ipEndpoint;
        private Thread readThread;
        private bool isReading;

        public event EventHandler<EventArgs> OnConnectionsLost;
        public event EventHandler<OnDataReceivedEventArgs> OnDataReceived;

        public NetworkManager(IPAddress ip)
        {
            this.ipEndpoint = new IPEndPoint(ip, 3000);
            this.client = new TcpClient();
            this.Connected = false;
        }

        public bool Connected
        {
            get;
            private set;
        }

        public void Start()
        {
            try
            {
                if (!this.client.ConnectAsync(this.ipEndpoint.Address, this.ipEndpoint.Port).Wait(1000))
                {
                    throw new TimeoutException();
                }
                else
                {
                    this.Connected = true;
                }
            }
            catch (Exception)
            {
                Console.Clear();

                Menu.DisplayGameHeader();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("The server is unreachable! Please try again later.");
                Console.WriteLine("Press any key to return to the main menu...");
                Console.ResetColor();
                Console.ReadKey(true);

                Console.Clear();


                Menu.DisplayMainMenu();
            }

            if (this.Connected == true)
            {
                try
                {
                    this.stream = client.GetStream();
                    this.readThread = new Thread(this.Read);
                    this.isReading = true;
                    readThread.Start();
                }
                catch
                {
                    this.FireOnConnectionLost();
                }
            }

            /*try
            {
                this.client.Connect(this.ipEndpoint);
            }
            catch
            {
                Console.Clear();

                Menu.DisplayGameHeader();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("The server is unreachable! Please try again later.");
                Console.WriteLine("Press any key to return to the main menu...");
                Console.ResetColor();
                Console.ReadKey(true);

                Console.Clear();

                Menu.DisplayMainMenu();
            }*/

            /*try
            {
                this.stream = client.GetStream();
                this.readThread = new Thread(this.Read);
                this.isReading = true;
                readThread.Start();
            }
            catch
            {
                this.FireOnConnectionLost();
            }*/
        }

        public void Stop()
        {
            try
            {
                this.isReading = false;
                this.Connected = false;
                this.stream.Close();
                this.client.Close();
                this.client = new TcpClient();
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
            while (this.isReading == true)
            {
                try
                {
                    if (!this.stream.DataAvailable)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    List<byte> receivedBytes = new List<byte>();
                    byte[] buffer = new byte[1];

                    while (true)
                    {
                        this.stream.Read(buffer, 0, 1);

                        // Checks if the current byte is the last byte of a protocol
                        if (buffer[0] == 33)
                        {
                            break;
                        }

                        receivedBytes.Add(buffer[0]);
                    }

                    this.FireOnDataReceived(receivedBytes.ToArray());
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
