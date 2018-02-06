using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Player
    {
        public Deck Deck
        {
            get => default(Deck);
            set
            {
            }
        }

        public void DiscardCard()
        {
            throw new System.NotImplementedException();
        }

        public void DrawCard()
        {
            throw new System.NotImplementedException();
        }
    }
}