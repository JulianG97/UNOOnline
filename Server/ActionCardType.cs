using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public enum ActionCardType
    {
        DrawTwo = 'T',
        Reverse = 'R',
        Skip = 'S',
        Wild = 'C',
        WildDrawFour = 'F'
    }
}