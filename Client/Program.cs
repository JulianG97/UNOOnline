using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.WindowHeight = 25;
            Console.WindowWidth = 40;

            WindowWatcher windowWatcher = new WindowWatcher(25, 40);
            windowWatcher.Start();

            Menu.DisplayMainMenu();
        }
    }
}
