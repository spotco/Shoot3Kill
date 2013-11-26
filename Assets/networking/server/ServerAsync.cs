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

public class ServerAsync {

	static ManualResetEvent _accept_thread_block = new ManualResetEvent(false);

	public static void Main(string[] args) {
		Socket connection_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		connection_socket.Bind(new IPEndPoint ( IPAddress.Any , 6997));
		connection_socket.Listen(6996);
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

		
	}

	public static void send_action(Socket target) {
		if (!target.Connected) {
			Console.WriteLine("target not connected");
			return;
		}
		byte[] msg_bytes = Encoding.ASCII.GetBytes(msg_send()+'\0');
		target.BeginSend(msg_bytes,0,msg_bytes.Length,0,new AsyncCallback(send_callback),target);
	}

	public static void send_callback(IAsyncResult res) {
		Socket listner = (Socket) res.AsyncState;
		listner.EndSend(res);
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

	public static void msg_recieved(string msg) {
		Console.WriteLine("recieved:("+msg+")");
	}

	public static string msg_send() {
		return "top lel";
	}
}

public class AsyncReadState {
	public Socket _socket = null;
	public const int BUFFER_SIZE = 1024;
	public byte[] _buffer = new byte[BUFFER_SIZE];
	public StringBuilder _msg = new StringBuilder();
}

