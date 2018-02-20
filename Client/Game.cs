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
        private NetworkManager networkManager;
        private bool validAction;
        private bool serverResponseReceived;
        private string roomList;
        private int playerID;
        private int playerIDWhoIsOnTurn;
        private int lobbyID;
        private Card lastCard;
        private List<Card> Deck;
        private List<string> numberOfCardsOfPlayers;
        
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
                    Console.Clear();

                    this.networkManager.Start();
                    this.networkManager.Send(ProtocolManager.CreateGame(amountOfPlayers.ToString()));

                    this.WaitForServerResponse();

                    if (validAction == true)
                    {
                        this.DisplayWaitingScreen();
                        this.Start();
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

                        if (this.validAction == true)
                        {
                            DisplayWaitingScreen();
                            this.Start();
                        }
                        else if (this.validAction == false)
                        {
                            Menu.DisplayGameHeader();
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.White;

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

            for (int i = 0; i < 3; i++)
            {
                this.WaitForServerResponse();
            }
        }

        public void Start()
        {
            Console.Clear();
            this.ShowPlayerStats();
        }

        public void ShowPlayerStats()
        {
            for (int i = 0, playerID = 1; i < numberOfCardsOfPlayers.Count; i++, playerID++)
            {
                Console.ForegroundColor = ConsoleColor.White;

                Console.Write("Player {0} | Turn: ", playerID);

                if (playerID == this.playerIDWhoIsOnTurn)
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

                Console.Write(" | Cards: {0}", numberOfCardsOfPlayers[i]);

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
                    int lobbyID;
                    int playerID;

                    bool isInteger = int.TryParse(Encoding.ASCII.GetString(new[] { args.Protocol.Content[0] }), out lobbyID);
                    bool isInteger2 = int.TryParse(Encoding.ASCII.GetString(new[] { args.Protocol.Content[2] }), out playerID);

                    if (isInteger == true && isInteger2 == true)
                    {
                        this.lobbyID = lobbyID;
                        this.playerID = playerID;
                        this.serverResponseReceived = true;
                    }
                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.GameOver))
                {
                    
                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.PlayerCards))
                {
                    this.Deck = new List<Card>();

                    string cards = Encoding.ASCII.GetString(args.Protocol.Content);
                    string[] cardArray = cards.Split('-');

                    char[] cardCharArray = new char[cardArray.Length];

                    for (int i = 0; i < cardArray.Length; i++)
                    {
                        cardCharArray[i] = Convert.ToChar(cardArray[i]);
                    }

                    for (int i = 0; i < cardArray.Length; i += 2)
                    {
                        Color color = (Color)cardCharArray[i];
                        Value value = (Value)cardCharArray[i + 1];

                        this.Deck.Add(new Card(color, value));
                    }

                    this.serverResponseReceived = true;
                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.RoomList))
                {
                    this.roomList = Encoding.ASCII.GetString(args.Protocol.Content);
                    this.serverResponseReceived = true;
                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.RoundInformation))
                {
                    string roundInformation = Encoding.ASCII.GetString(args.Protocol.Content);
                    string[] roundInformationArray = roundInformation.Split('-');

                    char[] roundInformationCharArray = new char[roundInformationArray.Length];

                    for (int i = 0; i < roundInformationArray.Length; i++)
                    {
                        roundInformationCharArray[i] = Convert.ToChar(roundInformationArray[i]);
                    }

                    Color color = (Color)roundInformationCharArray[0];
                    Value value = (Value)roundInformationCharArray[1];

                    this.lastCard = new Card(color, value);

                    this.playerIDWhoIsOnTurn = Int32.Parse(roundInformationArray[2]);

                    this.numberOfCardsOfPlayers = new List<string>();

                    for (int i = 3; i < roundInformationArray.Length; i++)
                    {
                        this.numberOfCardsOfPlayers.Add(roundInformationArray[i]);
                    }

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
