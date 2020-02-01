using System.Collections;
using System.Collections.Generic;
using GGJ2020.Game;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Player myPlayer;
    
    [SerializeField] private GameObject itemPrefab;

    private PlayerDto otherPlayerData;

    private State state;

    public Player MyPlayer => myPlayer;

    public PlayerDto OtherPlayerData
    {
        get => otherPlayerData;
        set => otherPlayerData = value;
    }

    public State State
    {
        get => state;
        set => state = value;
    }
    
    /**
     * Die Variable slots wird noch ignoriert
     */
    public void PrepareBoard(int slots, int items)
    {
        foreach (Slot slot in GetAllSlots())
        {
            if (slot.Item != null)
            {
                Destroy(slot.Item.gameObject);
                slot.Item = null;
            }
        }
        
        List<Slot> randomSlots = GetRandomSlots(items);

        foreach (Slot slot in randomSlots)
        {
            GameObject itemObj = Instantiate(itemPrefab);
            slot.Item = itemObj.GetComponent<Item>();
        }

        state = State.TakeItem;
    }

    public void PlaceItem(Slot slot, Item item)
    {
        
    }
    
    public List<Slot> GetAllSlots()
    {
        List<Slot> slots = new List<Slot>();
        foreach (GameObject slotObj in GameObject.FindGameObjectsWithTag("Slot"))
        {
            Slot slot = slotObj.GetComponent<Slot>();
            slots.Add(slot);
        }
        return slots;
    }
        
    public List<Slot> GetRandomSlots(int count)
    {
        List<Slot> randomSlots = new List<Slot>();
        List<Slot> allSlots = GetAllSlots();
        while (randomSlots.Count < count)
        {
            Slot randomSlot = allSlots[Random.Range(0, allSlots.Count)];
            if (!randomSlots.Contains(randomSlot))
            {
                randomSlots.Add(randomSlot);
            }
        }
        return randomSlots;
    }
}
