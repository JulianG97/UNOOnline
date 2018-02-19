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
        private bool serverResponseReceived;
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

                    this.WaitForServerResponse();

                    if (validAction == true)
                    {
                        this.DisplayWaitingScreen();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine();
                        Console.WriteLine("The game couldn't be created!");
                        Console.WriteLine("Press any key to return to the main menu...");

                        Console.ReadKey(true);
                    }

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

            this.WaitForServerResponse();

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

                    this.WaitForServerResponse();
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

                        this.WaitForServerResponse();

                        Console.Clear();

                        Menu.DisplayGameHeader();
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.White;

                        if (this.validAction == true)
                        {
                            DisplayWaitingScreen();
                        }
                        else if (this.validAction == false)
                        {
                            Console.WriteLine("The room you tried to join isn't open or already full!");
                            Console.WriteLine("Press any key to continue!");

                            Console.ReadKey(true);

                            this.networkManager.Send(ProtocolManager.RequestRooms());

                            this.WaitForServerResponse();

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

        private void DisplayWaitingScreen()
        {
            Menu.DisplayGameHeader();
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Please wait until enough players joined the game and the game starts!");

            this.WaitForServerResponse();
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
                    this.serverResponseReceived = true;
                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.Invalid))
                {
                    this.validAction = false;
                    this.serverResponseReceived = true;
                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.GameStart))
                {
                    this.serverResponseReceived = true;
                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.GameOver))
                {
                    this.serverResponseReceived = true;
                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.PlayerCards))
                {
                    this.serverResponseReceived = true;
                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.RoomList))
                {
                    this.roomList = Encoding.ASCII.GetString(args.Protocol.Content);
                    this.serverResponseReceived = true;
                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.RoundInformation))
                {
                    this.serverResponseReceived = true;
                }
            }
        }

        private void WaitForServerResponse()
        {
            while (this.serverResponseReceived == false)
            { }

            this.serverResponseReceived = false;
        }

        private void ConnectionLost(object sender, EventArgs args)
        {
            // If connection to server is lost
        }
    }
}
