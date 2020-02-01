using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private const float smoothing = 30;
    private const float startY = 50;
    
    private string id;
    
    public Vector3 targetPos;

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
