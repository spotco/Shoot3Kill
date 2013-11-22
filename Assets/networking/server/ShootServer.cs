using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Timers;
using System;

public class ShootServer {

	static Socket _connection_socket;

	static ArrayList _connections = new ArrayList ();
	static ArrayList _buffer = new ArrayList ();
	static ArrayList _byteBuffer = new ArrayList ();

	public static void Main(string[] args) {
		_connection_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);     
		IPEndPoint ipLocal = new IPEndPoint ( IPAddress.Any , 6996);
		_connection_socket.Bind( ipLocal );
		_connection_socket.Listen (100);
		
		Console.WriteLine("fuck");
		Timer timer = new Timer(100);
		timer.Elapsed += new ElapsedEventHandler(update);
		timer.Enabled = true;
		timer.Interval = 2000;

		while (true) {
			Console.WriteLine("Q to quit");
			string input = Console.ReadLine();
			if (input == "q") break;
		}
		timer.Stop();
	}

	private static void update(object source, ElapsedEventArgs e) {
		// Accept any incoming connections!
		ArrayList listenList = new ArrayList();
		listenList.Add(_connection_socket);
		Socket.Select(listenList, null, null, 1000);

		for( int i = 0; i < listenList.Count; i++ ) {
			Socket newSocket = ((Socket)listenList[i]).Accept();
			_connections.Add(newSocket);
			_byteBuffer.Add(new ArrayList());

			Console.WriteLine("connected!");
		}

		// Read data from the connections!
		if (_connections.Count != 0) {
			ArrayList connections = new ArrayList (_connections);
			Socket.Select(connections, null, null, 1000);
			// Go through all sockets that have data incoming!
			foreach (Socket socket in connections) {
				byte[] receivedbytes = new byte[512];

				ArrayList buffer = (ArrayList)_byteBuffer[_connections.IndexOf(socket)];
				int read = socket.Receive(receivedbytes);
				for (int i=0;i<read;i++)
					buffer.Add(receivedbytes[i]);

				while (true && buffer.Count > 0) {
					int length = (byte)buffer[0];

					if (length < buffer.Count) {
						ArrayList thismsgBytes = new ArrayList(buffer);
						thismsgBytes.RemoveRange(length + 1, thismsgBytes.Count - (length + 1));
						thismsgBytes.RemoveRange(0, 1);

						buffer.RemoveRange(0, length + 1);
						byte[] readbytes = (byte[])thismsgBytes.ToArray(typeof(byte));

						MessageData readMsg = MessageData.FromByteArray(readbytes);
						_buffer.Add(readMsg);
					}
					else
						break;
				}

				// string output = Encoding.UTF8.GetString(bytes);
			}           
		}
	}
}
