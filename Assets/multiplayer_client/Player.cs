using UnityEngine;

public class Player {
	public static Player instance;

	GameObject _player_object;
	int _id;
	string _name;
	public Vector3 _pos { get; set; }
	public Vector3 _vel { get; set; }
	public Vector3 _rotation { get; set; }
	public int _alive { get; set; }
	private int timer_count;
	private int time_to_respawn;

	public Player(SPPlayerObject player_message) {
		this.instance = this;
		this._id = player_message._id;
		this._name = player_message._name;
		this._pos = player_message._pos;
		this._vel = player_message._vel;
		this._rotation = player_message._rot;
		this._alive = player_message._alive;
		this.timer_count = 0;		
		this.time_to_respawn = 5;				                                   
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
		if (_alive == 0) {
			timer_count++;
			if (timer_count % 50 == 0) { // A second has passed
				this.time_to_respawn--;
				if (this.time_to_respawn == 0) {
					this._alive = 1;
					this._pos = new Vector3 (0.0, 0.0, 0.0);
					this.timer_count = 0;		
					this.time_to_respawn = 5;		
				}
			}
		}
	}
}
