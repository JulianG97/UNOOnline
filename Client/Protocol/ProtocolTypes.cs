using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public class ProtocolTypes
    {
        // CreateGame = CG
        public static byte[] CreateGame = new byte[] { 67, 71 };

        // RequestRooms = RR
        public static byte[] RequestRooms = new byte[] { 82, 82 };

        // JoinGame = JG
        public static byte[] JoinGame = new byte[] { 74, 71 };

        // SetCard = SC
        public static byte[] SetCard = new byte[] { 83, 67 };

        // RoomList = RL
        public static byte[] RoomList = new byte[] { 82, 76 };

        // OK = OK
        public static byte[] OK = new byte[] { 79, 75 };

        // Invalid = IN
        public static byte[] Invalid = new byte[] { 73, 78 };

        // GameStart = GS
        public static byte[] GameStart = new byte[] { 71, 83 };

        // IsAlive = IA
        public static byte[] IsAlive = new byte[] { 73, 65 };

        // RoundInformation = RI
        public static byte[] RoundInformation = new byte[] { 82, 73 };

        // PlayerCards = PC
        public static byte[] PlayerCards = new byte[] { 80, 67 };

        // GameOver = GO
        public static byte[] GameOver = new byte[] { 71, 79 };
    }
}