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

[System.Serializable]
public class S3KOnlineClient : MonoBehaviour {

	public static S3KOnlineClient inst;
	public bool _id_alloced = false;

	Thread _request_thread;
	Thread _send_thread;
	public Socket _socket;

	//public static string SERVER = "54.245.123.189";
	public static string SERVER = "127.0.0.1";

	void Start () {
		inst = this;
		Security.PrefetchSocketPolicy(SERVER,SocketPolicyServer.PORT,2000);

		PlayerInfo._id = Math.Abs(((int)DateTime.Now.Ticks))%10000000 * -1;
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		System.Net.IPAddress    remoteIPAddress  = System.Net.IPAddress.Parse(SERVER);
		System.Net.IPEndPoint   remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, Shoot3KillServer.PORT);


		socket.BeginConnect(remoteEndPoint,(IAsyncResult res)=>{
			_socket = (Socket) res.AsyncState;
			_socket.EndConnect(res);

			bool send_ok = true;

			_request_thread = new Thread(new ThreadStart(() => {
				AsyncReadState state = new AsyncReadState();
				while (true) {
					int read = _socket.Receive(state._buffer);
					
					if (read > 0) {
						int start = 0;
						int i = 0;
						
						for (; i < read; i++) {
							if (state._buffer[i] == (byte)Shoot3KillServer.MSG_TERMINATOR) {
								state._msg.Append(Encoding.ASCII.GetString(state._buffer,start,i));

								msg_recieved(state._msg.ToString());
								state._msg.Remove(0,state._msg.Length);
								start = i + 1;
								send_ok = true;
							}
						}
						state._msg.Append(Encoding.ASCII.GetString(state._buffer,start,read-start));
						
					} else {
						Debug.Log ("ERROR::request thread end, server down");
						break;
					}
				}
			}));
			_request_thread.Start();
			
			_send_thread = new Thread(new ThreadStart(() => {
				while (true) {
					if ((_socket == null || !_socket.Connected) || !send_ok) {
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
					
					byte[] msg_bytes = Encoding.ASCII.GetBytes(msg_text+Shoot3KillServer.MSG_TERMINATOR);

					_socket.Send(msg_bytes);

					send_ok = false;
				}
			}));
			_send_thread.Start();
		},socket);
	}

	Queue<string> _msg_recieve_queue = new Queue<string>();
	Queue<string> _msg_send_queue = new Queue<string>();
	
	void msg_recieved(string msg_str) {
		S3KGUI.inst._latency = (int)(IUtil.time_since("msg_recieved")/10000);
		IUtil.time_start("msg_recieved");

		lock (_msg_recieve_queue) {
			_msg_recieve_queue.Clear();
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

	void Update () {
		if (_socket == null || !_socket.Connected) {
			S3KGUI.inst._status = "not connected!";
			return;
		}

		if (!PlayerInfo._logged_in) {
			S3KGUI.inst._status = "not logged in!";
		} else if (!_id_alloced) {
			S3KGUI.inst._status = "allocating id...";
		} else {
			S3KGUI.inst._status = "logged in as: "+PlayerInfo._name;
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

			OnlinePlayerManager.inst.msg_recieved(msg);
			BulletManager.inst.msg_recieved(msg);
		}


		SPClientMessage msg_out = new SPClientMessage();

		msg_out._player._pos = Util.vector3_to_spvector(gameObject.transform.position);
		msg_out._player._rot = Util.vector3_to_spvector(gameObject.transform.eulerAngles);
		if (!S3KControl.inst._has_last_position) {
			msg_out._player._vel._x = 0;
			msg_out._player._vel._y = 0;
			msg_out._player._vel._z = 0;
		} else {
			msg_out._player._vel._x = gameObject.transform.position.x - S3KControl.inst._last_body_position.x;
			msg_out._player._vel._y = gameObject.transform.position.y - S3KControl.inst._last_body_position.y;
			msg_out._player._vel._z = gameObject.transform.position.z - S3KControl.inst._last_body_position.z;
			
		}

		msg_out._player._alive = PlayerInfo._alive ? 1 : 0;
		msg_out._player._id = PlayerInfo._id;
		msg_out._player._name = PlayerInfo._name;


		foreach (Bullet b in BulletManager.inst._bullets) {
			SPBulletObject obj = new SPBulletObject();
			obj._pos = Util.vector3_to_spvector(b._position);
			obj._vel = Util.vector3_to_spvector(b._vel);
			obj._rot = Util.vector3_to_spvector(b._obj.transform.eulerAngles);
			obj._id = b._id;
			obj._playerid = PlayerInfo._id;
			msg_out._bullets.Add(obj);
		}
		
		send_message(msg_out);
	}

}
