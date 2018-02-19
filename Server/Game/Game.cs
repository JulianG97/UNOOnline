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

        public void PrepareGameStart()
        {
            this.SetDefaultDeck();
            this.DrawPile.Mix();
        }

        private void SetDefaultDeck()
        {
            this.DrawPile = new Deck();
            this.DrawPile.Cards = new List<Card>();

            // Add all red cards to deck
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 0));
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 1));
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 1));
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 2));
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 2));
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 3));
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 3));
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 4));
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 4));
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 5));
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 5));
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 6));
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 6));
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 7));
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 7));
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 8));
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 8));
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 9));
            this.DrawPile.Cards.Add(new NumericCard(Color.Red, 9));
            this.DrawPile.Cards.Add(new ActionCard(Color.Red, ActionCardType.Skip));
            this.DrawPile.Cards.Add(new ActionCard(Color.Red, ActionCardType.Skip));
            this.DrawPile.Cards.Add(new ActionCard(Color.Red, ActionCardType.Reverse));
            this.DrawPile.Cards.Add(new ActionCard(Color.Red, ActionCardType.Reverse));
            this.DrawPile.Cards.Add(new ActionCard(Color.Red, ActionCardType.DrawTwo));
            this.DrawPile.Cards.Add(new ActionCard(Color.Red, ActionCardType.DrawTwo));

            // Add all yellow cards to deck
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 0));
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 1));
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 1));
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 2));
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 2));
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 3));
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 3));
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 4));
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 4));
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 5));
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 5));
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 6));
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 6));
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 7));
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 7));
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 8));
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 8));
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 9));
            this.DrawPile.Cards.Add(new NumericCard(Color.Yellow, 9));
            this.DrawPile.Cards.Add(new ActionCard(Color.Yellow, ActionCardType.Skip));
            this.DrawPile.Cards.Add(new ActionCard(Color.Yellow, ActionCardType.Skip));
            this.DrawPile.Cards.Add(new ActionCard(Color.Yellow, ActionCardType.Reverse));
            this.DrawPile.Cards.Add(new ActionCard(Color.Yellow, ActionCardType.Reverse));
            this.DrawPile.Cards.Add(new ActionCard(Color.Yellow, ActionCardType.DrawTwo));
            this.DrawPile.Cards.Add(new ActionCard(Color.Yellow, ActionCardType.DrawTwo));

            // Add all green cards to deck
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 0));
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 1));
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 1));
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 2));
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 2));
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 3));
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 3));
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 4));
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 4));
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 5));
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 5));
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 6));
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 6));
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 7));
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 7));
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 8));
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 8));
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 9));
            this.DrawPile.Cards.Add(new NumericCard(Color.Green, 9));
            this.DrawPile.Cards.Add(new ActionCard(Color.Green, ActionCardType.Skip));
            this.DrawPile.Cards.Add(new ActionCard(Color.Green, ActionCardType.Skip));
            this.DrawPile.Cards.Add(new ActionCard(Color.Green, ActionCardType.Reverse));
            this.DrawPile.Cards.Add(new ActionCard(Color.Green, ActionCardType.Reverse));
            this.DrawPile.Cards.Add(new ActionCard(Color.Green, ActionCardType.DrawTwo));
            this.DrawPile.Cards.Add(new ActionCard(Color.Green, ActionCardType.DrawTwo));

            // Add all blue cards to deck
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 0));
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 1));
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 1));
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 2));
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 2));
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 3));
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 3));
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 4));
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 4));
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 5));
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 5));
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 6));
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 6));
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 7));
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 7));
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 8));
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 8));
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 9));
            this.DrawPile.Cards.Add(new NumericCard(Color.Blue, 9));
            this.DrawPile.Cards.Add(new ActionCard(Color.Blue, ActionCardType.Skip));
            this.DrawPile.Cards.Add(new ActionCard(Color.Blue, ActionCardType.Skip));
            this.DrawPile.Cards.Add(new ActionCard(Color.Blue, ActionCardType.Reverse));
            this.DrawPile.Cards.Add(new ActionCard(Color.Blue, ActionCardType.Reverse));
            this.DrawPile.Cards.Add(new ActionCard(Color.Blue, ActionCardType.DrawTwo));
            this.DrawPile.Cards.Add(new ActionCard(Color.Blue, ActionCardType.DrawTwo));

            // Add all wild cards to deck
            this.DrawPile.Cards.Add(new ActionCard(Color.Wild, ActionCardType.Wild));
            this.DrawPile.Cards.Add(new ActionCard(Color.Wild, ActionCardType.Wild));
            this.DrawPile.Cards.Add(new ActionCard(Color.Wild, ActionCardType.Wild));
            this.DrawPile.Cards.Add(new ActionCard(Color.Wild, ActionCardType.Wild));
            this.DrawPile.Cards.Add(new ActionCard(Color.Wild, ActionCardType.WildDrawFour));
            this.DrawPile.Cards.Add(new ActionCard(Color.Wild, ActionCardType.WildDrawFour));
            this.DrawPile.Cards.Add(new ActionCard(Color.Wild, ActionCardType.WildDrawFour));
            this.DrawPile.Cards.Add(new ActionCard(Color.Wild, ActionCardType.WildDrawFour));
        }
    }
}