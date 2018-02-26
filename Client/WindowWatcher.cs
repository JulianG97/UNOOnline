using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Client
{
    public class WindowWatcher
    {
        private int height;
        private int width;
        private bool isWatching;

        public WindowWatcher(int height, int width)
        {
            this.height = height;
            this.width = width;
        }

        public void Start()
        {
            this.isWatching = true;
            Thread watchThread = new Thread(Worker);
            watchThread.Start();
        }

        public void Stop()
        {
            this.isWatching = false;
        }

        private void Worker()
        {
            while (this.isWatching == true)
            {
                if (Console.WindowHeight != this.height || Console.WindowWidth != this.width)
                {
                    try
                    {
                        Console.SetWindowSize(this.width, this.height);
                    }
                    catch { }

                    Console.CursorVisible = false;
                }

                Thread.Sleep(10);
            }
        }
    }
}
