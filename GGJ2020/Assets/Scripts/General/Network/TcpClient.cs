using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GGJ2020.Game;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GGJ2020
{
	public class TcpClient : TcpPeer
	{
		private Thread clientThread;
		private System.Net.Sockets.TcpClient master;
		private byte[] buffer;

		public string MasterIp = "127.0.0.1";
		public int Port = 12345;
		
		// Start is called before the first frame update
		public void Connect()
		{
			try
			{
				clientThread = new Thread(ClientListen);
				clientThread.IsBackground = true;
				clientThread.Start();

				if (SceneManager.GetActiveScene().name == "main_menu")
				{
					SceneManager.LoadScene("game");
				}
			}
			catch (SocketException e)
			{
				Debug.Log("On client connect exception " + e);
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
		void ClientListen()
		{
			try
			{
				do
				{
					master = new System.Net.Sockets.TcpClient();
					master.Connect(MasterIp, Port);
				} while (!master.Connected);

				Debug.Log("Connected to Master");

				var stream = master.GetStream();
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

				byte[] messageBytes = Encoding.UTF8.GetBytes(message + "\n");
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