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

        // JoinGame = JG
        public static byte[] JoinGame = new byte[] { 74, 71 };

        // IsAlive = IA
        public static byte[] IsAlive = new byte[] { 73, 65 };

        // SetCard = SC
        public static byte[] SetCard = new byte[] { 83, 67 };
    }
}