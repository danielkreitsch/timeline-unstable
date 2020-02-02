using System;
using UnityEngine;

namespace GGJ2020.Game
{
    public class Tcp : MonoBehaviour
    {
        private static Tcp instance;

        [SerializeField] private GameObject serverPrefab;
        [SerializeField] private GameObject clientPrefab;

        private string masterIp;
        private TcpPeer peer;
        private TcpType type = TcpType.None;

        public static string MasterIp
        {
            get => instance.masterIp;
            set => instance.masterIp = value;
        }

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
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static void StartTcp(TcpType type)
        {
            instance.type = type;
            if (type == TcpType.Server)
            {
                GameObject obj = Instantiate(instance.serverPrefab);
                TcpServer peer = obj.GetComponent<TcpServer>();
                instance.peer = peer;
                peer.Init();
            }
            else if (type == TcpType.Client)
            {
                GameObject obj = Instantiate(instance.clientPrefab);
                TcpClient peer = obj.GetComponent<TcpClient>();
                peer.MasterIp = MasterIp;
                instance.peer = peer;
                peer.Connect();
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
            else if (packet is RestartGamePacket)
            {
                GameController gameController = FindObjectOfType<GameController>();
                gameController.OnRestartGamePacketReceive((RestartGamePacket) packet);
            }
        }
    }

    public enum TcpType
    {
        Server,
        Client,
        None
    }
}