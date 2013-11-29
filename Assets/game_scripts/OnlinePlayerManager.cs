using UnityEngine;
using System.Collections.Generic;

public class OnlinePlayerManager : MonoBehaviour {

	public static OnlinePlayerManager instance;

	void Start () {
		instance = this;
	}

	Dictionary<int,OnlinePlayer> _id_to_onlineplayer = new Dictionary<int, OnlinePlayer>();

	public void msg_recieved(SPServerMessage msg) {
		foreach(SPPlayerObject obj in msg._players) {
			if (obj._id == PlayerInfo._id) continue;
			
			if (!_id_to_onlineplayer.ContainsKey(obj._id)) {
				GameObject player_gameobj = (GameObject)Instantiate(Resources.Load("onlineplayer"));
				player_gameobj.AddComponent<OnlinePlayer>();
				_id_to_onlineplayer[obj._id] = player_gameobj.GetComponent<OnlinePlayer>();
				_id_to_onlineplayer[obj._id].init();
			}
			
			_id_to_onlineplayer[obj._id].msg_recieved(obj);
		}
	}

	List<int> _to_remove = new List<int>();
	void Update () {

		foreach(int id in _id_to_onlineplayer.Keys) {
			OnlinePlayer p = _id_to_onlineplayer[id];
			if (p.should_remove()) {
				_to_remove.Add(id);
			}
		}

		foreach(int id in _to_remove) {
			OnlinePlayer p = _id_to_onlineplayer[id];
			_id_to_onlineplayer.Remove(id);
			p.do_remove();
			_id_to_onlineplayer.Remove(id);
		}
		_to_remove.Clear();
	}
}

class OnlinePlayer : MonoBehaviour {
	Vector3 _pos;
	Vector3 _vel;
	Vector3 _rot;
	int _ct;

	public void init() {
		_ct = 200;
	}

	public void msg_recieved(SPPlayerObject obj) {
		_ct = 200;
		_pos.x = obj._pos._x;
		_pos.y = obj._pos._y;
		_pos.z = obj._pos._z;

		_vel.x = obj._vel._x;
		_vel.y = obj._vel._y;
		_vel.z = obj._vel._z;

		_rot.x = obj._rot._x;
		_rot.y = obj._rot._y;
		_rot.z = obj._rot._z;

	}

	void Update() {
		_ct--;

		_pos.x += _vel.x;
		_pos.y += _vel.y;
		_pos.z += _vel.z;

		_vel.x *= 0.99f;
		_vel.z *= 0.99f;
		if (_vel.y > 0) _vel.y -= 0.01f; 

		gameObject.transform.position = _pos;
		gameObject.transform.eulerAngles = _rot;
	}

	public bool should_remove() {
		return _ct <= 0;
	}

	public void do_remove() {
		Destroy(gameObject);
	}
}
