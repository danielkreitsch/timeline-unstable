using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;

public class TcpHandler : MonoBehaviour
{
	#region private members 	
	private TcpClient masterConnection;
	private Thread clientReceiveThread;

	private TcpListener tcpListener;
	private Thread tcpListenerThread;
	private TcpClient clientConnection;

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
				var data = JsonConvert.SerializeObject(State);
				Debug.Log("Master Sent: " + data);
				SendTcp(data);
			}
			if (dataRecieved)
			{
				dataRecieved = false;
				var data = Encoding.ASCII.GetString(recData);
				game_state client_state = JsonConvert.DeserializeObject<game_state>(data);
				Debug.Log("Master Recieved: " + data);
			}
		}
		else
		{
			if (SendData)
			{
				SendData = false;
				var data = JsonConvert.SerializeObject(State);
				Debug.Log("Client Sent: " + data);
				SendTcp(data);
			}
		}
	}





	private void ConnectToTcpServer()
	{
		try
		{
			clientReceiveThread = new Thread(new ThreadStart(ListenForData));
			clientReceiveThread.IsBackground = true;
			clientReceiveThread.Start();
		}
		catch (Exception e)
		{
			Debug.Log("On client connect exception " + e);
		}
	}

	private void ListenForData()
	{
		try
		{
			masterConnection = new TcpClient(MasterIp, Port);
			Byte[] bytes = new Byte[1024];
			while (true)
			{
				// Get a stream object for reading 				
				using (NetworkStream stream = masterConnection.GetStream())
				{
					int length;
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						var incommingData = new byte[length];
						Array.Copy(bytes, 0, incommingData, 0, length);
						// Convert byte array to string message. 						
						string serverMessage = Encoding.ASCII.GetString(incommingData);
						//Debug.Log("server message received as: " + serverMessage);
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}



	private void StartTcpServer()
	{
		tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
		tcpListenerThread.IsBackground = true;
		tcpListenerThread.Start();
	}


	private void ListenForIncommingRequests()
	{
		try
		{
			tcpListener = new TcpListener(IPAddress.Parse(MasterIp), Port);
			tcpListener.Start();
			Debug.Log("Server is listening");
			recData = new Byte[1024];
			while (true)
			{
				using (clientConnection = tcpListener.AcceptTcpClient())
				{
					// Get a stream object for reading 					
					using (NetworkStream stream = clientConnection.GetStream())
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

	private void SendTcp(string message)
	{
		if (clientConnection == null) return;

		try
		{
			// Get a stream object for writing. 			
			NetworkStream stream = IsServer ? masterConnection.GetStream() : clientConnection.GetStream();
			if (stream.CanWrite)
			{
				// Convert string message to byte array.                 
				byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(message);
				// Write byte array to master_connection stream.               
				stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
				//Debug.Log("Server sent his message - should be received by client");
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}



}
