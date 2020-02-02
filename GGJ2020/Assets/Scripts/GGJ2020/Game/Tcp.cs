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

        private void Awake()
        {
            instance = this;
        }

        public void StartTcp(TcpType type)
        {
            this.type = type;
            if (type == TcpType.Server)
            {
                GameObject obj = Instantiate(serverPrefab);
                peer = obj.GetComponent<TcpServer>();
            }
            else if (type == TcpType.Client)
            {
                GameObject obj = Instantiate(clientPrefab);
                peer = obj.GetComponent<TcpClient>();
            }
        }
    }

    public enum TcpType
    {
        Server,
        Client
    }
}