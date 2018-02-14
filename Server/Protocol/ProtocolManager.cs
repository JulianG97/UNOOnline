using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ProtocolManager
    {
        public static Protocol RoomList(List<Game> gameList)
        {
            string roomList = string.Empty;

            foreach (Game game in gameList)
            {
                if (game.JoinedPlayers < game.PlayersNeeded)
                {
                    roomList += game.GameID + "-" + game.JoinedPlayers + "-" + game.PlayersNeeded;

                    if (game != gameList.Last())
                    {
                        roomList += "-";
                    }

                }
            }

            Protocol protocol = new Protocol(ProtocolTypes.RoomList, Encoding.ASCII.GetBytes(roomList));
            return protocol;
        }

        public static Protocol OK()
        {
            Protocol protocol = new Protocol(ProtocolTypes.OK, new byte[0]);
            return protocol;
        }

        public static Protocol Invalid()
        {
            Protocol protocol = new Protocol(ProtocolTypes.Invalid, new byte[0]);
            return protocol;
        }

        public static Protocol GameStart(string playerID)
        {
            Protocol protocol = new Protocol(ProtocolTypes.GameStart, Encoding.ASCII.GetBytes(playerID));
            return protocol;
        }

        public static Protocol IsAlive()
        {
            Protocol protocol = new Protocol(ProtocolTypes.IsAlive, new byte[0]);
            return protocol;
        }

        public static Protocol RoundInformation(Card lastCard, Player playerWhoIsOnTurn, List<Player> playerList)
        {
            char value = '\n';

            if (lastCard is ActionCard)
            {
                ActionCard actionCard = (ActionCard)lastCard;

                value = (char)actionCard.Type;
            }
            else if (lastCard is NumericCard)
            {
                NumericCard numericCard = (NumericCard)lastCard;

                value = (char)numericCard.Number;
            }

            string amountOfPlayerCards = string.Empty;

            foreach (Player player in playerList)
            {
                amountOfPlayerCards += player.Deck.Cards.Count;

                if (player != playerList.Last())
                {
                    amountOfPlayerCards += "-";
                }
            }

            Protocol protocol = new Protocol(ProtocolTypes.RoundInformation, Encoding.ASCII.GetBytes((char)lastCard.Color + "-" + value + "-" + playerWhoIsOnTurn.PlayerID + "-" + amountOfPlayerCards));
            return protocol;
        }
    }
}
