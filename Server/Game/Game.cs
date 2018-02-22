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
        private Direction direction;

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
            this.direction = Direction.Right;
            this.SetDefaultDeck();
            this.drawPile.Mix();
            this.ServeCards();
            this.MixUntilValidFirstCard();
            this.discardPile = new Deck();
            this.discardPile.Cards = new List<Card>();
            this.discardPile.AddCard(drawPile.DrawCard());
            this.playerWhoIsOnTurn = this.Players[0];
            this.ExecuteCardEffect(this.discardPile.Cards[0], this.playerWhoIsOnTurn, true);
            this.SendPlayerCards();
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
                        Color color;

                        if (Enum.IsDefined(typeof(Color), (int)(setCardArray[2].ToCharArray()[0])) == true)
                        {
                            color = (Color)(setCardArray[2].ToCharArray()[0]);

                            if (int.TryParse(setCardArray[3], out int number) == false)
                            {
                                if (Enum.IsDefined(typeof(ActionCardType), (int)(setCardArray[3].ToCharArray()[0])) == true)
                                {
                                    ActionCardType type = (ActionCardType)(setCardArray[3].ToCharArray()[0]);
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
                                        this.ExecuteCardEffect(card, this.GetNextPlayer(), false);
                                        this.RemoveCardAfterSet(card, player);

                                        player.NetworkManager.Send(ProtocolManager.OK());

                                        ChangePlayerTurn();
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
                if (card.Color == cardToCheck.Color)
                {
                    if (cardToCheck is ActionCard && card is ActionCard)
                    {
                        ActionCard ctc = (ActionCard)cardToCheck;
                        ActionCard c = (ActionCard)card;

                        if (ctc.Type == c.Type)
                        {
                            playerOwnsCard = true;
                            break;
                        }
                    }
                    else if (cardToCheck is NumericCard && card is NumericCard)
                    {
                        NumericCard ctc = (NumericCard)cardToCheck;
                        NumericCard c = (NumericCard)card;

                        if (ctc.Number == c.Number)
                        {
                            playerOwnsCard = true;
                            break;
                        }
                    }
                }
            }

            if (playerOwnsCard == false)
            {
                cardCanBePlayed = false;
            }
            else
            {
                // Checks if the last card and the card to be played have the same color.
                if (cardToCheck.Color == this.discardPile.Cards[0].Color)
                {
                    cardCanBePlayed = true;
                }
                else if (cardToCheck is ActionCard)
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

        public void ExecuteCardEffect(Card card, Player playerToBeAffected, bool firstRound)
        {
            this.discardPile.AddCard(card);

            if (card is ActionCard)
            {
                ActionCard ac = (ActionCard)card;

                if (ac.Type == ActionCardType.WildDrawFour)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        playerToBeAffected.Deck.AddCard(this.drawPile.DrawCard());
                    }

                    if (firstRound == true)
                    {
                        this.playerWhoIsOnTurn = this.GetNextPlayer();
                    }
                    else
                    {
                        this.playerWhoIsOnTurn = playerToBeAffected;
                    }
                }
                else if (ac.Type == ActionCardType.Reverse)
                {
                    if (this.Players.Count() == 2)
                    {
                        this.playerWhoIsOnTurn = playerToBeAffected;
                    }

                    if (this.direction == Direction.Left)
                    {
                        this.direction = Direction.Right;
                    }
                    else if (this.direction == Direction.Right)
                    {
                        this.direction = Direction.Left;
                    }
                }
                else if (ac.Type == ActionCardType.DrawTwo)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        playerToBeAffected.Deck.AddCard(this.drawPile.DrawCard());
                    }

                    if (firstRound == true)
                    {
                        this.playerWhoIsOnTurn = this.GetNextPlayer();
                    }
                    else
                    {
                        this.playerWhoIsOnTurn = playerToBeAffected;
                    }
                }
                else if (ac.Type == ActionCardType.Skip)
                {
                    if (firstRound == true)
                    {
                        this.playerWhoIsOnTurn = this.GetNextPlayer();
                    }
                    else
                    {
                        this.playerWhoIsOnTurn = playerToBeAffected;
                    }
                }
            }
        }

        private void RemoveCardAfterSet(Card card, Player player)
        {
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
                                player.Deck.RemoveCard(playerCard);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    player.Deck.RemoveCard(card);
                }
            }
            else if (card is NumericCard)
            {
                player.Deck.RemoveCard(card);
            }
        }

        private void CheckIfGameOver()
        {

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
            }
        }

        private void SendPlayerCards()
        {
            foreach (Player player in this.Players)
            {
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

        private Player GetNextPlayer()
        {
            Player player = null;

            if (this.direction == Direction.Right)
            {
                if (this.playerWhoIsOnTurn.PlayerID + 1 > this.Players.Count)
                {
                    player = this.Players[0];
                }
                else
                {
                    player = this.Players[this.playerWhoIsOnTurn.PlayerID];
                }
            }
            else if (this.direction == Direction.Left)
            {
                if (this.playerWhoIsOnTurn.PlayerID - 1 == 0)
                {
                    player = this.Players[this.Players.Count() - 1];
                }
                else
                {
                    player = this.Players[this.playerWhoIsOnTurn.PlayerID - 2];
                }
            }

            return player;
        }

        private void ChangePlayerTurn()
        {
            this.playerWhoIsOnTurn = this.GetNextPlayer();

            SendPlayerCards();
            SendRoundInformation();
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