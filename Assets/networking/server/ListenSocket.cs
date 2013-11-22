using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Timers;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DataPhase {
	public Socket _socket;
	public int _len;
	public DataPhase(Socket s, int len) { _socket = s; _len = len; }
	public byte[] _msg;
}

public class ListenSocket {

	Socket _connection_socket;
	List<Socket> _header_phase = new List<Socket>();
	List<DataPhase> _data_phase = new List<DataPhase>();

	Timer timer;

	public delegate void OnMessage(string msg);
	public delegate void OnEvent();
	public delegate string OnBroadcast();

	public OnMessage _on_message;
	public OnEvent _on_connection_start;
	public OnEvent _on_connection_end;
	public OnBroadcast _on_broadcast;

	public void init() {
		_connection_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);     
		IPEndPoint ipLocal = new IPEndPoint ( IPAddress.Any , 6996);
		_connection_socket.Bind( ipLocal );
		_connection_socket.Listen (100);
		timer = new Timer(100);
		timer.Elapsed += new ElapsedEventHandler(update);
		timer.Enabled = true;
		timer.Interval = 200;

		_on_message = (string msg)=>{};
		_on_connection_end = ()=>{};
		_on_connection_start = ()=>{};
		_on_broadcast = () => {return "";};
	}

	public void stop() {
		timer.Stop();
	}

	private void update(object source, ElapsedEventArgs e) {
		handle_in();
		broadcast_out();
	}

	private void broadcast_out() {
		ArrayList all_sockets = new ArrayList(_header_phase);
		foreach(DataPhase dp in _data_phase) all_sockets.Add(dp._socket);
		Socket.Select(null,all_sockets,null,1000);
		foreach(Socket s in all_sockets) {
			byte[] msg = serialize_object(_on_broadcast());
			byte[] len_header = int_to_bytea4(msg.Length);
			s.Send(len_header);
			s.Send(msg);


		}
	}

	private void handle_in() {
		ArrayList listenList = new ArrayList();
		listenList.Add(_connection_socket);
		Socket.Select(listenList, null, null, 1000);

		for( int i = 0; i < listenList.Count; i++ ) {
			Socket newSocket = ((Socket)listenList[i]).Accept();
			_header_phase.Add(newSocket);
			_on_connection_start();
		}

		if (_header_phase.Count != 0) {
			ArrayList connections = new ArrayList (_header_phase);
			Socket.Select(connections, null, null, 1000);
			foreach (Socket socket in connections) {
				byte[] length_header = new byte[4];
				socket.Receive(length_header);
				int len = bytea4_to_int(length_header);
				_header_phase.Remove(socket);
				if (len != 0) {
					_data_phase.Add(new DataPhase(socket,len));
				} else {
					_on_connection_end();
				}
			}
		}

		if (_data_phase.Count != 0) {
			ArrayList connections = new ArrayList();
			foreach(DataPhase dp in _data_phase) {
				connections.Add(dp._socket);
			}
			Socket.Select(connections,null,null,1000);
			foreach (Socket socket in connections) {
				DataPhase p = null;
				foreach(DataPhase dp in _data_phase) {
					if (dp._socket == socket) {
						p = dp;
						break;
					}
				}

				int len = p._len;
				byte[] msg_bytes = new byte[len];
				socket.Receive(msg_bytes);
				_data_phase.Remove(p);
				_header_phase.Add(socket);
				string msg = (string)deserialize_object(msg_bytes);
				_on_message(msg);
			}
		}
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
			          
	
