using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;

    private List<Slot> slots;

    public void GenerateSlots(int count)
    {
        slots = new List<Slot>();
        int tries = 0;
        while (slots.Count < count || tries < 1000)
        {
            float x = Random.Range(-12, 13);
            float z = Random.Range(-7, 8);

            GameObject slotObj = Instantiate(slotPrefab, new Vector3(x, 0, z), Quaternion.identity);

            Collider[] colliders = Physics.OverlapBox(slotObj.transform.position, slotObj.transform.localScale);
            bool blocking = false;
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject != slotObj && !collider.gameObject.CompareTag("DepthMask"))
                {
                    blocking = true;
                    Debug.Log(collider.gameObject.name);
                    break;
                }
            }

            if (blocking)
            {
                Destroy(slotObj);
                tries++;
                continue;
            }

            Slot slot = slotObj.GetComponent<Slot>();
            slots.Add(slot);
        }
    }
}