using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    public class Menu
    {
        public static void DisplayMainMenu()
        {
            NetworkManager networkManager = new NetworkManager(IPAddress.Parse("10.0.0.34"));

            string[] menuItems = {"Join Game", "Create Game", "Help", "Exit"};
            int menuPosition = DisplayMenu(menuItems);

            Game game = new Game(networkManager);

            switch (menuPosition)
            {
                case 0:
                    game.JoinGame();
                    break;
                case 1:
                    game.CreateGame();
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
        }

        public static int DisplayMenu(string[] menuItems)
        {
            int cursorPosition = 0;

            while (true)
            {
                DisplayGameHeader();

                Console.WriteLine();

                for (int i = 0; i < menuItems.Length; i++)
                {
                    if (i == cursorPosition)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    Console.WriteLine(menuItems[i]);
                }

                ConsoleKeyInfo cki = Console.ReadKey(true);

                if (cki.Key == ConsoleKey.DownArrow)
                {
                    if (cursorPosition + 1 == menuItems.Length)
                    {
                        cursorPosition = 0;
                    }
                    else
                    {
                        cursorPosition++;
                    }
                }
                else if (cki.Key == ConsoleKey.UpArrow)
                {
                    if (cursorPosition - 1 < 0)
                    {
                        cursorPosition = menuItems.Length - 1;
                    }
                    else
                    {
                        cursorPosition--;
                    }
                }
                else if (cki.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    break;
                }

                Console.Clear();
            }

            return cursorPosition;
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