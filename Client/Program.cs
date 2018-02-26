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
            Console.WindowHeight = 40;
            Console.WindowWidth = 50;

            WindowWatcher windowWatcher = new WindowWatcher(40, 70);
            windowWatcher.Start();

            Menu.DisplayMainMenu();
        }
    }
}
