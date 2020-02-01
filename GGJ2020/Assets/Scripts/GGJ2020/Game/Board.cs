using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;

    private List<Slot> slots;

    // Noch nicht fertig
    public void GenerateSlots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float x = 0;
            float z = 0;
            slots = new List<Slot>();
            GameObject slotObj = Instantiate(slotPrefab, new Vector3(x, 0, z), Quaternion.identity);
            Slot slot = slotObj.GetComponent<Slot>();
            slots.Add(slot);
        }
    }
}