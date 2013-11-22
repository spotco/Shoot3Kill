﻿using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;

public class Client : MonoBehaviour {

	public string m_IPAdress = "127.0.0.1";
	public const int kPort = 6996;

	private static Client singleton;


	private Socket m_Socket;
	void Start ()
	{
		m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		System.Net.IPAddress    remoteIPAddress  = System.Net.IPAddress.Parse(m_IPAdress);
		System.Net.IPEndPoint   remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, kPort);
		singleton = this;
		m_Socket.Connect(remoteEndPoint);

		Send(new MessageData());
	}

	void OnApplicationQuit ()
	{
		m_Socket.Close();
		m_Socket = null;
	}

	static public void Send(MessageData msgData)
	{
		if (singleton.m_Socket == null)
			return;

		byte[] sendData = MessageData.ToByteArray(msgData);
		byte[] prefix = new byte[1];
		prefix[0] = (byte)sendData.Length;
		singleton.m_Socket.Send(prefix);
		singleton.m_Socket.Send(sendData);
	}
}