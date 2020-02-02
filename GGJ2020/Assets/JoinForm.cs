using System;
using System.Collections;
using System.Collections.Generic;
using GGJ2020.Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoinForm : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //gameObject.SetActive(false);
        }
    }

    public void OnEndEdit()
    {
        string ip = inputField.text;
        if (ip.Length > 0)
        {
            Tcp.MasterIp = ip;
            Tcp.StartTcp(TcpType.Client);
        }
    }

    public void OnEnable()
    {
        inputField.Select();
    }
}
