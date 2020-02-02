using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GGJ2020.Game;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace GGJ2020
{
    public class TcpServer : TcpPeer
    {
        private System.Net.Sockets.TcpClient client;
        private Thread serverThread;
        private TcpListener tcpListener;
        private byte[] buffer;

        public int Port = 12345;
        
        // Start is called before the first frame update
        void OnEnable()
        {
            serverThread = new Thread(MasterListen);
            serverThread.IsBackground = true;
            serverThread.Start();
        }

        public override void SendPacket(object packet)
        {
            MasterWrite(NetworkUtility.ToNetwork(packet));
        }

        void MasterListen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, Port);
                tcpListener.Start();
                Debug.Log("Server is listening on port " + Port);

                client = tcpListener.AcceptTcpClient();
                var stream = client.GetStream();
                var streamReader = new StreamReader(stream);

                buffer = new byte[2048];

                while (true)
                {
                    string receivedString = streamReader.ReadLine();
                    Debug.Log("Received: " + receivedString);
                    var rec = NetworkUtility.FromNetwork(receivedString);

                    if (rec != null)
                    {
                        Run.OnMainThread(() => Tcp.OnReceivePacket(rec));
                    }
                    else
                    {
                        Debug.LogWarning("Packet deserialization didn't work: " + receivedString);
                    }
                }
            }
            catch (SocketException ex)
            {
                Debug.Log("Master Network error");
                Debug.LogException(ex);
            }
        }

        void MasterWrite(string message)
        {
            if (client == null)
            {
                Debug.Log("No Client connected.");
                return;
            }

            try
            {
                var stream = client.GetStream();
                if (!stream.CanWrite)
                {
                    Debug.Log("Master Cannot write to stream");
                    return;
                }

                Debug.Log("Sent: " + message);

                byte[] messageBytes = Encoding.UTF8.GetBytes(message + "\n");
                stream.Write(messageBytes, 0, messageBytes.Length);
                stream.Flush();
            }
            catch (SocketException)
            {
                Debug.Log("Master Sending Failed");
            }
        }
    }
}