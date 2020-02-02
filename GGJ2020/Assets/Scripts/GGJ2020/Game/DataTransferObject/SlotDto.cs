using System;
using UnityEngine;

namespace GGJ2020.Game
{
    [Serializable]
    public class SlotDto
    {
        public int id;
        public float x;
        public float y;
        public ItemDto item;
    }
}