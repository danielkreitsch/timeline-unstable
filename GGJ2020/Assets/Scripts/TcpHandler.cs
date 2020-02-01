using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts
{
	public class TcpHandler : MonoBehaviour
	{
		#region private members 	
		private TcpClient connectionToMaster;
		private Thread clientReceiveThread;

		private TcpListener tcpListener;
		private Thread tcpListenerThread;
		private TcpClient connectionToClient;

		private bool dataRecieved = false;
		Byte[] recData;
		#endregion

		public string MasterIp = "127.0.0.1";
		public int Port = 12345;
		public bool IsServer = false;

		public bool SendData = false;
		public game_state State = new game_state();


		// Start is called before the first frame update
		void Start()
		{
			if (IsServer)
				StartTcpServer();
			else
				ConnectToTcpServer();
		}

		// Update is called once per frame
		void Update()
		{
			if (IsServer)
			{
				if (SendData)
				{
					SendData = false;
					State.tick++;
					var data = JsonUtility.ToJson(State);
					Debug.Log("Master Sent: " + data);
					TcpSend(data);
				}
				if (dataRecieved)
				{
					dataRecieved = false;
					var data = Encoding.ASCII.GetString(recData);
					game_state clientState = JsonUtility.FromJson<game_state>(data);
					Debug.Log("Master Recieved: " + data);
				}
			}
			else
			{
				if (SendData)
				{
					SendData = false;
					State.tick++;
					var data = JsonUtility.ToJson(State);
					Debug.Log("Client Sent: " + data);
					TcpSend(data);
				}
				if (dataRecieved)
				{
					dataRecieved = false;
					var data = Encoding.ASCII.GetString(recData);
					game_state clientState = JsonUtility.FromJson<game_state>(data);
					Debug.Log("Client Recieved: " + data);
				}
			}
		}
		private void StartTcpServer()
		{
			tcpListenerThread = new Thread(new ThreadStart(MasterListen));
			tcpListenerThread.IsBackground = true;
			tcpListenerThread.Start();
		}

		private void ConnectToTcpServer()
		{
			try
			{
				clientReceiveThread = new Thread(new ThreadStart(ClientListen));
				clientReceiveThread.IsBackground = true;
				clientReceiveThread.Start();
			}
			catch (Exception e)
			{
				Debug.Log("On client connect exception " + e);
			}
		}


		private void TcpSend(string message)
		{
			if (connectionToMaster == null)
				return;
			try
			{
				// Get a stream object for writing. 			
				NetworkStream stream = IsServer ? connectionToClient.GetStream() : connectionToMaster.GetStream();
				if (stream.CanWrite)
				{   // Convert string message to byte array.                 
					byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(message);
					// Write byte array to connectionToMaster stream.                 
					stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
					//Debug.Log("Client sent his message - should be received by server");
				}
			}
			catch (SocketException socketException)
			{
				Debug.Log("Socket exception: " + socketException);
			}
		}

		private void ClientListen()
		{
			try
			{
				connectionToMaster = new TcpClient(MasterIp, Port);
				recData = new byte[1024];
				while (true)
				{
					// Get a stream object for reading 				
					using (NetworkStream stream = connectionToMaster.GetStream())
					{
						int length;
						// Read incomming stream into byte arrary. 					
						while ((length = stream.Read(recData, 0, recData.Length)) != 0)
						{
							var incommingData = new byte[length];
							Array.Copy(recData, 0, incommingData, 0, length);
							// Convert byte array to string message. 						
							string serverMessage = Encoding.ASCII.GetString(incommingData);
							//Debug.Log("server message received as: " + serverMessage);
							dataRecieved = true;
						}
					}
				}
			}
			catch (SocketException socketException)
			{
				Debug.Log("Socket exception: " + socketException);
			}
		}
		private void MasterListen()
		{
			try
			{
				tcpListener = new TcpListener(IPAddress.Parse(MasterIp), Port);
				tcpListener.Start();
				Debug.Log("Server is listening");
				recData = new Byte[1024];
				while (true)
				{
					using (connectionToClient = tcpListener.AcceptTcpClient())
					{
						// Get a stream object for reading 					
						using (NetworkStream stream = connectionToClient.GetStream())
						{
							int length;
							// Read incomming stream into byte arrary. 						
							while ((length = stream.Read(recData, 0, recData.Length)) != 0)
							{
								var incommingData = new byte[length];
								Array.Copy(recData, 0, incommingData, 0, length);
								// Convert byte array to string message. 							
								string clientMessage = Encoding.ASCII.GetString(incommingData);
								//Debug.Log("client message received as: " + clientMessage);
								dataRecieved = true;
							}
						}
					}
				}
			}
			catch (SocketException socketException)
			{
				Debug.Log("SocketException " + socketException.ToString());
			}
		}

	}
}
