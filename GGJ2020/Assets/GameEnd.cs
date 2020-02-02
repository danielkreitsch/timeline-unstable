using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    /**
     * Testklasse
     */
    
    public void OnWon()
    {
        Debug.Log("Gewonnen! :)");
    }

    public void OnLost()
    {
        Debug.Log("Verloren :(");
    }
}
