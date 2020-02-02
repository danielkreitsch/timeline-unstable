using System.Collections;
using System.Collections.Generic;
using GGJ2020;
using UnityEngine;

public class TcpSendTest : MonoBehaviour
{
    public TcpClient client;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SendTest();
        }
    }

    public void SendTest()
    {
        client.SendData = true;
    }
}
