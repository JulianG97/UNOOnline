using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Game
    {
        private Deck DiscardPile;
        private Deck DrawPile;

        public List<Player> Players
        {
            get => default(List<Player>);
            set
            {
            }
        }
    }
}