using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Timers;
using System;
using System.IO;

public class ShootServer  {

	public static void Main(string[] args) {
		ListenSocket s = new ListenSocket();
		s.init();
		s._on_message = (string msg) => {
			Console.WriteLine(msg);
		};
		s._on_broadcast = () => {
			return "";	
		};

		while (true) {
			Console.WriteLine("Q to quit");
			string input = Console.ReadLine();
			if (input == "q") break;
		}

		s.stop();
	}
}
