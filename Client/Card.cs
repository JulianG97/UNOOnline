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
    }
}
