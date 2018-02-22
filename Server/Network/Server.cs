using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class Server
    {
        private TcpListener listener;
        private Thread listenerThread;
        private List<Game> games;
        private int nextGameID;

        public bool isRunning
        {
            get;
            private set;
        }

        public Server()
        {
            this.nextGameID = 1;
        }

        public void Start()
        {
            this.games = new List<Game>();
            this.listener = new TcpListener(IPAddress.Any, 3000);
            this.listenerThread = new Thread(this.StartListening);
            listenerThread.Start();
            this.isRunning = true;

            Console.WriteLine("The server was started successfully!");
        }

        public void StartListening()
        {
            this.listener.Start();

            while (this.isRunning == true)
            {
                if (!this.listener.Pending())
                {
                    Thread.Sleep(100);
                    continue;
                }

                TcpClient client = this.listener.AcceptTcpClient();
                Thread session = new Thread(new ParameterizedThreadStart(HandleNewSession));
                session.Start(client);
            }
        }

        public void HandleNewSession(object data)
        {
            TcpClient client = (TcpClient)data;
            NetworkManager networkManager = new NetworkManager(client);
            networkManager.OnConnectionsLost += this.ConnectionLost;
            networkManager.OnDataReceived += this.DataReceived;
            networkManager.Start();
        }

        public void Stop()
        {
            // Stop all connected network managers
            this.listener.Stop();
            this.isRunning = false;
            Console.WriteLine("The server was stopped successfully!");
        }

        private void ConnectionLost(object sender, EventArgs args)
        {
            // Cards of the player moves to the bottom of the discard pile
            // Player gets removed from the game
            // Game continues if there are still at least two players
            // Else the game ends and the player who is still connected wins the game
        }

        private void DataReceived(object sender, OnDataReceivedEventArgs args)
        {
            if (args.Protocol != null)
            {
                if (args.Protocol.Type.SequenceEqual(ProtocolTypes.CreateGame))
                {
                    int integer;

                    bool isInteger = int.TryParse(Encoding.ASCII.GetString(args.Protocol.Content), out integer);

                    if (isInteger == true)
                    {
                        this.CreateNewGame(integer, args.NetworkManager);
                    }
                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.JoinGame))
                {
                    int integer;

                    bool isInteger = int.TryParse(Encoding.ASCII.GetString(args.Protocol.Content), out integer);

                    if (isInteger == true)
                    {
                        this.JoinGame(integer, args.NetworkManager);
                    }
                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.RequestRooms))
                {
                    this.SendRoomList(args.NetworkManager);
                }
                else if (args.Protocol.Type.SequenceEqual(ProtocolTypes.SetCard))
                {
                    string setCardString = Encoding.ASCII.GetString(args.Protocol.Content);
                    string[] setCardArray = setCardString.Split('-');

                    if (setCardArray.Length == 5)
                    {
                        foreach (Game game in this.games)
                        {
                            if (game.GameID.ToString() == setCardArray[0])
                            {
                                foreach (Player player in game.Players)
                                {
                                    if (player.PlayerID.ToString() == setCardArray[1])
                                    {
                                        Card card = null;

                                        if (Enum.TryParse(setCardArray[2], out Color color))
                                        {
                                            if (int.TryParse(setCardArray[3], out int number) == false)
                                            {
                                                if (Enum.TryParse(setCardArray[3], out ActionCardType type))
                                                {
                                                    card = new ActionCard(color, type);
                                                }
                                            }
                                            else
                                            {
                                                card = new NumericCard(color, number);
                                            }

                                            if (int.TryParse(setCardArray[4], out int uno) == true)
                                            {
                                                if (uno == 0 || uno == 1)
                                                {
                                                    if (game.CheckIfValidCard(card, player) == true)
                                                    {
                                                        game.ExecuteCardEffect(card, player, uno);

                                                        player.NetworkManager.Send(ProtocolManager.OK());
                                                    }
                                                    else
                                                    {
                                                        player.NetworkManager.Send(ProtocolManager.Invalid());
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

            private void CreateNewGame(int amountOfPlayers, NetworkManager networkManager)
            {
                Game game = new Game(this.nextGameID, amountOfPlayers);
                this.nextGameID++;
                game.Players = new List<Player>();
                game.Players.Add(new Player(1, networkManager));
                this.games.Add(game);

                if (amountOfPlayers < 2 || amountOfPlayers > 4)
                {
                    networkManager.Send(ProtocolManager.Invalid());
                }
                else
                {
                    networkManager.Send(ProtocolManager.OK());

                    Console.WriteLine("{0} created a new game (GameID: {1}, Players: {2}/{3})!", ((IPEndPoint)networkManager.PlayerClient.Client.RemoteEndPoint).Address.ToString(), game.GameID, game.JoinedPlayers, game.PlayersNeeded);
                }
            }

            private void JoinGame(int gameID, NetworkManager networkManager)
            {
                bool gameFound = false;

                foreach (Game game in games)
                {
                    if (game.GameID == gameID)
                    {
                        if (game.JoinedPlayers < game.PlayersNeeded)
                        {
                            game.Players.Add(new Player(game.JoinedPlayers + 1, networkManager));
                            game.JoinedPlayers++;
                            gameFound = true;
                            networkManager.Send(ProtocolManager.OK());

                            Console.WriteLine("{0} joined a game (GameID: {1}, PlayerID: {2}, Players: {3}/{4})!", ((IPEndPoint)networkManager.PlayerClient.Client.RemoteEndPoint).Address.ToString(), game.GameID, game.JoinedPlayers, game.JoinedPlayers, game.PlayersNeeded);

                            if (game.JoinedPlayers == game.PlayersNeeded)
                            {
                                this.StartGame(game);
                            }
                        }
                        else
                        {
                            networkManager.Send(ProtocolManager.Invalid());
                        }

                        break;
                    }
                }

                if (gameFound == false)
                {
                    networkManager.Send(ProtocolManager.Invalid());
                }
            }

            private void StartGame(Game game)
            {
                game.PrepareGameStart();

                foreach (Player player in game.Players)
                {
                    player.NetworkManager.Send(ProtocolManager.GameStart(game.GameID.ToString(), player.PlayerID.ToString()));
                }

                Console.WriteLine("A game started (GameID: {0}, Players: {1}/{2})!", game.GameID, game.JoinedPlayers, game.PlayersNeeded);

                game.Start();
            }

            private void SendRoomList(NetworkManager networkManager)
            {
                networkManager.Send(ProtocolManager.RoomList(games));
            }
        }
    }