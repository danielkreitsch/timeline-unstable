using System;
using System.Collections.Generic;
using GGJ2020.Game;

namespace GGJ2020
{
    [Serializable]
    public class EndGamePacket
    {
        public bool won;

        public EndGamePacket(bool won)
        {
            this.won = won;
        }
    }
}