using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] private Item item;

    private int id;

    private static int nextId = 1;

    public int Id
    {
        get => id;
        set => id = value;
    }

    public Item Item
    {
        get => item;
        set
        {
            item = value;
            if (item != null)
            {
                item.TargetPos = transform.position;
            }
        }
    }

    void Awake()
    {
        id = nextId++;
    }

    public bool IsEmpty()
    {
        return item == null;
    }
}
