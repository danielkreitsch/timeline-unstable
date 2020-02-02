using System.Collections.Generic;
using GGJ2020.Game;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;

    private List<Slot> slots = new List<Slot>();

    public List<Slot> Slots => slots;

    public void Clear()
    {
        foreach (Slot slot in slots)
        {
            if (slot.Item != null)
            {
                Destroy(slot.Item.gameObject);
            }
            Destroy(slot.gameObject);
        }

        slots.Clear();
    }

    public void GenerateSlots(GameObject slotsPrefab)
    {
        Instantiate(slotsPrefab);
        foreach (Slot slot in FindObjectsOfType<Slot>())
        {
            slots.Add(slot);
            Debug.Log("Slot added");
        }
        /*int loopCount = 0;
        while (slots.Count < count && loopCount < 1000)
        {
            loopCount++;
            float x = Random.Range(-12f, 13f);
            float z = Random.Range(-7f, 8f);
            GameObject slotObj = Instantiate(slotPrefab, new Vector3(x, 1, z), Quaternion.identity);

            Collider[] colliders = Physics.OverlapBox(slotObj.transform.position, 0.3f * slotObj.GetComponent<BoxCollider>().size);
            bool blocking = false;
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject != slotObj && !collider.gameObject.CompareTag("DepthMask"))
                {
                    blocking = true;
                    //Debug.Log(collider.gameObject.name);
                    break;
                }
            }
            
            if (blocking)
            {
                Destroy(slotObj);
            }
            else
            {
                Slot slot = slotObj.GetComponent<Slot>();
                slots.Add(slot);
            }
        }*/
    }

    public void AddSlotFromDto(SlotDto dto)
    {
        GameObject slotObj = Instantiate(slotPrefab);
        Slot slot = slotObj.GetComponent<Slot>();
        slot.Id = dto.id;
        slot.transform.position = new Vector3(dto.x, slot.transform.position.y, dto.y);
        slots.Add(slot);
    }
}