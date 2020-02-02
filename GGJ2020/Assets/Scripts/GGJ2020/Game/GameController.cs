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

    [SerializeField] private bool autoWin;

    [SerializeField] private Game game;
    
    [SerializeField] private LayerMask slotLayer;

    [SerializeField] private LayerMask itemLayer;

    [SerializeField] private LayerMask boardLayer;
    
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
            game.PrepareBoard();
            game.StartGame(game.GetRandomItemIds(itemCount));
        }
        else
        {
            if (Tcp.Type == TcpType.Client)
            {
                List<int> itemIds = game.GetRandomItemIds(itemCount);
                
                game.PrepareBoard();
                game.StartGame(itemIds);
                
                Tcp.Peer.SendPacket(game.MyPlayer.CreateStartGamePacket(itemIds));
            }
        }
    }

    void Update()
    {
        if (game.Running)
        {
            if (game.Timer >= 20)
            {
                game.Timer = 20;
                game.EndGame(autoWin);
            }
            else
            {
                game.Timer += Time.deltaTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
                    if (!offlineMode)
                    {
                        SendItemsData();
                    }
                }
                else
                {
                    game.PlaceItem(oldSlot, CursorItem);
                    CursorItem = null;
                    if (!offlineMode)
                    {
                        SendItemsData();
                    }
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

    public void OnStartGamePacketReceive(StartGamePacket packet)
    {
        foreach (SlotDto slotDto in packet.board.slots)
        {
            game.MyPlayer.Board.AddSlotFromDto(slotDto);
        }

        game.StartGame(packet.itemIds);
    }

    public void OnRestartGamePacketReceive(RestartGamePacket packet)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnItemsDataPacketReceive(ItemsDataPacket packet)
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
            Tcp.Peer.SendPacket(new EndGamePacket(won));
        }
        else
        {
            Debug.Log("NOT EQUAL");
        }
    }

    public void OnEndGamePacketReceive(EndGamePacket packet)
    {
        game.EndGame(packet.won);
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
        Tcp.Peer.SendPacket(packet);
    }
}

public enum State
{
    AwaitReady,
    Countdown,
    TakeItem,
    PlaceItem
}