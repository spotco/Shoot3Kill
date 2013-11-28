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
	Thread _send_thread;
	Socket _socket;

	bool _id_alloced = false;

	public static string SERVER = "54.245.123.189";

	void Start () {
		Security.PrefetchSocketPolicy(SERVER,SocketPolicyServer.PORT,2000);

		PlayerInfo._id = Math.Abs(((int)DateTime.Now.Ticks))%10000000 * -1;
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		System.Net.IPAddress    remoteIPAddress  = System.Net.IPAddress.Parse(SERVER);
		System.Net.IPEndPoint   remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, Shoot3KillServer.PORT);


		socket.BeginConnect(remoteEndPoint,(IAsyncResult res)=>{
			_socket = (Socket) res.AsyncState;
			_socket.EndConnect(res);

			int sr_ct = 0;

			_request_thread = new Thread(new ThreadStart(() => {
				AsyncReadState state = new AsyncReadState();
				while (true) {
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
						Debug.Log ("ERROR::request thread end, server down");
						break;
					}

					sr_ct--;
				}
			}));
			_request_thread.Start();
			
			_send_thread = new Thread(new ThreadStart(() => {
				while (true) {
					if ((_socket == null || !_socket.Connected) || sr_ct > 10) {
						Thread.Sleep(40);
						continue;
						
					}
					
					string msg_text = null;
					bool cont = false;
					lock (_msg_send_queue) {
						cont = _msg_send_queue.Count > 0;
						if (cont) msg_text = _msg_send_queue.Dequeue();
					}
					if (!cont) {
						Thread.Sleep(40);
						continue;
					}
					
					byte[] msg_bytes = Encoding.ASCII.GetBytes(msg_text+'\0');
					_socket.Send(msg_bytes);

					sr_ct++;
				}
			}));
			_send_thread.Start();
		},socket);


#if SOCKET_SYNC
		socket.Connect(remoteEndPoint);
		_socket = socket;
		_request_thread = new Thread(new ThreadStart(request_thread));
		_request_thread.Start();
		
		_send_thread = new Thread(new ThreadStart(send_thread));
		_send_thread.Start();
#endif
	}

	Queue<string> _msg_recieve_queue = new Queue<string>();
	Queue<string> _msg_send_queue = new Queue<string>();

	void msg_recieved(string msg_str) {
		lock (_msg_recieve_queue) {
			if (_msg_recieve_queue.Count > 0) _msg_recieve_queue.Clear();
			_msg_recieve_queue.Enqueue(msg_str);
		}
	}

	void send_message(SPClientMessage msg) {
		lock (_msg_send_queue) {
			_msg_send_queue.Clear();
			_msg_send_queue.Enqueue(msg.to_json().ToString());
		}
	}

	void OnApplicationQuit() {
		_send_thread.Abort();
		_request_thread.Abort();
		_socket.Close();
	}
	
	Vector3 _last_body_position;
	bool _has_last_position = false; 
	void Update () {
		if (_socket == null || !_socket.Connected) {
			Debug.Log ("not connected");
			return;
		}

		while(true) {
			bool cont;
			string msg_str = null;
			lock (_msg_recieve_queue) {
				cont = _msg_recieve_queue.Count > 0;
				if (cont) msg_str = _msg_recieve_queue.Dequeue();
			}
			if (!cont) break;

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

		SPClientMessage msg_out = new SPClientMessage();
		msg_out._player._pos = Util.vector3_to_spvector(gameObject.transform.position);
		msg_out._player._rot = Util.vector3_to_spvector(gameObject.transform.eulerAngles);
		if (!_has_last_position) {
			msg_out._player._vel._x = 0;
			msg_out._player._vel._y = 0;
			msg_out._player._vel._z = 0;
		} else {
			msg_out._player._vel._x = gameObject.transform.position.x - _last_body_position.x;
			msg_out._player._vel._y = gameObject.transform.position.y - _last_body_position.y;
			msg_out._player._vel._z = gameObject.transform.position.z - _last_body_position.z;
			
		}
		msg_out._player._id = PlayerInfo._id;


		foreach (Bullet b in BulletManager.instance._bullets) {
			SPBulletObject obj = new SPBulletObject();
			obj._pos = Util.vector3_to_spvector(b._position);
			obj._vel = Util.vector3_to_spvector(b._vel);
			obj._rot = Util.vector3_to_spvector(b._obj.transform.eulerAngles);
			obj._id = b._id;
			obj._playerid = PlayerInfo._id;
			msg_out._bullets.Add(obj);
		}
		
		send_message(msg_out);
		
		_last_body_position = gameObject.transform.position;
		_has_last_position = true;
	}

}
