using System;
using UnityEngine;

namespace GGJ2020.Game
{
    public class Tcp : MonoBehaviour
    {
        private static Tcp instance;

        [SerializeField] private GameObject serverPrefab;
        [SerializeField] private GameObject clientPrefab;

        private TcpPeer peer;
        private TcpType type;

        public static TcpPeer Peer
        {
            get { return instance.peer; }
        }

        public static TcpType Type
        {
            get { return instance.type; }
        }

        private void Awake()
        {
            instance = this;
        }

        public static void StartTcp(TcpType type)
        {
            instance.type = type;
            if (type == TcpType.Server)
            {
                GameObject obj = Instantiate(instance.serverPrefab);
                TcpServer peer = obj.GetComponent<TcpServer>();
                instance.peer = peer;
            }
            else if (type == TcpType.Client)
            {
                GameObject obj = Instantiate(instance.clientPrefab);
                TcpClient peer = obj.GetComponent<TcpClient>();
                instance.peer = peer;
            }
        }
        
        public static void OnReceivePacket(object packet)
        {
            //Debug.Log("Received packet in game controller");
            if (packet is StartGamePacket)
            {
                GameController gameController = FindObjectOfType<GameController>();
                gameController.OnStartGamePacketReceive((StartGamePacket) packet);
            }
            else if (packet is ItemsDataPacket)
            {
                GameController gameController = FindObjectOfType<GameController>();
                gameController.OnItemsDataPacketReceive((ItemsDataPacket) packet);
            }
            else if (packet is EndGamePacket)
            {
                GameController gameController = FindObjectOfType<GameController>();
                gameController.OnEndGamePacketReceive((EndGamePacket) packet);
            }
        }
    }

    public enum TcpType
    {
        Server,
        Client
    }
}