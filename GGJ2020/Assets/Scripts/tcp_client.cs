using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

public class tcp_client : MonoBehaviour
{
	#region private members 	
	private TcpClient socketConnection;
	private Thread clientReceiveThread;

	private TcpListener tcpListener;
	private Thread tcpListenerThread;
	private TcpClient connectedTcpClient;

	private bool data_recieved = false;
	Byte[] rec_data;
	#endregion

	public string master_ip = "127.0.0.1";
	public int port = 12345;
	public bool is_server = false;

	public bool send_data = false;
	public game_state state = new game_state();


	// Start is called before the first frame update
	void Start()
	{
		if(is_server)
			StartTcpServer();
		else
			ConnectToTcpServer();
	}

	// Update is called once per frame
	void Update()
	{
		if (is_server)
		{
			if (data_recieved)
			{
				data_recieved = false;
				var data = Encoding.ASCII.GetString(rec_data);
				game_state client_state = JsonUtility.FromJson<game_state>(data);
				Debug.Log("Recieved: " + data);
			}
		}
		else
		{
			if (send_data)
			{
				send_data = false;
				var data = JsonUtility.ToJson(state);
				Debug.Log("Sent: " + data);
				SendMessage(data);
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
			socketConnection = new TcpClient(master_ip, port);
			Byte[] bytes = new Byte[1024];
			while (true)
			{
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream())
				{
					int length;
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						var incommingData = new byte[length];
						Array.Copy(bytes, 0, incommingData, 0, length);
						// Convert byte array to string message. 						
						string serverMessage = Encoding.ASCII.GetString(incommingData);
						Debug.Log("server message received as: " + serverMessage);
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}

	private void SendMessage(string message)
	{
		if (socketConnection == null)
			return;
		try
		{
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream();
			if (stream.CanWrite)
			{   // Convert string message to byte array.                 
				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(message);
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
				Debug.Log("Client sent his message - should be received by server");
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
			tcpListener = new TcpListener(IPAddress.Parse(master_ip), port);
			tcpListener.Start();
			Debug.Log("Server is listening");
			rec_data = new Byte[1024];
			while (true)
			{
				using (connectedTcpClient = tcpListener.AcceptTcpClient())
				{
					// Get a stream object for reading 					
					using (NetworkStream stream = connectedTcpClient.GetStream())
					{
						int length;
						// Read incomming stream into byte arrary. 						
						while ((length = stream.Read(rec_data, 0, rec_data.Length)) != 0)
						{
							var incommingData = new byte[length];
							Array.Copy(rec_data, 0, incommingData, 0, length);
							// Convert byte array to string message. 							
							string clientMessage = Encoding.ASCII.GetString(incommingData);
							Debug.Log("client message received as: " + clientMessage);
							data_recieved = true;
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
