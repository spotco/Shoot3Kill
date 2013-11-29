using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Collections.Generic;

public class SN {
	public static string ID = "i";
	public static string PLAYER_ID = "p";
	public static string POS = "o";
	public static string VEL = "v";
	public static string ROT = "r";
	public static string TYPE = "t";
	public static string VALUE = "a";
	public static string X = "x";
	public static string Y = "y";
	public static string Z = "z";
	public static string BULLETS = "b";
	public static string PLAYERS = "l";
	public static string PLAYER = "q";
	public static string EVENTS = "e";
	public static string NAME = "n";
}

public class SPBulletObject {
	public int _id;
	public int _playerid;
	public SPVector _pos  = new SPVector(0,0,0);
	public SPVector _vel  = new SPVector(0,0,0);
	public SPVector _rot  = new SPVector(0,0,0);

	public int __timeout = 20;

	public SPBulletObject(){}

	public SPBulletObject(int id, int playerid, SPVector pos, SPVector vel, SPVector rot) {
		_id = id;
		_playerid = playerid;
		_pos = pos;
		_vel = vel;
		_rot = rot;
	}

	public JSONObject to_json() {
		JSONObject rtv = new JSONObject();
		rtv.Add(SN.ID,_id);
		rtv.Add(SN.PLAYER_ID,_playerid);
		rtv.Add(SN.POS,_pos.to_json());
		rtv.Add(SN.VEL,_vel.to_json());
		rtv.Add(SN.ROT,_rot.to_json());
		return rtv;
	}
	
	public static SPBulletObject from_json(JSONObject jso) {
		SPBulletObject rtv = new SPBulletObject();
		rtv._id = (int)jso.GetNumber(SN.ID);
		rtv._playerid = (int)jso.GetNumber(SN.PLAYER_ID);
		rtv._pos = SPVector.from_json(jso.GetObject(SN.POS));
		rtv._vel = SPVector.from_json(jso.GetObject(SN.VEL));
		rtv._rot = SPVector.from_json(jso.GetObject(SN.ROT));
		return rtv;
	}

	public string unique_key() {
		return _playerid + "_" + _id;
	}
}

public class SPEvent {
	public static int TYPE_ALLOC_ID = 0;
	public static int TYPE_KILL = 1;

	public int _type;
	public int _player_id;
	public int _value;

	public int __duration = 9999;

	public SPEvent(int type, int player_id,int v) {
		_type = type;
		_player_id = player_id;
		_value = v;
	}

	public JSONObject to_json() {
		JSONObject rtv = new JSONObject();
		rtv.Add(SN.TYPE,_type);
		rtv.Add(SN.PLAYER_ID,_player_id);
		rtv.Add(SN.VALUE,_value);
		return rtv;
	}

	public static SPEvent from_json(JSONObject jso) {
		SPEvent evt = new SPEvent(0,0,0);
		evt._type = (int)jso.GetNumber(SN.TYPE);
		evt._player_id = (int)jso.GetNumber(SN.PLAYER_ID);
		evt._value = (int)jso.GetNumber(SN.VALUE);
		return evt;
	}
}

public class SPVector {
	public float _x,_y,_z;
	public SPVector(float x, float y, float z) {
		_x = x;
		_y = y;
		_z = z;
	}
	public JSONObject to_json() {
		JSONObject rtv = new JSONObject();
		rtv.Add(SN.X,_x);
		rtv.Add(SN.Y,_y);
		rtv.Add(SN.Z,_z);
		return rtv;
	}
	
	public static SPVector from_json(JSONObject jso) {
		SPVector rtv = new SPVector(
			(float)jso.GetNumber(SN.X),
			(float)jso.GetNumber(SN.Y),
			(float)jso.GetNumber(SN.Z)
		);
		return rtv;
	}

	public SPVector copy() {
		return new SPVector(_x,_y,_z);
	}
}

public class SPPlayerObject {
	public int _id;
	public string _name = "";
	public SPVector _pos = new SPVector(0,0,0);
	public SPVector _vel = new SPVector(0,0,0);
	public SPVector _rot = new SPVector(0,0,0);

	public int __timeout = 50;

	public SPPlayerObject(int id, string name, SPVector pos, SPVector vel, SPVector rot) {
		_id = id;
		_name = name;
		_pos = pos;
		_vel = vel;
		_rot = rot;
	}

	public JSONObject to_json() {
		JSONObject rtv = new JSONObject();
		rtv.Add(SN.ID,_id);
		rtv.Add(SN.NAME,_name);
		rtv.Add(SN.POS,_pos.to_json());
		rtv.Add(SN.VEL,_vel.to_json());
		rtv.Add(SN.ROT,_rot.to_json());
		return rtv;
	}

	public static SPPlayerObject from_json(JSONObject jso) {
		SPPlayerObject rtv = new SPPlayerObject(0,"",new SPVector(0,0,0),new SPVector(0,0,0),new SPVector(0,0,0));
		rtv._id = (int)jso.GetNumber(SN.ID);
		rtv._name = jso.GetString(SN.NAME);
		rtv._pos = SPVector.from_json(jso.GetObject(SN.POS));
		rtv._vel = SPVector.from_json(jso.GetObject(SN.VEL));
		rtv._rot = SPVector.from_json(jso.GetObject(SN.ROT));
		return rtv;
	}
}

public class SPServerMessage {
	public List<SPPlayerObject> _players = new List<SPPlayerObject>();
	public List<SPBulletObject> _bullets = new List<SPBulletObject>();
	public List<SPEvent> _events = new List<SPEvent>();

	public JSONObject to_json() {
		JSONObject rtv = new JSONObject();

		JSONArray players = new JSONArray();
		foreach(SPPlayerObject o in _players) {
			players.Add(o.to_json());
		}
		rtv.Add(SN.PLAYERS,players);

		JSONArray bullets = new JSONArray();
		foreach(SPBulletObject o in _bullets) {
			bullets.Add(o.to_json());
		}
		rtv.Add(SN.BULLETS,bullets);

		JSONArray events = new JSONArray();
		foreach(SPEvent o in _events) {
			events.Add(o.to_json());
		}
		rtv.Add(SN.EVENTS,events);

		return rtv;
	}

	public static SPServerMessage from_json(JSONObject jso) {
		SPServerMessage rtv = new SPServerMessage();

		JSONArray players = jso.GetArray(SN.PLAYERS);
		foreach(JSONValue v in players) {
			rtv._players.Add(SPPlayerObject.from_json(v.Obj));
		}

		JSONArray bullets = jso.GetArray(SN.BULLETS);
		foreach(JSONValue v in bullets) {
			rtv._bullets.Add(SPBulletObject.from_json(v.Obj));
		}

		JSONArray events = jso.GetArray(SN.EVENTS);
		foreach(JSONValue v in events) {
			rtv._events.Add(SPEvent.from_json(v.Obj));
		}

		return rtv;
	}
}

public class SPClientMessage {
	public List<SPBulletObject> _bullets = new List<SPBulletObject>();
	public SPPlayerObject _player = new SPPlayerObject(0,"",new SPVector(0,0,0),new SPVector(0,0,0),new SPVector(0,0,0));

	public JSONObject to_json() {
		JSONObject rtv = new JSONObject();
		
		JSONArray bullets = new JSONArray();
		foreach(SPBulletObject o in _bullets) {
			bullets.Add(o.to_json());
		}
		rtv.Add(SN.BULLETS,bullets);

		rtv.Add(SN.PLAYER,_player.to_json());
		return rtv;
	}

	public static SPClientMessage from_json(JSONObject jso) {
		SPClientMessage rtv = new SPClientMessage();
		
		JSONArray bullets = jso.GetArray(SN.BULLETS);
		foreach(JSONValue v in bullets) {
			rtv._bullets.Add(SPBulletObject.from_json(v.Obj));
		}

		rtv._player = SPPlayerObject.from_json(jso.GetObject(SN.PLAYER));
		
		return rtv;
	}

}

