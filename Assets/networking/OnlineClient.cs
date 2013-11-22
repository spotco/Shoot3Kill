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

public class OnlineClient : MonoBehaviour {
	
	public static int _player_id = -((new System.Random()).Next()%100)-1;
	
	public void recieve_message(string msg) {
		JSON json = new JSON();
		json.serialized = msg;
		
		
		Array events = json.ToArray<JSON>("events");
		foreach(JSON evt in events) {
			int type = evt.ToInt("type");
			int player_id = evt.ToInt("player_id");
			int val = evt.ToInt("value");
			Debug.Log("event from:"+_player_id+ " aka:"+player_id+ " to:"+val);
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
		
		//Debug.Log (neu_message._players.Count);
	}
	
	public string send_message() {
		JSON neu_json = new JSON();
		neu_json["playerid"] = _player_id;
		neu_json["pos"] = (JSON)gameObject.transform.position;
		neu_json["vel"] = (JSON)gameObject.rigidbody.velocity;
		neu_json["rot"] = (JSON)gameObject.transform.eulerAngles;
		neu_json["name"] = name;
		neu_json["bullets"] = new ArrayList();
		
		return neu_json.serialized;	
	}

	Socket _socket;
	void Start ()
	{
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		System.Net.IPAddress    remoteIPAddress  = System.Net.IPAddress.Parse("127.0.0.1");
		System.Net.IPEndPoint   remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, 6996);
		socket.Connect(remoteEndPoint);
		_socket = socket;

	}
	
	int ct = 0;
	void Update() {
		ct++;
		if (ct%10 ==0) {
			if (_last_next_size != -1) {
				byte[] rcc = new byte[_last_next_size];
				_socket.Receive(rcc);
				string s = (string)ListenSocket.deserialize_object(rcc);
				recieve_message(s);
	
			}

			byte[] serialized_msg_bytes = serialize_object(send_message());
			byte[] len_header = int_to_bytea4(serialized_msg_bytes.Length);
			_socket.Send(len_header);
			_socket.Send(serialized_msg_bytes);
	
			byte[] rsb = new byte[4];
			_socket.Receive(rsb);
			_last_next_size = ListenSocket.bytea4_to_int(rsb);
		}
	}

	int _last_next_size = -1;

	int cid = (new System.Random()).Next();


	void OnApplicationQuit ()
	{
		//_socket.Close();
		//_socket = null;
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