using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Timers;
using System;
using System.IO;

public class ShootServer  {
	
	static int _allocid = 0;
	static ArrayList _queued_events = new ArrayList();
	
	static Dictionary<int,SPPlayerObject> _pid_to_player = new Dictionary<int, SPPlayerObject>();

	public static void Main(string[] args) {
		ListenSocket s = new ListenSocket();
		s.init();
		s._on_message = (string msg) => {
			JSON js = new JSON();
			js.serialized = msg;
			int id = js.ToInt("playerid");
			string name = js.ToString("name");
			
			SPVector pos = SPVector.from_jsonobject(js.ToJSON("pos"));
			SPVector vel = SPVector.from_jsonobject(js.ToJSON("vel"));
			SPVector rot = SPVector.from_jsonobject(js.ToJSON("rot"));
			
			if (id < 0) {
				_queued_events.Add(new SPEvent(SPEvent.ALLOC_ID,id,_allocid));
				id = _allocid;
				_allocid++;
			}
			
			if (!_pid_to_player.ContainsKey(id)) {
				_pid_to_player.Add(id,new SPPlayerObject());	
			}
			
			SPPlayerObject tar_obj = _pid_to_player[id];
			tar_obj._id = id;
			tar_obj._name = name;
			tar_obj._pos = pos;
			tar_obj._vel = vel;
			tar_obj._rot = rot;
			tar_obj.__timeout = 30;
			
		};
		s._on_broadcast = () => {
			
			foreach(int key in _pid_to_player.Keys) {
				SPPlayerObject obj = _pid_to_player[key];
				obj.__timeout--;
				if (obj.__timeout <= 0) {
					_pid_to_player.Remove(key);	
				}
			}
			
			JSON rtv = new JSON();
			ArrayList players = new ArrayList();
			rtv.fields["players"] = players;
			foreach(int pid in _pid_to_player.Keys) {
				SPPlayerObject player = _pid_to_player[pid];
				JSON json_pl = new JSON();
				json_pl.fields["id"] = player._id;
				json_pl.fields["name"] = player._name;
				json_pl.fields["pos"] = SPVector.to_jsonobj(player._pos);
				json_pl.fields["vel"] = SPVector.to_jsonobj(player._vel);
				json_pl.fields["rot"] = SPVector.to_jsonobj(player._rot);
				players.Add(json_pl);
			}
			
			ArrayList evts = new ArrayList();
			rtv.fields["events"] = evts;
			foreach(SPEvent spevt in _queued_events) {
				evts.Add(SPEvent.to_jsonobj(spevt));	
			}
			_queued_events.Clear();
			Console.WriteLine(rtv.serialized);
			return rtv.serialized;
		};

		while (true) {
			Console.WriteLine("Q to quit");
			string input = Console.ReadLine();
			if (input == "q") break;
		}

		s.stop();
	}
}

public class SPEvent {
	public static int ALLOC_ID;
	public int _type;
	public int _player_id;
	public int _value;
	public SPEvent(int type, int player_id,int v) {
		_type = type;
		_player_id = player_id;
		_value = v;
	}
	public static JSON to_jsonobj(SPEvent evt) {
		JSON rtv = new JSON();
		rtv.fields["type"] = evt._type;
		rtv.fields["player_id"] = evt._player_id;
		rtv.fields["value"] = evt._value;
		return rtv;
	}
}

public class SPVector {
	public float _x,_y,_z;
	public SPVector(float x, float y, float z) {
		_x = x;
		_y = y;
		_z = z;
	}
	
	public static SPVector from_jsonobject(JSON o) {
		return new SPVector(o.ToFloat("x"),o.ToFloat("y"),o.ToFloat("z"));
	}
	
	public static JSON to_jsonobj(SPVector v) {
		JSON rtv = new JSON();
		rtv.fields["x"] = v._x;
		rtv.fields["y"] = v._y;
		rtv.fields["z"] = v._z;
		return rtv;
	}
}

public class SPPlayerObject {
	public int _id;
	public string _name;
	public SPVector _pos;
	public SPVector _vel;
	public SPVector _rot;
	
	public int __timeout;
}

