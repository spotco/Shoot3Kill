using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;

public class AsyncReadState {
	public Socket _socket = null;
	public const int BUFFER_SIZE = 65535;
	public byte[] _buffer = new byte[BUFFER_SIZE];
	public StringBuilder _msg = new StringBuilder();
}

public class Shoot3KillServer {
	
	public static int PORT = 6976;

	public static void Main(string[] args) {
		SocketPolicyServer server = new SocketPolicyServer (SocketPolicyServer.AllPolicy);
		server.Start();

		Shoot3KillServer shoot_server = new Shoot3KillServer();
		shoot_server.start();

		while (true) {
			IOut.Log("Input:");
			string input = Console.ReadLine();
			if (input == "q") {
				break;
			}
		}

		server.Stop();
		shoot_server.stop();

	}

	ManualResetEvent _accept_thread_block = new ManualResetEvent(false);
	Socket _connection_socket;
	Thread _update_thread;
	Thread _accept_thread;

	public void start() {

		_connection_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		_connection_socket.Bind(new IPEndPoint ( IPAddress.Any , PORT));
		_connection_socket.Listen(100);

		_update_thread = new Thread(new ThreadStart(()=>{
			while (true) {
				update ();
				Thread.Sleep(100);
			}
		}));
		_update_thread.Start();

		_accept_thread = new Thread(new ThreadStart(()=>{
			while (true) {
				_accept_thread_block.Reset();
				_connection_socket.BeginAccept(new AsyncCallback(accept_callback),_connection_socket);
				_accept_thread_block.WaitOne();
			}
		}));
		_accept_thread.Start();
	}

	public void stop() {
		_update_thread.Abort();
		_accept_thread.Abort();
		_connection_socket.Close();
		IOut.Log("stopped");
	}

	public void accept_callback(IAsyncResult res) {
		Socket listener = (Socket) res.AsyncState;
		Socket handler = listener.EndAccept(res);

		_accept_thread_block.Set();

		AsyncReadState state = new AsyncReadState();
		state._socket = handler;
		handler.BeginReceive(state._buffer,0,AsyncReadState.BUFFER_SIZE,0,new AsyncCallback(read_callback),state);
		send_action(state._socket);
	}

	public void send_action(Socket target) {
		byte[] msg_bytes = Encoding.ASCII.GetBytes(msg_send()+'\0');
		target.BeginSend(msg_bytes,0,msg_bytes.Length,0,new AsyncCallback(send_callback),target);
	}

	public void send_callback(IAsyncResult res) {
		Socket listener = (Socket) res.AsyncState;
		try {
			listener.EndSend(res);
		} catch (Exception e) {
			listener.Close();
		}
	}

	public void read_callback(IAsyncResult res) {
		AsyncReadState state = (AsyncReadState) res.AsyncState;
		Socket handler = state._socket;
		try {
			int read = handler.EndReceive(res);
			if (read > 0) {
				int start = 0;
				int i = 0;
				for (; i < read; i++) {
					if (state._buffer[i] == (byte)'\0') {
						state._msg.Append(Encoding.ASCII.GetString(state._buffer,start,i));
						msg_recieved(state._msg.ToString());
						send_action(handler);
						state._msg.Remove(0,state._msg.Length);
						start = i + 1;
					}
				}
				state._msg.Append(Encoding.ASCII.GetString(state._buffer,start,read-start));
				handler.BeginReceive(state._buffer,0,AsyncReadState.BUFFER_SIZE,0,new AsyncCallback(read_callback),state);	

			} else {
				if (state._msg.Length > 0) {
					msg_recieved(state._msg.ToString());
					IOut.Log(state._msg.ToString());
				}
				state._msg.Remove(0,state._msg.Length);
			}

				

		} catch (Exception e){
			handler.Close();
		}


	}

//----------------------------------------------

	HashSet<int> _generated_ids_outstanding = new HashSet<int>();

	List<SPEvent> _events = new List<SPEvent>();
	Dictionary<int,SPPlayerObject> _id_to_players = new Dictionary<int, SPPlayerObject>();
	Dictionary<string,SPBulletObject> _key_to_bullets = new Dictionary<string, SPBulletObject>();

	Queue<string> _queued_client_msgs = new Queue<string>();
	readonly object _queued_client_msgs_lock = new object();
	int _allocid = 0;

	public void msg_recieved(string msg) {
		lock (_queued_client_msgs_lock) {
			_queued_client_msgs.Enqueue(msg);
		}
	}

	public string msg_send() {
		SPServerMessage msg = new SPServerMessage();

		foreach(int id in _id_to_players.Keys) {
			msg._players.Add(_id_to_players[id]);
		}

		foreach(string bullet_key in _key_to_bullets.Keys) {
			msg._bullets.Add(_key_to_bullets[bullet_key]);
		}

		foreach(SPEvent evt in _events) {
			msg._events.Add(evt);
		}

		return msg.to_json().ToString();
	}

	private void update() {
		while (true) {
			bool queue_empty = false;
			string next_client_msg_str = null;
			lock (_queued_client_msgs_lock) {
				queue_empty = _queued_client_msgs.Count == 0;
				if (!queue_empty) next_client_msg_str = _queued_client_msgs.Dequeue();
			}
			if (queue_empty) break;

			SPClientMessage next_client_msg = SPClientMessage.from_json(JSONObject.Parse(next_client_msg_str));

			if (next_client_msg._player._id < 0) {
				if (!_generated_ids_outstanding.Contains(next_client_msg._player._id)) {
					_events.Add(new SPEvent(SPEvent.TYPE_ALLOC_ID,next_client_msg._player._id,_allocid));
					_allocid++;
				}
				_generated_ids_outstanding.Add(next_client_msg._player._id);

			} else {
				for (int i = _events.Count-1; i >= 0; i--) {
					SPEvent evt = _events[i];
					if (evt._type == SPEvent.TYPE_ALLOC_ID && next_client_msg._player._id == evt._value) {
						_events.RemoveAt(i);
					}
				}

				if (!_id_to_players.ContainsKey(next_client_msg._player._id)) {
					SPPlayerObject neu_obj = new SPPlayerObject(
						next_client_msg._player._id,
						next_client_msg._player._name,
						next_client_msg._player._pos,
						next_client_msg._player._vel,
						next_client_msg._player._rot
					);
					_id_to_players[next_client_msg._player._id] = neu_obj;
				}

				SPPlayerObject tar_obj = _id_to_players[next_client_msg._player._id];
				tar_obj._pos = next_client_msg._player._pos;
				tar_obj._rot = next_client_msg._player._rot;
				tar_obj._vel = next_client_msg._player._vel;
				tar_obj.__timeout = 10;

				foreach(SPBulletObject b in next_client_msg._bullets) {
					if (!_key_to_bullets.ContainsKey(b.unique_key())) {
						_key_to_bullets[b.unique_key()] = new SPBulletObject();
					}

					SPBulletObject tar = _key_to_bullets[b.unique_key()];
					tar._id = b._id;
					tar._playerid = b._playerid;
					tar._pos = b._pos.copy();
					tar._rot = b._rot.copy();
					tar._vel = b._vel.copy();
					tar.__timeout = 10;
				}
			}
		}

		List<string> uniquekeys_to_remove = new List<string>();
		foreach(string key in _key_to_bullets.Keys) {
			SPBulletObject b = _key_to_bullets[key];
			b.__timeout--;
			if (b.__timeout <= 0) uniquekeys_to_remove.Add(key);
		}
		foreach(string key in uniquekeys_to_remove) _key_to_bullets.Remove(key);

		List<int> ids_to_remove = new List<int>();
		foreach(int id in _id_to_players.Keys) {
			SPPlayerObject obj = _id_to_players[id];
			obj.__timeout--;
			if (obj.__timeout <= 0) {
				ids_to_remove.Add(id);
			}
		}
		foreach(int i in ids_to_remove) _id_to_players.Remove(i);

		for (int i = _events.Count-1; i >= 0; i--) {
			SPEvent evt = _events[i];
			evt.__duration--;
			if (evt.__duration <= 0) {
				_events.RemoveAt(i);
			}
		}

	}
}

