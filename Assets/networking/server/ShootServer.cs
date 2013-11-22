using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Timers;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ShootServer {

	static Socket _connection_socket;

	static ArrayList _connections = new ArrayList ();
	static ArrayList _byteBuffer = new ArrayList ();

	public static void Main(string[] args) {
		/*
		SPPlayerObject obj1 = new SPPlayerObject();
		obj1._id = 5;

		MemoryStream stream = new MemoryStream();
		BinaryFormatter formatter = new BinaryFormatter();
		formatter.Serialize(stream, obj1);

		byte[] data = stream.ToArray();

		SPPlayerObject obj2 = (SPPlayerObject)formatter.Deserialize(new MemoryStream(data));
		Console.WriteLine(obj2._id);
		*/


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

	public static int bytea4_to_int(byte[] arg) {
		return arg[0] + (arg[1] << 8) + (arg[2] << 16) + (arg[3] << 24);
	}

	private static void update(object source, ElapsedEventArgs e) {
		ArrayList listenList = new ArrayList();
		listenList.Add(_connection_socket);
		Socket.Select(listenList, null, null, 1000);

		for( int i = 0; i < listenList.Count; i++ ) {
			Socket newSocket = ((Socket)listenList[i]).Accept();
			_connections.Add(newSocket);
			_byteBuffer.Add(new ArrayList());

			Console.WriteLine("connected!");
		}

		if (_connections.Count != 0) {
			ArrayList connections = new ArrayList (_connections);
			Socket.Select(connections, null, null, 1000);
			foreach (Socket socket in connections) {
				byte[] length_header = new byte[4];
				socket.Receive(length_header);
				int len = bytea4_to_int(length_header);
				Console.WriteLine("read length:"+len);


				/*
				byte[] receivedbytes = new byte[512];
				ArrayList buffer = (ArrayList)_byteBuffer[_connections.IndexOf(socket)];
				int read = socket.Receive(receivedbytes);
				for (int i=0;i<read;i++) buffer.Add(receivedbytes[i]);

				while (true && buffer.Count > 0) {
					int length = (byte)buffer[0];

					if (length < buffer.Count) {
						ArrayList thismsgBytes = new ArrayList(buffer);
						thismsgBytes.RemoveRange(length + 1, thismsgBytes.Count - (length + 1));
						thismsgBytes.RemoveRange(0, 1);

						buffer.RemoveRange(0, length + 1);
						byte[] readbytes = (byte[])thismsgBytes.ToArray(typeof(byte));

						MessageData readMsg = MessageData.FromByteArray(readbytes);
					}
					else break;
				}
				*/

				// string output = Encoding.UTF8.GetString(bytes);
			}           
		}
	}
}
