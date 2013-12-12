using UnityEngine;
using System.Collections.Generic;

public class OnlinePlayer : MonoBehaviour {
	Vector3 _pos;
	Vector3 _vel;
	Vector3 _rot;
	int _ct;

	TextMesh _name_plate;

	void Start() {
		_name_plate = Util.FindInHierarchy(gameObject,"NameText").GetComponent<TextMesh>();
	}
	
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

		if (obj._alive == 1) {
			gameObject.SetActive(true);
		} else {
			gameObject.SetActive(false);
		}

		if (obj._name == null) obj._name = "Anonymous";
		if (_name_plate != null) _name_plate.text = obj._name;
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