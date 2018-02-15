using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Player
    {
        public Player(int playerID, NetworkManager networkManager)
        {
            this.PlayerID = playerID;
            this.NetworkManager = networkManager;
        }

        public int PlayerID
        {
            get;
            set;
        }

        public Deck Deck
        {
            get;
            set;
        }

        public NetworkManager NetworkManager
        {
            get;
            set;
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