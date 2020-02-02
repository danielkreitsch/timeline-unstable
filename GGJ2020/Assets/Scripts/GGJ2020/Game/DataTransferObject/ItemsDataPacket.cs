using System;
using System.Collections.Generic;
using GGJ2020.Game;

namespace GGJ2020
{
    [Serializable]
    public class ItemsDataPacket
    {
        public List<ItemDto> items = new List<ItemDto>();
    }
}