using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Card
    {
        public Card(Color color, Value value)
        {
            this.Color = color;
            this.Value = value;
        }

        public Color Color
        {
            get;
            private set;
        }

        public Value Value
        {
            get;
            private set;
        }

        public void Draw(int positionX, int positionY)
        {
            switch (this.Color)
            {
                case Color.Red:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case Color.Blue:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case Color.Green:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case Color.Yellow:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case Color.White:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }

            Console.SetCursorPosition(positionX, positionY);
            Console.WriteLine(" _____ ");

            Console.SetCursorPosition(positionX, positionY + 1);
            Console.WriteLine("|     |");

            Console.SetCursorPosition(positionX, positionY + 2);
            Console.WriteLine("|     |");

            Console.SetCursorPosition(positionX, positionY + 3);

            if (this.Value == Value.Skip)
            {
                Console.WriteLine("|  X  |");
            }
            else if (this.Value == Value.Reverse)
            {
                Console.WriteLine("| <-> |");
            }
            else if (this.Value == Value.DrawTwo)
            {
                Console.WriteLine("| +2  |");
            }
            else if (this.Value == Value.WildDrawFour)
            {
                Console.WriteLine("| +4  |");
            }
            else if (this.Value == Value.Wild)
            {
                Console.WriteLine("|COLOR|");
            }
            else if (int.TryParse(((char)this.Value).ToString(), out int checkIfNumeric))
            {
                Console.WriteLine("|  {0}  |", checkIfNumeric);
            }
            else if (this.Value == Value.Uno)
            {
                Console.WriteLine("| UNO |");
            }

            Console.SetCursorPosition(positionX, positionY + 4);
            Console.WriteLine("|     |");


            Console.SetCursorPosition(positionX, positionY + 5);
            Console.WriteLine("|_____|");

            Console.ResetColor();
        }
    }
}
