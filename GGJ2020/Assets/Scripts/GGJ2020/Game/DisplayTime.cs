using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DisplayTime : MonoBehaviour
{
    [SerializeField]
    Game game;
    [SerializeField]
    float initialSeconds = 20;
    [SerializeField]
    TextMeshPro text;
    TimeSpan timeSpan;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeSpan = TimeSpan.FromSeconds(initialSeconds - game.Timer);
        text.text = timeSpan.ToString(@"ss\:fff");
    }
}
