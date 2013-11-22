using UnityEngine;
using System;
using System.Timers;

public class Player {
	public static Player instance;

	public GameObject _player_object { get; set;}
	public int _id {get; set;}
	public string _name {get; set;}
	public SPVector _pos { get; set; }
	public SPVector _vel { get; set; }
	public SPVector _rotation { get; set; }
	public int _alive { get; set; }
	private Timer timer;

	public Player(SPPlayerObject player_message) {
		instance = this;
		this._id = player_message._id;
		this._name = player_message._name;
		this._pos = player_message._pos;
		this._vel = player_message._vel;
		this._rotation = player_message._rot;
		
		//_player_object.transform.position = new Vector3(_pos._x,_pos._y,_pos._z);
	}
	

	public int get_id() {
		return this._id;
	}

	public GameObject get_player_object() {
		return _player_object;
	}

	public void Update() {
		Vector3 pos = new Vector3(_pos._x,_pos._y,_pos._z);
		pos.x += _vel._x;
		pos.y += _vel._y;
		pos.z += _vel._z;
		_player_object.transform.position = pos;
		_pos = new SPVector(pos.x,pos.y,pos.z);
		
		Debug.Log(pos);
	}
	
	public SPPlayerObject convert_to_obj(){
			SPPlayerObject myObject = new SPPlayerObject();
			myObject._id = _id;
			myObject._name = _name;
			myObject._pos = _pos;
			myObject._vel = _vel;
			myObject._rot = _rotation;
			//myObject._alive = _alive;

			return myObject;
	}
}
