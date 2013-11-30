using UnityEngine;
using System.Collections.Generic;

public class OnlinePlayerManager : MonoBehaviour {

	public static OnlinePlayerManager inst;

	void Start () {
		inst = this;
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
