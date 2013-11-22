using UnityEngine;
using System;
using System.Timers;

public class Player {
	public static Player instance;

	public GameObject _player_object { get; set;}
	public int _id {get; set;}
	public string _name {get; set;}
	public Vector3 _pos { get; set; }
	public Vector3 _vel { get; set; }
	public Vector3 _rotation { get; set; }
	public int _alive { get; set; }
		private Timer timer;

	public Player(SPPlayerObject player_message) {
		instance = this;
		this._id = player_message._id;
		this._name = player_message._name;
		this._pos = player_message._pos;
		this._vel = player_message._vel;
		this._rotation = player_message._rot;
		this._alive = player_message._alive;			                                   
	}
	

	public int get_id() {
		return this._id;
	}

	public GameObject get_player_object() {
		return _player_object;
	}

	public void set_player_object(GameObject player_object) {
		this._player_object = player_object;
	}

	public void update() {
		MotionSimulatedPlayer player_component = _player_object.GetComponent<MotionSimulatedPlayer> ();

		player_component._pos.x = _pos.x;
		player_component._pos.y = _pos.y;
		player_component._pos.z = _pos.z;

		player_component._vel.x = _vel.x;
		player_component._vel.y = _vel.y;
		player_component._vel.z = _vel.z;

		player_component._rotation.x = _rotation.x;
		player_component._rotation.y = _rotation.y;
		player_component._rotation.z = _rotation.z;
	}

	public void respawn(){
				Debug.Log ("Starting respawn timer...");
				timer = new Timer ();

				timer.Elapsed += new ElapsedEventHandler (spawn);
				timer.Interval = 5000;
				timer.Enabled = true;
				timer.Start ();
	}

		void spawn(object sender, EventArgs e){
			this._alive = 1;
			this._pos = new Vector3 (0.0f, 0.0f, 0.0f);
			timer.Stop();			
	}

	public SPPlayerObject convert_to_obj(){
			SPPlayerObject myObject = new SPPlayerObject();
			myObject._id = _id;
			myObject._name = _name;
			myObject._pos = _pos;
			myObject._vel = _vel;
			myObject._rot = _rotation;
			myObject._alive = _alive;

			return myObject;
	}
}
