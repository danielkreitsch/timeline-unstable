using System.Collections;
using System.Collections.Generic;
using GGJ2020.Game;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Game game;
    
    void Start()
    {
        
        //game.MyPlayer.Board.GenerateSlots(5);
    }

    void Update()
    {
        
    }
    
    public void OnOtherPlayerDataReceive(PlayerDto otherPlayerData)
    {
        game.OtherPlayerData = otherPlayerData;
        
        // Check if the board data is same
    }
}
