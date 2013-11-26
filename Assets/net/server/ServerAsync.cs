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


public class AsyncReadState {
	public Socket _socket = null;
	public const int BUFFER_SIZE = 1024;
	public byte[] _buffer = new byte[BUFFER_SIZE];
	public StringBuilder _msg = new StringBuilder();
}

public class ServerAsync {
	
	public static int PORT = 7004;
	static ManualResetEvent _accept_thread_block = new ManualResetEvent(false);

	public static void Main(string[] args) {

		System.Timers.Timer timer = new System.Timers.Timer(100);
		timer.Elapsed += new ElapsedEventHandler(update);
		timer.Enabled = true;

		Socket connection_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		connection_socket.Bind(new IPEndPoint ( IPAddress.Any , PORT));
		connection_socket.Listen(PORT);
		while (true) {
			_accept_thread_block.Reset();
			connection_socket.BeginAccept(new AsyncCallback(accept_callback),connection_socket);
			_accept_thread_block.WaitOne();
		}
	}

	public static void accept_callback(IAsyncResult res) {
		Socket listener = (Socket) res.AsyncState;
		Socket handler = listener.EndAccept(res);

		_accept_thread_block.Set();

		AsyncReadState state = new AsyncReadState();
		state._socket = handler;
		handler.BeginReceive(state._buffer,0,AsyncReadState.BUFFER_SIZE,0,new AsyncCallback(read_callback),state);
		send_action(state._socket);
	}

	public static void send_action(Socket target) {
		byte[] msg_bytes = Encoding.ASCII.GetBytes(msg_send()+'\0');
		target.BeginSend(msg_bytes,0,msg_bytes.Length,0,new AsyncCallback(send_callback),target);
	}

	public static void send_callback(IAsyncResult res) {
		try {
			Socket listner = (Socket) res.AsyncState;
			listner.EndSend(res);
		} catch (Exception e) {}
	}

	public static void read_callback(IAsyncResult res) {
		AsyncReadState state = (AsyncReadState) res.AsyncState;
		Socket handler = state._socket;
		int read = handler.EndReceive(res);
		send_action(handler);

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

//----------------------------------------------

	static HashSet<int> _generated_ids_outstanding = new HashSet<int>();

	static List<SPEvent> _events = new List<SPEvent>();
	static Dictionary<int,SPPlayerObject> _id_to_players = new Dictionary<int, SPPlayerObject>();

	static Queue<string> _queued_client_msgs = new Queue<string>();
	static readonly object _queued_client_msgs_lock = new object();
	static int _allocid = 0;

	public static void msg_recieved(string msg) {
		lock (_queued_client_msgs_lock) {
			_queued_client_msgs.Enqueue(msg);
		}
	}

	public static string msg_send() {
		SPServerMessage msg = new SPServerMessage();

		foreach(int id in _id_to_players.Keys) {
			msg._players.Add(_id_to_players[id]);
		}

		foreach(SPEvent evt in _events) {
			msg._events.Add(evt);
		}

		return msg.to_json().ToString();
	}

	private static void update(object source, ElapsedEventArgs e) {
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
				foreach(SPEvent evt in _events) {
					if (evt._type == SPEvent.TYPE_ALLOC_ID && next_client_msg._player._id == evt._value) {
						_events.Remove(evt);
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
				tar_obj.__timeout = 50;
			}
		}

		foreach(int id in _id_to_players.Keys) {
			SPPlayerObject obj = _id_to_players[id];
			obj.__timeout--;
			if (obj.__timeout <= 0) {
				_id_to_players.Remove(id);
			}
		}

		foreach(SPEvent evt in _events) {
			evt.__duration--;
			if (evt.__duration <= 0) {
				_events.Remove(evt);
			}
		}

		Console.WriteLine(msg_send());

	}
}

