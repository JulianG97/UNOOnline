using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Deck
    {
        public event EventHandler<EventArgs> DeckIsEmpty;

        public List<Card> Cards
        {
            get;
            set;
        }

        public void AddCard(Card newCard)
        {
            List<Card> newCards = new List<Card>();
            newCards.Add(newCard);

            foreach (Card card in this.Cards)
            {
                newCards.Add(card);
            }

            this.Cards = newCards;
        }

        public void RemoveCard(Card cardToBeRemoved)
        {
            int position = GetPositionOfCard(cardToBeRemoved);
            this.Cards.RemoveAt(position);
        }

        public int GetPositionOfCard(Card cardToGetPositionOf)
        {
            int position = 0;

            foreach (Card card in this.Cards)
            {
                if (card.Color == cardToGetPositionOf.Color)
                {
                    if (cardToGetPositionOf is ActionCard && card is ActionCard)
                    {
                        ActionCard ctc = (ActionCard)cardToGetPositionOf;
                        ActionCard c = (ActionCard)card;

                        if (ctc.Type == c.Type)
                        {
                            break;
                        }
                    }
                    else if (cardToGetPositionOf is NumericCard && card is NumericCard)
                    {
                        NumericCard ctc = (NumericCard)cardToGetPositionOf;
                        NumericCard c = (NumericCard)card;

                        if (ctc.Number == c.Number)
                        {
                            break;
                        }
                    }
                }

                position++;
            }

            return position;
        }

        public Card DrawCard()
        {
            if (this.Cards.Count == 0)
            {
                this.FireOnDeckIsEmpty();

                while (this.Cards.Count == 0)
                { }
            }

            Card card = this.Cards[0];
            this.Cards.RemoveAt(0);
            return card;
        }

        protected virtual void FireOnDeckIsEmpty()
        {
            if (this.DeckIsEmpty != null)
            {
                this.DeckIsEmpty(this, new EventArgs());
            }
        }

        public void Mix()
        {
            Random random = new Random();

            for (int i = 0; i < 1000; i++)
            {
                int r1 = random.Next(0, this.Cards.Count);

                int r2 = random.Next(0, this.Cards.Count);

                Card temp = this.Cards[r1];
                this.Cards[r1] = this.Cards[r2];
                this.Cards[r2] = temp;
            }
        }
    }
}