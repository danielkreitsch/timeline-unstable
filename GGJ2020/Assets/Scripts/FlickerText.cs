using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlickerText : MonoBehaviour
{

    [SerializeField]
    float probabilityThreshhold;
    [SerializeField]
    TextMeshPro text;

    // Update is called once per frame
    void Update()
    {
        if (Random.Range(0, 1f) > probabilityThreshhold) {
            text.gameObject.SetActive(false);
        } else {
            text.gameObject.SetActive(true);
        }
    }
}
