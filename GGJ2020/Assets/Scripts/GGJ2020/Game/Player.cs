using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020.Game
{
    public class Player : MonoBehaviour
    {
        private string name = "Peter";
        [SerializeField] private Board board;

        public void Init(string name)
        {
            this.name = name;
        }

        public string Name => name;

        public Board Board => board;

        public StartGamePacket CreateStartGamePacket()
        {
            StartGamePacket startGamePacket = new StartGamePacket();

            startGamePacket.name = name;
            startGamePacket.board = new BoardDto();
            startGamePacket.board.slots = new List<SlotDto>();
            foreach (Slot slot in board.Slots)
            {
                SlotDto slotDto = new SlotDto();
                slotDto.id = slot.Id;
                slotDto.x = slot.transform.position.x;
                slotDto.y = slot.transform.position.z;
                if (slot.Item != null)
                {
                    slotDto.item = new ItemDto();
                    slotDto.item.id = slot.Item.Id;
                    slotDto.item.slotId = slot.Id;
                }
                startGamePacket.board.slots.Add(slotDto);
            }
            
            startGamePacket.itemIds = new List<int>();

            return startGamePacket;
        }
    }
}