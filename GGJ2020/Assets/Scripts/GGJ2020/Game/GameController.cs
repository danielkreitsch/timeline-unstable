using System;
using System.Collections;
using System.Collections.Generic;
using GGJ2020;
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
    private Vector3 cursorDisplacement;

    private Slot oldSlot;

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
        if (game.Started)
        {
            if (game.Timer >= 20)
            {
                game.Timer = 20;
            }
            else
            {
                game.Timer += Time.deltaTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            game.PrepareBoard(slotCount, itemCount);
        }

        if (game.State == State.TakeItem)
        {
            if (CursorItem != null)
            {
                throw new Exception("The cursor has an item but the game state is TakeItem");
            }

            RaycastHit hit;
            if (RaycastUtils.RaycastMouse(out hit, itemLayer))
            {
                hoveringItem = hit.transform.gameObject.GetComponent<Item>();
            }
            else
            {
                hoveringItem = null;
            }

            if (hoveringItem != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    cursorDisplacement = new Vector3(hit.transform.position.x - hit.point.x, 0, hit.transform.position.z - hit.point.z);
                    CursorItem = hit.transform.gameObject.GetComponent<Item>();
                    foreach (Slot slot in game.GetAllSlots())
                    {
                        if (slot.Item == CursorItem)
                        {
                            oldSlot = slot;
                            slot.Item = null;
                        }
                    }
                    game.State = State.PlaceItem;
                }
            }
        }
        else if (game.State == State.PlaceItem)
        {
            if (CursorItem == null)
            {
                throw new Exception("The cursor has no item but the game state is PlaceItem");
            }

            RaycastHit hit;
            if (RaycastUtils.RaycastMouse(out hit, slotLayer))
            {
                Slot slot = hit.transform.gameObject.GetComponent<Slot>();
                hoveringSlot = slot;
            }
            else
            {
                hoveringSlot = null;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (hoveringSlot != null && hoveringSlot.IsEmpty())
                {
                    game.PlaceItem(hoveringSlot, CursorItem);
                    CursorItem = null;
                }
                else
                {
                    game.PlaceItem(oldSlot, CursorItem);
                    CursorItem = null;
                }
            }
        }

        if (CursorItem != null)
        {
            RaycastHit hit;
            if (RaycastUtils.RaycastMouse(out hit, boardLayer))
            {
                Vector3 position = hit.point + cursorDisplacement;
                CursorItem.TargetPos = position;
            }
        }
    }

    public void OnReceivePacket(object packet)
    {
        Debug.Log("Received packet in game controller");
        if (packet is PlayerDto)
        {
            OnOtherPlayerDataReceive((PlayerDto) packet);
        }
    }

    public void OnOtherPlayerDataReceive(PlayerDto otherPlayerData)
    {
        Debug.Log("Received player data");
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