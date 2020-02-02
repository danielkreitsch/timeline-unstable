using System;
using System.Collections;
using System.Collections.Generic;
using GGJ2020;
using GGJ2020.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private bool offlineMode = false;
    
    [SerializeField] private Game game;

    [SerializeField] private TcpPeer tcpPeer;

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
        if (offlineMode)
        {
            game.PrepareBoard(slotCount);
            game.StartGame(itemCount);
        }
        else
        {
            if (tcpPeer is TcpClientHandler)
            {
                game.PrepareBoard(slotCount);
                SendBoardData();
            }
        }
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //game.PrepareBoard(slotCount, itemCount);
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
                    SendItemsData();
                }
                else
                {
                    game.PlaceItem(oldSlot, CursorItem);
                    CursorItem = null;
                    SendItemsData();
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
        //Debug.Log("Received packet in game controller");
        if (packet is StartGamePacket)
        {
            OnOtherPlayerDataReceive((StartGamePacket) packet);
        }
        else if (packet is ItemsDataPacket)
        {
            OnItemsDataReceive((ItemsDataPacket) packet);
        }
    }

    public void OnOtherPlayerDataReceive(StartGamePacket otherPlayerData)
    {
        game.OtherPlayerData = otherPlayerData;

        foreach (SlotDto slotDto in otherPlayerData.board.slots)
        {
            game.MyPlayer.Board.AddSlotFromDto(slotDto);
        }
        
        game.StartGame(itemCount);

        Debug.Log("Player data received (" + game.OtherPlayerData.board.slots.Count + " slots)");
        
        // Check if the board data is same
        
    }

    public void OnItemsDataReceive(ItemsDataPacket packet)
    {
        bool equal = true;
        foreach (Slot slot in game.MyPlayer.Board.Slots)
        {
            if (slot.Item != null)
            {
                bool found = false;
                foreach (ItemDto itemDto in packet.items)
                {
                    if (itemDto.id == slot.Item.Id && itemDto.slotId == slot.Id)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    equal = false;
                    break;
                }
            }
        }

        if (equal)
        {
            bool won = true;
            game.EndGame(won);
            tcpPeer.SendPacket(new EndGamePacket(won));
            Debug.Log("EQUAL! WON!");
        }
        else
        {
            Debug.Log("NOT EQUAL");
        }
    }

    public void SendBoardData()
    {
        Debug.Log("Send my board data");
        tcpPeer.SendPacket(game.MyPlayer.ToDto());
        game.StartGame(itemCount);
    }

    public void SendItemsData()
    {
        ItemsDataPacket packet = new ItemsDataPacket();
        foreach (Slot slot in game.MyPlayer.Board.Slots)
        {
            if (slot.Item != null)
            {
                ItemDto itemDto = new ItemDto();
                itemDto.id = slot.Item.Id;
                itemDto.slotId = slot.Id;
                packet.items.Add(itemDto);
            }
        }
        tcpPeer.SendPacket(packet);
    }
}

public enum State
{
    AwaitReady,
    Countdown,
    TakeItem,
    PlaceItem
}