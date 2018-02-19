using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class NumericCard : Card
    {
        public NumericCard(Color color, int number)
        {
            this.Color = color;
            this.Number = number;
        }

        public int Number
        {
            get;
            set;
        }
    }
}