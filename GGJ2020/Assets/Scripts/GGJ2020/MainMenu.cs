using System.Collections;
using System.Collections.Generic;
using GGJ2020.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnHostClick()
    {
        Debug.Log("Host");
        
        Tcp.StartTcp(TcpType.Server);
        
        SceneManager.LoadScene("game");
    }

    public void OnJoinClick()
    {
        Debug.Log("Join");
        
        Tcp.StartTcp(TcpType.Client);
        
        SceneManager.LoadScene("game");
    }

    public void OnCreditsClick()
    {
        Debug.Log("Credits");
        SceneManager.LoadScene("credits");
    }

    public void OnTutorialClick()
    {
        Debug.Log("Tutorial");
        SceneManager.LoadScene("tutorial");
    }

    public void OnExitClick()
    {
        Application.Quit();
    }
}
