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
            this.MixUntilValidFirstCard();
            this.discardPile = new Deck();
            this.discardPile.Cards = new List<Card>();
            this.discardPile.AddCard(drawPile.DrawCard());
            this.playerWhoIsOnTurn = this.Players[0];
            this.SendRoundInformation();
        }

        public void NewCardSet(string[] setCardArray)
        {
            if (setCardArray.Length == 5)
            {
                foreach (Player player in this.Players)
                {
                    if (player.PlayerID.ToString() == setCardArray[1])
                    {
                        Card card = null;

                        if (Enum.TryParse(setCardArray[2], out Color color))
                        {
                            if (int.TryParse(setCardArray[3], out int number) == false)
                            {
                                if (Enum.TryParse(setCardArray[3], out ActionCardType type))
                                {
                                    card = new ActionCard(color, type);
                                }
                            }
                            else
                            {
                                card = new NumericCard(color, number);
                            }

                            if (int.TryParse(setCardArray[4], out int uno) == true)
                            {
                                if (uno == 0 || uno == 1)
                                {
                                    if (this.CheckIfValidCard(card, player) == true)
                                    {
                                        this.ExecuteCardEffect(card, player, uno);

                                        player.NetworkManager.Send(ProtocolManager.OK());
                                    }
                                    else
                                    {
                                        player.NetworkManager.Send(ProtocolManager.Invalid());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool CheckIfValidCard(Card cardToCheck, Player player)
        {
            bool playerOwnsCard = false;
            bool cardCanBePlayed = false;

            foreach (Card card in player.Deck.Cards)
            {
                if (card == cardToCheck)
                {
                    playerOwnsCard = true;
                    break;
                }
            }

            if (playerOwnsCard == false)
            {
                cardCanBePlayed = false;
            }
            else
            {
                if (cardToCheck is ActionCard)
                {
                    ActionCard ac = (ActionCard)cardToCheck;

                    // Checks if the card is a wild card. Then it can be always played.
                    if (ac.Type == ActionCardType.Wild || ac.Type == ActionCardType.WildDrawFour)
                    {
                        cardCanBePlayed = true;
                    }
                    // Checks if the last card is an action card and if it has the same type as the card to be played.
                    else if (this.discardPile.Cards[0] is ActionCard)
                    {
                        ActionCard lc = (ActionCard)this.discardPile.Cards[0];

                        if (ac.Type == lc.Type)
                        {
                            cardCanBePlayed = true;
                        }
                    }
                }
                // Checks if the last card and the card to be played have the same color.
                else if (cardToCheck.Color == this.discardPile.Cards[0].Color)
                {
                    cardCanBePlayed = true;
                }
                // Checks if the last card is a numeric card and if it has the same number as the card to be played.
                else if (cardToCheck is NumericCard)
                {
                    NumericCard nc = (NumericCard)cardToCheck;

                    if (this.discardPile.Cards[0] is NumericCard)
                    {
                        NumericCard lc = (NumericCard)this.discardPile.Cards[0];

                        if (nc.Number == lc.Number)
                        {
                            cardCanBePlayed = true;
                        }
                    }
                }
            }

            return cardCanBePlayed;
        }

        public void ExecuteCardEffect(Card card, Player player, int unoYesOrNo)
        {
            this.discardPile.AddCard(card);

            if (card is ActionCard)
            {
                ActionCard ac = (ActionCard)card;

                // If the card to be played is a wild card a card with the same action card type gets removed from the player deck.
                if (ac.Type == ActionCardType.Wild || ac.Type == ActionCardType.WildDrawFour)
                {
                    foreach (Card playerCard in player.Deck.Cards)
                    {
                        if (playerCard is ActionCard)
                        {
                            ActionCard pac = (ActionCard)playerCard;

                            if (ac.Type == pac.Type)
                            {
                                player.Deck.Cards.Remove(playerCard);
                            }
                        }
                    }
                }
            }
            else
            {
                player.Deck.Cards.Remove(card);
            }

            ChangePlayerTurn();
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

        private void MixUntilValidFirstCard()
        {
            while (true)
            {
                if (drawPile.Cards[0].Color != Color.White)
                {
                    break;
                }

                drawPile.Mix();
            }
        }

        private void ChangePlayerTurn()
        {
            if (this.playerWhoIsOnTurn.PlayerID + 1 > this.Players.Count)
            {
                this.playerWhoIsOnTurn = this.Players[0];
            }
            else
            {
                this.playerWhoIsOnTurn = this.Players[this.playerWhoIsOnTurn.PlayerID + 1];
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
            this.drawPile.Cards.Add(new ActionCard(Color.White, ActionCardType.Wild));
            this.drawPile.Cards.Add(new ActionCard(Color.White, ActionCardType.Wild));
            this.drawPile.Cards.Add(new ActionCard(Color.White, ActionCardType.Wild));
            this.drawPile.Cards.Add(new ActionCard(Color.White, ActionCardType.Wild));
            this.drawPile.Cards.Add(new ActionCard(Color.White, ActionCardType.WildDrawFour));
            this.drawPile.Cards.Add(new ActionCard(Color.White, ActionCardType.WildDrawFour));
            this.drawPile.Cards.Add(new ActionCard(Color.White, ActionCardType.WildDrawFour));
            this.drawPile.Cards.Add(new ActionCard(Color.White, ActionCardType.WildDrawFour));
        }
    }
}