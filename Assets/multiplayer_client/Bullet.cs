﻿using UnityEngine;

public class Bullet{
	GameObject _bullet_object;
	int _bul_id;
	int _player_id;
	public Vector3 _bul_pos{ get; set;}
	public Vector3 _bul_vel{ get; set;}
	public Vector3 _bul_rotation{ get; set;}

	public Bullet(SPBulletObject bullet_message) {
		this._bul_id = bullet_message._id;
		this._player_id = bullet_message._playerid;
		this._bul_pos = bullet_message._pos;
		this._bul_vel = bullet_message._vel;
		this._bul_rotation = bullet_message._rot;
	}

	public bool is_out_of_bounds(){
		float x = _bul_pos.x;
		float y = _bul_pos.y;
		float z = _bul_pos.z;
		float oob = 10000f;

		if((x > oob || x < -oob) || (y > oob || y < -oob) || (z > oob || z < -oob)){
			return true;
		}
		return false;
	}

	public Vector3 get_bullet_pos(){
		return _bul_pos;
	}

	public GameObject get_bullet_object(){
		return _bullet_object;
	}

	public void set_bullet_object(GameObject bullet_object){
		this._bullet_object = bullet_object;
	}
}