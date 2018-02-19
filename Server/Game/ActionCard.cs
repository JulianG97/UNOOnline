using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class ActionCard : Card
    {
        public ActionCard(Color color, ActionCardType type)
        {
            this.Color = color;
            this.Type = type;
        }

        public ActionCardType Type
        {
            get;
            set;
        }
    }
}