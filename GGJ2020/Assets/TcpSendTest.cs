using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TcpSendTest : MonoBehaviour
{
    public TcpClientHandler client;

    // Update is called once per frame
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
