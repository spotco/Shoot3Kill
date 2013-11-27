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

public class SyncClient : MonoBehaviour {

	Thread _request_thread;
	Socket _socket;

	bool _id_alloced = false;

	void Start () {
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		System.Net.IPAddress    remoteIPAddress  = System.Net.IPAddress.Parse("127.0.0.1");
		System.Net.IPEndPoint   remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, Shoot3KillServer.PORT);
		socket.Connect(remoteEndPoint);

		_socket = socket;
		_request_thread = new Thread(new ThreadStart(request_thread));
		_request_thread.Start();

		PlayerInfo._id = Math.Abs(((int)DateTime.Now.Ticks))%10000000 * -1;
	}


	long _TEST_LAST = 0;

	void request_thread() {
		AsyncReadState state = new AsyncReadState();
		while (true) {

			ChatWindow.TEST_LAST_UPDATE = ""+(DateTime.Now.Ticks-_TEST_LAST);
			_TEST_LAST = DateTime.Now.Ticks;

			int read = _socket.Receive(state._buffer);
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
				
			} else {
				if (state._msg.Length > 0) {
					msg_recieved(state._msg.ToString());
				}
			}
		}
	}

	void msg_recieved(string msg_str) {
		SPServerMessage msg = SPServerMessage.from_json(JSONObject.Parse(msg_str));
		
		if (!_id_alloced) {
			foreach (SPEvent evt in msg._events) {
				if (evt._type == SPEvent.TYPE_ALLOC_ID && evt._player_id == PlayerInfo._id) {
					PlayerInfo._id = evt._value;
					_id_alloced = true;
				}
			}
		}
		
		OnlinePlayerManager.instance.msg_recieved(msg);
		BulletManager.instance.msg_recieved(msg);
	}

	bool send_message(string msg) {
		if (_socket == null || !_socket.Connected) return false;
		byte[] msg_bytes = Encoding.ASCII.GetBytes(msg+'\0');
		_socket.Send(msg_bytes);
		return true;
	}
	
	Vector3 _last_body_position;
	bool _has_last_position = false; 
	void Update () {
		SPClientMessage msg = new SPClientMessage();
		msg._player._pos = Util.vector3_to_spvector(gameObject.transform.position);
		msg._player._rot = Util.vector3_to_spvector(gameObject.transform.eulerAngles);
		if (!_has_last_position) {
			msg._player._vel._x = 0;
			msg._player._vel._y = 0;
			msg._player._vel._z = 0;
		} else {
			msg._player._vel._x = gameObject.transform.position.x - _last_body_position.x;
			msg._player._vel._y = gameObject.transform.position.y - _last_body_position.y;
			msg._player._vel._z = gameObject.transform.position.z - _last_body_position.z;
			
		}
		msg._player._id = PlayerInfo._id;

		foreach (Bullet b in BulletManager.instance._bullets) {
			SPBulletObject obj = new SPBulletObject();
			obj._pos = Util.vector3_to_spvector(b._position);
			obj._vel = Util.vector3_to_spvector(b._vel);
			obj._rot = Util.vector3_to_spvector(b._obj.transform.eulerAngles);
			obj._id = b._id;
			obj._playerid = obj._playerid;
			msg._bullets.Add(obj);
		}
		
		send_message(msg.to_json().ToString());
		
		_last_body_position = gameObject.transform.position;
		_has_last_position = true;
	}

}
