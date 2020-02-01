using System.Collections;
using System.Collections.Generic;
using GGJ2020.Game;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Game game;

    [SerializeField] private GameObject itemPrefab;
    
    [SerializeField] private LayerMask slotLayer;

    [SerializeField] private LayerMask itemLayer;

    [SerializeField] private LayerMask boardLayer;

    private State state = State.TakeItem;
    private Slot hoveringSlot;
    private Item hoveringItem;
    private Item _cursorItem;

    public Item CursorItem
    {
        get => _cursorItem;
        set { _cursorItem = value; }
    }

    void Start()
    {
        PrepareBoard();
        //game.MyPlayer.Board.GenerateSlots(5);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PrepareBoard();
        }
        
        if (state == State.TakeItem)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, itemLayer))
            {
                if (CursorItem == null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        CursorItem = hit.transform.gameObject.GetComponent<Item>();
                        foreach (Slot slot in getAllSlots())
                        {
                            if (slot.Item == CursorItem)
                            {
                                slot.Item = null;
                            }
                        }
                        state = State.PlaceItem;
                        //print("Item: " + hit.collider.name);
                    }
                }
            }
        }
        else if (state == State.PlaceItem)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, slotLayer))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (CursorItem != null)
                    {
                        Slot slot = hit.transform.gameObject.GetComponent<Slot>();
                        if (slot.Item == null)
                        {
                            slot.Item = CursorItem;
                            CursorItem.transform.position = slot.transform.position;
                            CursorItem = null;
                            state = State.TakeItem;
                        }
                    }
                }
            }
        }

        if (CursorItem != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, boardLayer))
            {
                Vector3 position = hit.point;
                CursorItem.transform.position = position;
            }
        }
    }

    public void OnOtherPlayerDataReceive(PlayerDto otherPlayerData)
    {
        game.OtherPlayerData = otherPlayerData;

        // Check if the board data is same
    }

    private void PrepareBoard()
    {
        foreach (Slot slot in getAllSlots())
        {
            if (slot.Item != null)
            {
                Destroy(slot.Item.gameObject);
                slot.Item = null;
            }
        }
        
        List<Slot> randomSlots = getRandomSlots(3);

        foreach (Slot slot in randomSlots)
        {
            GameObject itemObj = Instantiate(itemPrefab);
            slot.Item = itemObj.GetComponent<Item>();
        }

        state = State.TakeItem;
    }

    private List<Slot> getRandomSlots(int count)
    {
        List<Slot> randomSlots = new List<Slot>();
        List<Slot> allSlots = getAllSlots();
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

    private List<Slot> getAllSlots()
    {
        List<Slot> slots = new List<Slot>();
        foreach (GameObject slotObj in GameObject.FindGameObjectsWithTag("Slot"))
        {
            Slot slot = slotObj.GetComponent<Slot>();
            slots.Add(slot);
        }
        return slots;
    }
}

public enum State
{
    AwaitReady,
    Countdown,
    TakeItem,
    PlaceItem
}