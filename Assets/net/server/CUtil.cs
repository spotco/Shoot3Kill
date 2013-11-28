using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;


public class CUtil {

	
	static Dictionary<string,long> _starts = new Dictionary<string, long>();
	public static void time_start(string name) {
		_starts[name] = DateTime.Now.Ticks;
	}

	public static long time_since(string name) {
		if (!_starts.ContainsKey(name)) return 0;
		return DateTime.Now.Ticks - _starts[name];
	}

}
