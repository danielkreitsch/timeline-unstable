using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] private Item item;

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

    public bool IsEmpty()
    {
        return item == null;
    }
}
