using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GGJ2020.Game;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GGJ2020
{
	public class TcpClientHandler : TcpPeer
	{
		private Thread clientThread;
		private TcpClient master;
		private byte[] buffer;

		public string MasterIp = "127.0.0.1";
		public int Port = 12345;

		public bool SendData = false;

		[SerializeField] private GameController gameController;

		// Start is called before the first frame update
		void OnEnable()
		{
			try
			{
				clientThread = new Thread(new ThreadStart(ClientListen));
				clientThread.IsBackground = true;
				clientThread.Start();
			}
			catch (SocketException e)
			{
				Debug.Log("On client connect exception " + e);
			}
		}

		// Update is called once per frame
		void Update()
		{
			if (SendData)
			{
				SendData = false;
				var packet = new ReadyPacket();
				packet.message = "hallo welt";
				SendPacket(packet);
			}
		}

		public override void SendPacket(object packet)
		{
			ClientWrite(NetworkUtility.ToNetwork(packet));
		}

		public void SetHostname(string NewText)
		{
			MasterIp = NewText;
		}

		public void SetSend()
		{
			SendData = true;
		}
		void ClientListen()
		{
			try
			{
				do
				{
					master = new TcpClient();
					master.Connect(MasterIp, Port);
				} while (!master.Connected);

				Debug.Log("Connected to Master");

				buffer = new byte[2048];
				var stream = master.GetStream();

				buffer = new byte[2048];
				while (true)
				{
					if (!stream.CanRead)
					{
						Debug.Log("Client Cannot Read");
						continue;
					}

					if (stream.DataAvailable)
					{
						int l = stream.Read(buffer, 0, buffer.Length);
						//Debug.Log("Client read " + l + "Bytes");
						string receivedString = Encoding.ASCII.GetString(buffer);
						Debug.Log("Received: " + receivedString);
						var rec = NetworkUtility.FromNetwork(receivedString);

						if (rec != null)
						{
							Run.OnMainThread(() => gameController.OnReceivePacket(rec));
						}
						else
						{
							Debug.LogWarning("Packet deserialization didn't work: " + receivedString);
						}
					}
				}
			}
			catch (SocketException ex)
			{
				Debug.Log("Client Socket Error");
				Debug.LogException(ex);
			}
		}

		void ClientWrite(string message)
		{
			if (master == null)
			{
				Debug.Log("No Master connected.");
				return;
			}
			try
			{
				var stream = master.GetStream();
				if (!stream.CanWrite)
				{
					Debug.Log("Client Cannot write to stream");
					return;
				}

				Debug.Log("Sent: " + message);

				byte[] messageBytes = Encoding.ASCII.GetBytes(message + "\n");
				stream.Write(messageBytes, 0, messageBytes.Length);
				stream.Flush();
			}
			catch (SocketException)
			{
				Debug.Log("Client Sending Failed");
			}
		}
	}
}