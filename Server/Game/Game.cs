using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Game
    {
        private Deck DrawPile;

        public Game(int gameID, int playersNeeded)
        {
            this.GameID = gameID;
            this.PlayersNeeded = playersNeeded;
            this.JoinedPlayers = 1;
        }

        public Deck DiscardPile
        {
            get;
            private set;
        }

        public Player PlayerWhoIsOnTurn
        {
            get;
            private set;
        }

        public int GameID
        {
            get;
            set;
        }

        public int PlayersNeeded
        {
            get;
            set;
        }
        public int JoinedPlayers
        {
            get;
            set;
        }

        public List<Player> Players
        {
            get;
            set;
        }
    }
}