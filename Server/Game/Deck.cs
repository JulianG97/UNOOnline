using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Deck
    {
        public List<Card> Cards
        {
            get;
            set;
        }

        public void AddCard()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveCard()
        {
            throw new System.NotImplementedException();
        }

        public void Mix()
        {
            Random random = new Random();

            for (int i = 0; i < 1000; i++)
            {
                int r1 = random.Next(0, 108);

                int r2 = random.Next(0, 108);

                Card temp = this.Cards[r1];
                this.Cards[r1] = this.Cards[r2];
                this.Cards[r2] = temp;
            }
        }
    }
}