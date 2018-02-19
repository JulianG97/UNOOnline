using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Game game = new Game(1, 4);
            game.PrepareGameStart();
            Console.ReadKey();
            Server server = new Server();

            DisplayGameHeader();

            Console.ForegroundColor = ConsoleColor.White;
            Console.CursorVisible = false;

            Console.WriteLine();
            Console.WriteLine("[S] Start server [Q] Stop server [E] Exit server");
            Console.WriteLine();

            while (true)
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);

                if (cki.Key == ConsoleKey.S)
                {
                    if (server.isRunning == false)
                    {
                        server.Start();
                    }
                    else
                    {
                        Console.WriteLine("The server is already running!");
                    }
                }
                else if (cki.Key == ConsoleKey.Q)
                {
                    if (server.isRunning == true)
                    {
                        server.Stop();
                    }
                    else
                    {
                        Console.WriteLine("The server isn't running!");
                    }
                }
                else if (cki.Key == ConsoleKey.E)
                {
                    if (server.isRunning == true)
                    {
                        server.Stop();
                    }

                    Environment.Exit(0);
                    break;
                }
            }
        }

        public static void DisplayGameHeader()
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("  _    _                ____        _ _            ");
            Console.WriteLine(" | |  | |              / __ \\      | (_)           ");
            Console.WriteLine(" | |  | |_ __   ___   | |  | |_ __ | |_ _ __   ___ ");
            Console.WriteLine(" | |  | | '_ \\ / _ \\  | |  | | '_ \\| | | '_ \\ / _ \\");
            Console.WriteLine(" | |__| | | | | (_) | | |__| | | | | | | | | |  __/");
            Console.WriteLine("  \\____/|_| |_|\\___/   \\____/|_| |_|_|_|_| |_|\\___|");

            Console.ResetColor();
        }
    }
}
