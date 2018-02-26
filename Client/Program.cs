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

            WindowWatcher windowWatcher = new WindowWatcher(25, 75);
            windowWatcher.Start();

            Menu.DisplayMainMenu();
        }
    }
}
