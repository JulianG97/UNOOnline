using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class ProtocolTypes
    {
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