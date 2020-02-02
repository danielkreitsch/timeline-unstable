using System;
using System.Collections.Generic;
using GGJ2020.Game;

namespace GGJ2020
{
    [Serializable]
    public class StartGamePacket
    {
        public string name;
        public BoardDto board;
        public List<int> itemIds;
    }
}