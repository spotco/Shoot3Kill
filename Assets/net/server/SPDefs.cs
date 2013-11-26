using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Collections.Generic;

public class SPBulletObject {
	public int _id;
	public int _playerid;
	public SPVector _pos  = new SPVector(0,0,0);
	public SPVector _vel  = new SPVector(0,0,0);
	public SPVector _rot  = new SPVector(0,0,0);

	public SPBulletObject(int id, int playerid, SPVector pos, SPVector vel, SPVector rot) {
		_id = id;
		_playerid = playerid;
		_pos = pos;
		_vel = vel;
		_rot = rot;
	}

	public JSONObject to_json() {
		JSONObject rtv = new JSONObject();
		rtv.Add("id",_id);
		rtv.Add("player_id",_playerid);
		rtv.Add("pos",_pos.to_json());
		rtv.Add("vel",_vel.to_json());
		rtv.Add("rot",_rot.to_json());
		return rtv;
	}
	
	public static SPBulletObject from_json(JSONObject jso) {
		SPBulletObject rtv = new SPBulletObject(0,0,new SPVector(0,0,0),new SPVector(0,0,0),new SPVector(0,0,0));
		rtv._id = (int)jso.GetNumber("id");
		rtv._playerid = (int)jso.GetNumber("player_id");
		rtv._pos = SPVector.from_json(jso.GetObject("pos"));
		rtv._vel = SPVector.from_json(jso.GetObject("vel"));
		rtv._rot = SPVector.from_json(jso.GetObject("rot"));
		return rtv;
	}
}

public class SPEvent {
	public static int TYPE_ALLOC_ID = 0;
	public static int TYPE_KILL = 1;

	public int _type;
	public int _player_id;
	public int _value;
	public SPEvent(int type, int player_id,int v) {
		_type = type;
		_player_id = player_id;
		_value = v;
	}

	public JSONObject to_json() {
		JSONObject rtv = new JSONObject();
		rtv.Add("type",_type);
		rtv.Add("player_id",_player_id);
		rtv.Add("value",_value);
		return rtv;
	}

	public static SPEvent from_json(JSONObject jso) {
		SPEvent evt = new SPEvent(0,0,0);
		evt._type = (int)jso.GetNumber("type");
		evt._player_id = (int)jso.GetNumber("player_id");
		evt._value = (int)jso.GetNumber("value");
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
		rtv.Add("x",_x);
		rtv.Add("y",_y);
		rtv.Add("z",_z);
		return rtv;
	}
	
	public static SPVector from_json(JSONObject jso) {
		SPVector rtv = new SPVector(
			(float)jso.GetNumber("x"),
			(float)jso.GetNumber("y"),
			(float)jso.GetNumber("z")
		);
		return rtv;
	}
}

public class SPPlayerObject {
	public int _id;
	public string _name = "";
	public SPVector _pos = new SPVector(0,0,0);
	public SPVector _vel = new SPVector(0,0,0);
	public SPVector _rot = new SPVector(0,0,0);

	public int __timeout;

	public SPPlayerObject(int id, string name, SPVector pos, SPVector vel, SPVector rot) {
		_id = id;
		_name = name;
		_pos = pos;
		_vel = vel;
		_rot = rot;
	}

	public JSONObject to_json() {
		JSONObject rtv = new JSONObject();
		rtv.Add("id",_id);
		rtv.Add("name",_name);
		rtv.Add("pos",_pos.to_json());
		rtv.Add("vel",_vel.to_json());
		rtv.Add("rot",_rot.to_json());
		return rtv;
	}

	public static SPPlayerObject from_json(JSONObject jso) {
		SPPlayerObject rtv = new SPPlayerObject(0,"",new SPVector(0,0,0),new SPVector(0,0,0),new SPVector(0,0,0));
		rtv._id = (int)jso.GetNumber("id");
		rtv._name = jso.GetString("name");
		rtv._pos = SPVector.from_json(jso.GetObject("pos"));
		rtv._vel = SPVector.from_json(jso.GetObject("vel"));
		rtv._rot = SPVector.from_json(jso.GetObject("rot"));
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
		rtv.Add("players",players);

		JSONArray bullets = new JSONArray();
		foreach(SPBulletObject o in _bullets) {
			bullets.Add(o.to_json());
		}
		rtv.Add("bullets",bullets);

		JSONArray events = new JSONArray();
		foreach(SPEvent o in _events) {
			events.Add(o.to_json());
		}
		rtv.Add("events",events);

		return rtv;
	}

	public static SPServerMessage from_json(JSONObject jso) {
		SPServerMessage rtv = new SPServerMessage();

		JSONArray players = jso.GetArray("players");
		foreach(JSONValue v in players) {
			rtv._players.Add(SPPlayerObject.from_json(v.Obj));
		}

		JSONArray bullets = jso.GetArray("bullets");
		foreach(JSONValue v in bullets) {
			rtv._bullets.Add(SPBulletObject.from_json(v.Obj));
		}

		JSONArray events = jso.GetArray("events");
		foreach(JSONValue v in events) {
			rtv._events.Add(SPEvent.from_json(v.Obj));
		}

		return rtv;
	}
}

public class SPClientMessage {

}

