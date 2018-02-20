using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ProtocolManager
    {
        public static Protocol CreateGame(string amountOfPlayers)
        {
            Protocol protocol = new Protocol(ProtocolTypes.CreateGame, Encoding.ASCII.GetBytes(amountOfPlayers));
            return protocol;
        }

        public static Protocol RequestRooms()
        {
            Protocol protocol = new Protocol(ProtocolTypes.RequestRooms, new byte[0]);
            return protocol;
        }

        public static Protocol JoinGame(string gameID)
        {
            Protocol protocol = new Protocol(ProtocolTypes.JoinGame, Encoding.ASCII.GetBytes(gameID));
            return protocol;
        }

        public static Protocol IsAlive()
        {
            Protocol protocol = new Protocol(ProtocolTypes.IsAlive, new byte[0]);
            return protocol;
        }

        public static Protocol SetCard(string gameID, string playerID, string cardColor, string cardValue, string unoYesOrNo)
        {
            Protocol protocol = new Protocol(ProtocolTypes.SetCard, Encoding.ASCII.GetBytes(gameID + "-" + playerID + "-" + cardColor + "-" + cardValue + "-" + unoYesOrNo));
            return protocol;
        }
    }
}
