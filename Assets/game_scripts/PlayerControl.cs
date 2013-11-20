using UnityEngine;
using System.Collections;
using System;


[System.Serializable]
public class PlayerControl : MonoBehaviour {

	Rigidbody _body;
	public Transform _model_transform;
	public float JUMP_FORCE = 225;
	public float MOVE_SPEED = 4;

	public int _jump_count;
	public int _jump_cooldown;
	public int _move_cooldown;
	public bool _on_ground;
	public Vector3 _ground_normal;

	void Start () {
		_body = gameObject.GetComponent<Rigidbody>();
		_body.freezeRotation = true;
		_jump_count = 0;
		_on_ground = false;
		_model_transform = Util.FindInHierarchy(gameObject,"test_player").transform;
	}

	void Update () {
		if (_jump_count > 0 && _jump_cooldown <= 0 && Input.GetKey(KeyCode.Space)) {
			_jump_count--;
			_jump_cooldown = 20;
			_move_cooldown = 20;
			Vector3 jump_dir = new Vector3(0,1,0);

			if (_on_ground) {
				jump_dir = _ground_normal;
			}
			jump_dir.Normalize();
			jump_dir.Scale(new Vector3(JUMP_FORCE,JUMP_FORCE,JUMP_FORCE));

			_body.AddForce(jump_dir);
		}

		if (_jump_cooldown > 0) _jump_cooldown--;
		if (_move_cooldown > 0) _move_cooldown--;

		if (_move_cooldown <= 0) {
			Vector3 v = _body.velocity;
			Vector3 facing_dir = Vector3.zero;
			if (Input.GetKey(KeyCode.W) && _ground_normal.z != -1.0f) {
				v.z = MOVE_SPEED;
				facing_dir.z = 1;
			} else if (Input.GetKey(KeyCode.S) && _ground_normal.z != 1.0f) {
				v.z = -MOVE_SPEED;
				facing_dir.z = -1;
			}

			if (Input.GetKey(KeyCode.A) && _ground_normal.x != 1.0f) {
				v.x = -MOVE_SPEED;
				facing_dir.x = -1;
			} else if (Input.GetKey(KeyCode.D) && _ground_normal.x != -1.0f) {
				v.x = MOVE_SPEED;
				facing_dir.x = 1;
			}
			_body.velocity = v;
			if (facing_dir.magnitude != 0) {
				Vector3 body_rotation = gameObject.transform.localEulerAngles;
				body_rotation.y = Mathf.Atan2(-facing_dir.z,facing_dir.x) * Util.rad2deg - 90;
				gameObject.transform.localEulerAngles = body_rotation;
			}
		}



		if (Input.GetKey(KeyCode.R)) {
			gameObject.transform.position = Vector3.zero;
			_body.velocity = Vector3.zero;
		}
	}

	void OnCollisionEnter(Collision col) {
		_jump_count = 2;

		ContactPoint contact = col.contacts[0];
		_ground_normal = contact.normal;
		Vector3 vel = _body.velocity;
		if (Math.Abs(_ground_normal.x) > Math.Abs(_ground_normal.y) && Math.Abs(_ground_normal.x) > Math.Abs(_ground_normal.z)) {
			_ground_normal.y = 0;
			_ground_normal.z = 0;
			vel.x = 0;
		}
		if (Math.Abs(_ground_normal.y) > Math.Abs(_ground_normal.x) && Math.Abs(_ground_normal.y) > Math.Abs(_ground_normal.z)) {
			_ground_normal.x = 0;
			_ground_normal.z = 0;
			vel.y = 0;
		}
		if (Math.Abs(_ground_normal.z) > Math.Abs(_ground_normal.y) && Math.Abs(_ground_normal.z) > Math.Abs(_ground_normal.x)) {
			_ground_normal.x = 0;
			_ground_normal.y = 0;
			vel.z = 0;
		}
		_body.velocity = vel;
		_ground_normal.Normalize();
		_on_ground = true;
	}

	void OnCollisionExit(Collision col) {
		_on_ground = false;
		_ground_normal = Vector3.zero;
	}
}
