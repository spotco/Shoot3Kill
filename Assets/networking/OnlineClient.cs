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
using System.Threading;

public class OnlineClient : MonoBehaviour {
	
	public static int _player_id = -((new System.Random()).Next()%100)-1;
	static Socket _socket = null;
	static int _last_next_size = -1;
	
	private static readonly object _send_lock = new object();
	private static readonly object _recieve_lock = new object();
	
	static OnlineClient instance;
	
	static Thread _request_thread;
	
	void Start ()
	{
		instance = this;
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		System.Net.IPAddress    remoteIPAddress  = System.Net.IPAddress.Parse("127.0.0.1");
		System.Net.IPEndPoint   remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, 6996);
		socket.Connect(remoteEndPoint);
		_socket = socket;
		sendct = 0;
		_request_thread = new Thread(new ThreadStart(OnlineClient.request_thread));
		_request_thread.Start();
		
	}
	
	public static void recieve_message(string msg) {
		lock (_recieve_lock) {
			_cached_recieve_message = msg;	
		}
	}
	
	public static string send_message() {
		lock (_send_lock) {
			return _cached_send_message;	
		}
	}
	
	static int sendct = 0;
	static void request_thread() {
		while(true) {
			if (_last_next_size != -1) {
				byte[] rcc = new byte[_last_next_size];
				_socket.Receive(rcc);
				string s = (string)ListenSocket.deserialize_object(rcc);
				recieve_message(s);
	
			}
			
			if (sendct == 0 || _player_id >= 0) {
				byte[] serialized_msg_bytes = OnlineClient.serialize_object(send_message());
				byte[] len_header = int_to_bytea4(serialized_msg_bytes.Length);
				_socket.Send(len_header);
				_socket.Send(serialized_msg_bytes);
				
			}
			if (send_message().Length != 0) {
				sendct++;
			}
	
			byte[] rsb = new byte[4];
			_socket.Receive(rsb);
			_last_next_size = ListenSocket.bytea4_to_int(rsb);
			
			Thread.Sleep(200);
		}
		
	}
	
	static string _cached_send_message = "";
	void update_send() {
		JSON neu_json = new JSON();
		neu_json["playerid"] = _player_id;
		neu_json["pos"] = (JSON)gameObject.transform.position;
		neu_json["vel"] = (JSON)gameObject.rigidbody.velocity;
		neu_json["rot"] = (JSON)gameObject.transform.eulerAngles;
		neu_json["name"] = name;
		neu_json["bullets"] = new ArrayList();
		
		lock (_send_lock) {
			_cached_send_message = neu_json.serialized;
		}
	}
	
	static string _cached_recieve_message = "";
	void update_recieve() {
		JSON json = new JSON();
		bool kill = false;
		lock (_recieve_lock) {
			if (_cached_recieve_message.Length ==0) kill = true;
		}
		if (kill) return;
		lock (_recieve_lock) {
			json.serialized = _cached_recieve_message;
		}
		
		Array events = json.ToArray<JSON>("events");
		foreach(JSON evt in events) {
			int type = evt.ToInt("type");
			int player_id = evt.ToInt("player_id");
			int val = evt.ToInt("value");
			
			if (type == SPEvent.ALLOC_ID && _player_id == player_id) {
				_player_id = val;	
			}
		}
		
		SPMessage neu_message = new SPMessage();
		foreach(JSON player in json.ToArray<JSON>("players")) {
			SPPlayerObject neu_obj = new SPPlayerObject();
			neu_obj._id = player.ToInt("id");
			if (neu_obj._id == _player_id) continue;
			
			neu_obj._name = player.ToString("name");
			neu_obj._pos = SPVector.from_jsonobject(player.ToJSON("pos"));
			neu_obj._vel = SPVector.from_jsonobject(player.ToJSON("vel"));
			neu_obj._rot = SPVector.from_jsonobject(player.ToJSON("rot"));
			
			neu_message._players.Add(neu_obj);
		}
	}
	
	void Update() {
		update_send();
		update_recieve();
	}
	
	void OnApplicationQuit() {
		_request_thread.Abort();
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