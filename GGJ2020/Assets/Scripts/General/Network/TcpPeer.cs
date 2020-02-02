using UnityEngine;

namespace GGJ2020
{
    public abstract class TcpPeer : MonoBehaviour
    {
        public abstract void SendPacket(object obj);
    }
}