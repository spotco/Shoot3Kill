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


	static long _start;
	public static void time_start() {
		_start = DateTime.Now.Ticks;
	}

	public static long time_since() {
		return DateTime.Now.Ticks - _start;
	}

}
