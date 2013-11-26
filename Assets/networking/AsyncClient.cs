﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Timers;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;

public class AsyncClient : MonoBehaviour {

	Socket _server_socket;
	
	void Start () {
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		System.Net.IPAddress    remoteIPAddress  = System.Net.IPAddress.Parse("127.0.0.1");
		System.Net.IPEndPoint   remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, 6997);
		socket.BeginConnect(remoteEndPoint,new AsyncCallback(connect_callback),socket);
	}

	void connect_callback(IAsyncResult res) {
		Socket listener = (Socket) res.AsyncState;
		Socket handler = listener.EndAccept(res);
		_server_socket = listener;

		AsyncReadState state = new AsyncReadState();
		state._socket = handler;
		handler.BeginReceive(state._buffer,0,AsyncReadState.BUFFER_SIZE,0,new AsyncCallback(read_callback),state);
	}

	void read_callback(IAsyncResult res) {
		AsyncReadState state = (AsyncReadState) res.AsyncState;
		Socket handler = state._socket;
		int read = handler.EndReceive(res);
		
		if (read > 0) {
			int start = 0;
			int i = 0;
			for (; i < read; i++) {
				if (state._buffer[i] == (byte)'\0') {
					state._msg.Append(Encoding.ASCII.GetString(state._buffer,start,i));
					msg_recieved(state._msg.ToString());
					state._msg.Remove(0,state._msg.Length);
					start = i + 1;
				}
			}
			state._msg.Append(Encoding.ASCII.GetString(state._buffer,start,read-start));
			handler.BeginReceive(state._buffer,0,AsyncReadState.BUFFER_SIZE,0,new AsyncCallback(read_callback),state);
			
		} else {
			msg_recieved(state._msg.ToString());
		}
	}

	void send_callback(IAsyncResult res) {
		Socket listener = (Socket) res.AsyncState;
		listener.EndSend(res);
	}

	bool send_message(string msg) {
		if (_server_socket == null) {
			Debug.Log ("server connection not established");
			return false;
		}
		byte[] msg_bytes = Encoding.ASCII.GetBytes(msg+'\0');
		_server_socket.BeginSend(msg_bytes,0,msg_bytes.Length,0,new AsyncCallback(send_callback),_server_socket);
		return true;
	}

	void msg_recieved(string msg) {
		Debug.Log ("recieved:"+msg);
	}

	int ct = 0;
	void Update () {
		send_message("abcdefghijklmnopqrstuvwxyz".Substring(ct));
		ct++;
		if (ct > 26) ct = 0;
	}
}
