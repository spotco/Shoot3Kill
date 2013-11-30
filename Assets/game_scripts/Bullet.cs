﻿using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour {
	
	public int _id;
	public int _playerid;
	public Vector3 _position;
	public Vector3 _vel;
	public GameObject _obj;
	
	public int __ct = 0;
	
	public void start(Vector3 position, Vector3 vel,GameObject obj, int id, int playerid) {
		_position = position;
		_playerid = playerid;
		_vel = vel;
		_obj = obj; 
		_id = id;
		__ct = 50;
		_obj.transform.position = position;
		_obj.transform.forward = _vel;
	}
	
	void Update() {
		_position.x += _vel.x;
		_position.y += _vel.y;
		_position.z += _vel.z;
		_obj.transform.position = _position;
		__ct--;
	}
	
	public bool should_remove() {
		return __ct <= 0;	
	}
	
	public void do_remove() {
		GameObject.Destroy(_obj);
		_obj = null;
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.GetComponent<PlayerControl>() == null && col.gameObject.GetComponent<Bullet>() == null) {
			__ct = 0;
		}
	}
	
}