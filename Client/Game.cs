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
    public class Game
    {
        private int players;
        private bool onTurn;
        private NetworkManager networkManager;
        private bool validAction;
        private string roomList;
        
        public Game(NetworkManager networkManager)
        {
            this.networkManager = networkManager;
            this.networkManager.OnConnectionsLost += this.ConnectionLost;
            this.networkManager.OnDataReceived += this.DataReceived;
            this.validAction = false;
        }

        public void CreateGame()
        {
            int amountOfPlayers = 2;

            while (true)
            {
                Menu.DisplayGameHeader();

                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("Please set the number of players with the UP ARROW and DOWN ARROW!");
                Console.WriteLine("Press ENTER to confirm and create a new game!");
                Console.WriteLine();
                Console.Write("Players: ");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(amountOfPlayers);

                ConsoleKeyInfo cki = Console.ReadKey(true);

                if (cki.Key == ConsoleKey.UpArrow)
                {
                    if (amountOfPlayers + 1 > 4)
                    {
                        amountOfPlayers = 2;
                    }
                    else
                    {
                        amountOfPlayers++;
                    }
                }
                else if (cki.Key == ConsoleKey.DownArrow)
                {
                    if (amountOfPlayers - 1 < 2)
                    {
                        amountOfPlayers = 4;
                    }
                    else
                    {
                        amountOfPlayers--;
                    }
                }
                else if (cki.Key == ConsoleKey.Enter)
                {
                    this.players = amountOfPlayers;

                    Console.Clear();

                    this.networkManager.Start();
                    this.networkManager.Send(ProtocolManager.CreateGame(this.players.ToString()));
                    break;
                }

                Console.Clear();
            }
        }

        public void JoinGame()
        {
            this.networkManager.Start();
            string[] roomArray = null;
            this.networkManager.Send(ProtocolManager.RequestRooms());

            Thread.Sleep(100);

            int position = 0;

            while (true)
            {
                Menu.DisplayGameHeader();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("[UP/DOWN ARROW] Navigate [R] Refresh room list [E] Go back to main menu");
                Console.WriteLine();

                if (this.roomList == null || this.roomList == string.Empty)
                {
                    Console.WriteLine("There aren't any rooms open to join!");
                    Console.WriteLine("Please create a new game...");
                }
                else
                {
                    roomArray = this.roomList.Split('-');

                    for (int i = 0; i < roomArray.Length; i += 3)
                    {
                        if (i == position * 3)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }

                        Console.WriteLine("Room ID: {0}", roomArray[i]);
                        Console.WriteLine("Players: {0}/{1}", roomArray[i + 1], roomArray[i + 2]);
                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

                ConsoleKeyInfo cki = Console.ReadKey(true);

                if (cki.Key == ConsoleKey.R)
                {
                    this.networkManager.Send(ProtocolManager.RequestRooms());
                }
                else if (cki.Key == ConsoleKey.E)
                {
                    break;
                }
                else if (cki.Key == ConsoleKey.UpArrow)
                {
                    if (roomArray != null)
                    {
                        if (position - 1 < 0)
                        {
                            position = (roomArray.Length / 3) - 1;
                        }
                        else
                        {
                            position--;
                        }
                    }
                }
                else if (cki.Key == ConsoleKey.DownArrow)
                {
                    if (roomArray != null)
                    {
                        if (position + 1 > (roomArray.Length / 3) - 1)
                        {
                            position = 0;
                        }
                        else
                        {
                            position++;
                        }
                    }
                }
                else if (cki.Key == ConsoleKey.Enter)
                {
                    if (roomArray != null)
                    {
                        this.networkManager.Send(ProtocolManager.JoinGame(roomArray[position * 3]));

                        Console.Clear();

                        Menu.DisplayGameHeader();
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.White;

                        if (this.validAction == true)
                        {
                            Console.WriteLine("Please wait until enough players joined the game...");
                        }
                        else if (this.validAction == false)
                        {
                            Console.WriteLine("The room you tried to join isn't open or already full!");
                            Console.WriteLine("Press any key to continue!");

                            Console.ReadKey(true);

                            this.networkManager.Send(ProtocolManager.RequestRooms());
                            roomArray = this.roomList.Split('-');
                        }
                    }
                }

                Console.Clear();
            }

            this.networkManager.Stop();

            Console.Clear();

            Menu.DisplayMainMenu();
        }

        public void Start()
        {
            ShowPlayerStats("3-7-5-13-2");

            Console.ReadKey(true);
        }

        public void ShowPlayerStats(string playerStats)
        {
            string[] playerStatsArray = playerStats.Split('-');

            for (int i = 1; i <= this.players; i++)
            {
                Console.ForegroundColor = ConsoleColor.White;

                Console.Write("Player {0} | Turn: ", i);

                if (i == int.Parse(playerStatsArray[0]))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("true ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("false");
                }

                Console.ForegroundColor = ConsoleColor.White;

                Console.Write(" | Cards: {0}", playerStatsArray[i]);

                Console.WriteLine();
            }
        }

        private void DataReceived(object sender, OnDataReceivedEventArgs args)
        {
            if (args.Protocol != null)
            {
                if (args.Protocol.Type.SequenceEqual(ProtocolTypes.OK))
                {
                    this.validAction = true;
                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.Invalid))
                {
                    this.validAction = false;
                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.GameStart))
                {

                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.GameOver))
                {

                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.PlayerCards))
                {

                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.RoomList))
                {
                    this.roomList = Encoding.ASCII.GetString(args.Protocol.Content);
                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.RoundInformation))
                {

                }
            }
        }

        private void ConnectionLost(object sender, EventArgs args)
        {
            // If connection to server is lost
        }
    }
}
