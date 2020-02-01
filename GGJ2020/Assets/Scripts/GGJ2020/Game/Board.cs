using System.Collections;
using System.Collections.Generic;
using GGJ2020.Game;
using UnityEditorInternal;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    
    private List<Slot> slots;

    public void GenerateSlots()
    {
        slots = new List<Slot>();
        for (int i = 0; i < 5; i++)
        {
            float x = Random.Range(0, 50);
            float z = Random.Range(0, 50);
            Debug.Log("spawn at " + x + ", " + z);
            GameObject slotObj = Instantiate(slotPrefab, new Vector3(x, 0, z), Quaternion.identity);
            Slot slot = slotObj.GetComponent<Slot>();
            slots.Add(slot);
        }
    }
}
