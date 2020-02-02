using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private const float smoothing = 20;
    private const float startY = 50;
    
    [SerializeField] private int id;
    
    private Vector3 targetPos;
    
    public Vector3 TargetPos
    {
        get => targetPos;
        set => targetPos = value;
    }

    public int Id => id;

    public void PlaySpawnAnimation()
    {
        targetPos = transform.position;
        transform.position = transform.position + new Vector3(0, startY, 0);
    }
    
    private void Update()
    {
        transform.position = transform.position + (targetPos - transform.position) * Time.deltaTime * smoothing;
    }
}
