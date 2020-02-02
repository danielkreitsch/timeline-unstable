using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuOption : MonoBehaviour
{
    public UnityEvent onSelect;

    public Vector3 targetPos;
    
    private float smoothing = 20;

    private void Awake()
    {
        targetPos = transform.position;
    }
    
    private void Update()
    {
        transform.position = transform.position + (targetPos - transform.position) * Mathf.Min(1, Time.deltaTime * smoothing);
    }
}
