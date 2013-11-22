using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Timers;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class Client : MonoBehaviour {

	Socket _socket;
	System.Timers.Timer timer;

	void Start ()
	{
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		System.Net.IPAddress    remoteIPAddress  = System.Net.IPAddress.Parse("127.0.0.1");
		System.Net.IPEndPoint   remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, 6996);
		socket.Connect(remoteEndPoint);
		_socket = socket;

		timer = new System.Timers.Timer();
		timer.Elapsed += new ElapsedEventHandler(socket_update);
		timer.Enabled = true;
		timer.Interval = 200;
	}

	int _last_next_size = -1;

	int cid = (new System.Random()).Next();

	void socket_update(object source, ElapsedEventArgs e) {
		if (_last_next_size != -1) {
			byte[] rcc = new byte[_last_next_size];
			_socket.Receive(rcc);
			string s = (string)ListenSocket.deserialize_object(rcc);
			Debug.Log(s);

		}

		byte[] serialized_msg_bytes = serialize_object("msg_from_player_"+cid);
		Debug.Log(serialized_msg_bytes.Length);
		byte[] len_header = int_to_bytea4(serialized_msg_bytes.Length);
		_socket.Send(len_header);
		_socket.Send(serialized_msg_bytes);

		byte[] rsb = new byte[4];
		_socket.Receive(rsb);
		_last_next_size = ListenSocket.bytea4_to_int(rsb);

	}

	void OnApplicationQuit ()
	{
		_socket.Close();
		_socket = null;
	}

	public static int bytea4_to_int(byte[] arg) {
		return arg[0] + (arg[1] << 8) + (arg[2] << 16) + (arg[3] << 24);
	}

	public static byte[] int_to_bytea4(int value) {
		uint uvalue = (uint)value;
		return new byte[] {
			(byte)uvalue,
			(byte)(uvalue >> 8),
			(byte)(uvalue >> 16),
			(byte)(uvalue >> 24)
		};
	}

	public static byte[] serialize_object(object o) {
		MemoryStream stream = new MemoryStream();
		(new BinaryFormatter()).Serialize(stream, o);
		return stream.ToArray();
	}
	public static object deserialize_object(byte[] b) {
		return (new BinaryFormatter()).Deserialize(new MemoryStream(b));
	}
}