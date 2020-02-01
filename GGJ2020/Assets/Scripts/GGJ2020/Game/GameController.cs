using System.Collections;
using System.Collections.Generic;
using GGJ2020.Game;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Game game;
    
    [SerializeField] private LayerMask slotLayer;

    [SerializeField] private LayerMask itemLayer;

    [SerializeField] private LayerMask boardLayer;

    /**
     * Wird noch ignoriert (noch nicht implementiert)
     */
    [SerializeField] private int slotCount;

    [SerializeField] private int itemCount;
    
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
        game.PrepareBoard(slotCount, itemCount);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            game.PrepareBoard(slotCount, itemCount);
        }
        
        if (game.State == State.TakeItem)
        {
            RaycastHit hit;
            if (RaycastUtils.RaycastMouse(out hit, itemLayer))
            {
                if (CursorItem == null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                            
                        CursorItem = hit.transform.gameObject.GetComponent<Item>();
                        foreach (Slot slot in game.GetAllSlots())
                        {
                            if (slot.Item == CursorItem)
                            {
                                slot.Item = null;
                            }
                        }
                        game.State = State.PlaceItem;
                    }
                }
            }
        }
        else if (game.State == State.PlaceItem)
        {
            RaycastHit hit;
            if (RaycastUtils.RaycastMouse(out hit, slotLayer))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (CursorItem != null)
                    {
                        Slot slot = hit.transform.gameObject.GetComponent<Slot>();
                        if (slot.IsEmpty())
                        {
                            slot.Item = CursorItem;
                            game.State = State.TakeItem;
                            CursorItem = null;
                        }
                    }
                }
            }
        }

        if (CursorItem != null)
        {
            RaycastHit hit;
            if (RaycastUtils.RaycastMouse(out hit, boardLayer))
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
}

public enum State
{
    AwaitReady,
    Countdown,
    TakeItem,
    PlaceItem
}