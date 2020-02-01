using System.Collections;
using System.Collections.Generic;
using GGJ2020.Game;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Player myPlayer;
    
    private PlayerDto otherPlayerData;

    public Player MyPlayer => myPlayer;

    public PlayerDto OtherPlayerData
    {
        get => otherPlayerData;
        set => otherPlayerData = value;
    }
}
