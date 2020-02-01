using System.Collections;
using System.Collections.Generic;
using GGJ2020;
using GGJ2020.Game;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Player myPlayer;

    [SerializeField] private GameObject[] itemPrefabs;

    private PlayerDto otherPlayerData;

    private State state;

    private float timer;

    private bool started;

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

    public float Timer
    {
        get => timer;
        set => timer = value;
    }

    public bool Started => started;
    
    public void PrepareBoard(int slots, int items)
    {
        StartCoroutine(CPrepareBoard(slots, items));
    }

    IEnumerator CPrepareBoard(int slots, int items)
    {
        timer = 0;
        started = false;
        
        //myPlayer.Board.GenerateSlots(slots);
        
        foreach (Slot slot in GetAllSlots())
        {
            if (slot.Item != null)
            {
                Destroy(slot.Item.gameObject);
                slot.Item = null;
            }
        }

        List<Slot> randomSlots = GetRandomSlots(items);
        List<GameObject> randomItemPrefabs = GetRandomItemPrefabs(items);

        for (int i = 0; i < items; i++)
        {
            yield return new WaitForSeconds(0.25f);
            Slot slot = randomSlots[i];
            GameObject itemPrefab = randomItemPrefabs[i];
            GameObject itemObj = Instantiate(itemPrefab);
            itemObj.transform.position = slot.transform.position;
            slot.Item = itemObj.GetComponent<Item>();
            slot.Item.PlaySpawnAnimation();
        }

        started = true;
        state = State.TakeItem;
    }

    public void PlaceItem(Slot slot, Item item)
    {
        if (slot.IsEmpty())
        {
            slot.Item = item;
            state = State.TakeItem;
        }
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

    public List<GameObject> GetRandomItemPrefabs(int count)
    {
        List<GameObject> randomPrefabs = new List<GameObject>();
        while (randomPrefabs.Count < count)
        {
            GameObject randomPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
            if (!randomPrefabs.Contains(randomPrefab))
            {
                randomPrefabs.Add(randomPrefab);
            }
        }
        return randomPrefabs;
    }
}