using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Server
{
    public class Game
    {
        private Deck drawPile;
        private Deck discardPile;
        private Player playerWhoIsOnTurn;

        public Game(int gameID, int playersNeeded)
        {
            this.GameID = gameID;
            this.PlayersNeeded = playersNeeded;
            this.JoinedPlayers = 1;
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
            this.drawPile.Mix();
            this.ServeCards();
            Thread.Sleep(100);
            this.discardPile = new Deck();
            this.discardPile.Cards = new List<Card>();
            this.discardPile.AddCard(drawPile.DrawCard());
            this.playerWhoIsOnTurn = this.Players[0];
            this.SendRoundInformation();
            Thread.Sleep(100);
        }

        private void ServeCards()
        {
            foreach (Player player in this.Players)
            {
                player.Deck = new Deck();
                player.Deck.Cards = new List<Card>();

                for (int i = 0; i < 7; i++)
                {
                    player.Deck.AddCard(this.drawPile.DrawCard());
                }

                player.NetworkManager.Send(ProtocolManager.PlayerCards(player));
            }
        }

        private void SendRoundInformation()
        {
            foreach (Player player in this.Players)
            {
                player.NetworkManager.Send(ProtocolManager.RoundInformation(this.discardPile.Cards[0], this.playerWhoIsOnTurn, this.Players));
            }
        }

        private void SetDefaultDeck()
        {
            this.drawPile = new Deck();
            this.drawPile.Cards = new List<Card>();

            // Add all red cards to deck
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 0));
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 1));
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 1));
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 2));
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 2));
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 3));
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 3));
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 4));
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 4));
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 5));
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 5));
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 6));
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 6));
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 7));
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 7));
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 8));
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 8));
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 9));
            this.drawPile.Cards.Add(new NumericCard(Color.Red, 9));
            this.drawPile.Cards.Add(new ActionCard(Color.Red, ActionCardType.Skip));
            this.drawPile.Cards.Add(new ActionCard(Color.Red, ActionCardType.Skip));
            this.drawPile.Cards.Add(new ActionCard(Color.Red, ActionCardType.Reverse));
            this.drawPile.Cards.Add(new ActionCard(Color.Red, ActionCardType.Reverse));
            this.drawPile.Cards.Add(new ActionCard(Color.Red, ActionCardType.DrawTwo));
            this.drawPile.Cards.Add(new ActionCard(Color.Red, ActionCardType.DrawTwo));

            // Add all yellow cards to deck
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 0));
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 1));
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 1));
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 2));
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 2));
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 3));
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 3));
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 4));
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 4));
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 5));
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 5));
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 6));
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 6));
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 7));
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 7));
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 8));
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 8));
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 9));
            this.drawPile.Cards.Add(new NumericCard(Color.Yellow, 9));
            this.drawPile.Cards.Add(new ActionCard(Color.Yellow, ActionCardType.Skip));
            this.drawPile.Cards.Add(new ActionCard(Color.Yellow, ActionCardType.Skip));
            this.drawPile.Cards.Add(new ActionCard(Color.Yellow, ActionCardType.Reverse));
            this.drawPile.Cards.Add(new ActionCard(Color.Yellow, ActionCardType.Reverse));
            this.drawPile.Cards.Add(new ActionCard(Color.Yellow, ActionCardType.DrawTwo));
            this.drawPile.Cards.Add(new ActionCard(Color.Yellow, ActionCardType.DrawTwo));

            // Add all green cards to deck
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 0));
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 1));
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 1));
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 2));
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 2));
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 3));
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 3));
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 4));
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 4));
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 5));
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 5));
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 6));
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 6));
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 7));
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 7));
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 8));
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 8));
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 9));
            this.drawPile.Cards.Add(new NumericCard(Color.Green, 9));
            this.drawPile.Cards.Add(new ActionCard(Color.Green, ActionCardType.Skip));
            this.drawPile.Cards.Add(new ActionCard(Color.Green, ActionCardType.Skip));
            this.drawPile.Cards.Add(new ActionCard(Color.Green, ActionCardType.Reverse));
            this.drawPile.Cards.Add(new ActionCard(Color.Green, ActionCardType.Reverse));
            this.drawPile.Cards.Add(new ActionCard(Color.Green, ActionCardType.DrawTwo));
            this.drawPile.Cards.Add(new ActionCard(Color.Green, ActionCardType.DrawTwo));

            // Add all blue cards to deck
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 0));
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 1));
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 1));
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 2));
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 2));
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 3));
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 3));
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 4));
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 4));
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 5));
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 5));
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 6));
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 6));
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 7));
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 7));
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 8));
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 8));
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 9));
            this.drawPile.Cards.Add(new NumericCard(Color.Blue, 9));
            this.drawPile.Cards.Add(new ActionCard(Color.Blue, ActionCardType.Skip));
            this.drawPile.Cards.Add(new ActionCard(Color.Blue, ActionCardType.Skip));
            this.drawPile.Cards.Add(new ActionCard(Color.Blue, ActionCardType.Reverse));
            this.drawPile.Cards.Add(new ActionCard(Color.Blue, ActionCardType.Reverse));
            this.drawPile.Cards.Add(new ActionCard(Color.Blue, ActionCardType.DrawTwo));
            this.drawPile.Cards.Add(new ActionCard(Color.Blue, ActionCardType.DrawTwo));

            // Add all wild cards to deck
            this.drawPile.Cards.Add(new ActionCard(Color.Wild, ActionCardType.Wild));
            this.drawPile.Cards.Add(new ActionCard(Color.Wild, ActionCardType.Wild));
            this.drawPile.Cards.Add(new ActionCard(Color.Wild, ActionCardType.Wild));
            this.drawPile.Cards.Add(new ActionCard(Color.Wild, ActionCardType.Wild));
            this.drawPile.Cards.Add(new ActionCard(Color.Wild, ActionCardType.WildDrawFour));
            this.drawPile.Cards.Add(new ActionCard(Color.Wild, ActionCardType.WildDrawFour));
            this.drawPile.Cards.Add(new ActionCard(Color.Wild, ActionCardType.WildDrawFour));
            this.drawPile.Cards.Add(new ActionCard(Color.Wild, ActionCardType.WildDrawFour));
        }
    }
}