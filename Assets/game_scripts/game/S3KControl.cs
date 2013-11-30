using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class S3KControl : MonoBehaviour {

	public static S3KControl inst;

	public static float JUMP_FORCE = 220f;
	public static float MOVE_SPEED = 4f;

	public Rigidbody _body;
	public GameObject _character_model;
	public Vector3 _last_body_position;
	public bool _has_last_position = false; 

	void Start () {
		inst = this;
		_body = gameObject.GetComponent<Rigidbody>();
		_character_model = Util.FindInHierarchy(gameObject,"CharacterModel");
		_body.freezeRotation = true;
		_bullet_cooldown = 0;
	}

	bool _mouse_centered = true;

	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) _mouse_centered = !_mouse_centered;
		if (_mouse_centered) {
			Screen.showCursor = false;
			Screen.lockCursor = true;
		} else {
			Screen.showCursor = true;
			Screen.lockCursor = false;
		}

		if (PlayerInfo._alive) {
			_character_model.SetActive(true);
			if (Input.GetKey(KeyCode.R)) {
				gameObject.transform.position = Vector3.zero;
				_body.velocity = Vector3.zero;
			}
			
			click_shoot();
			jump();
			move();
			if (_mouse_centered) {
				fps_turn();
			}

			_last_body_position = gameObject.transform.position;
			_has_last_position = true;

		} else {
			_character_model.SetActive(false);
			_has_last_position = false;
		}
	}

	int _bullet_cooldown = 0;

	void click_shoot() {
		_bullet_cooldown--;
		if ( Input.GetMouseButton(0)  && _bullet_cooldown <= 0 ) {
			Vector3 bullet_vel = S3KCamera.inst._fps_clocator.transform.forward;
			bullet_vel.Normalize();
			bullet_vel = Util.vec_scale(bullet_vel,0.25f);
			BulletManager.inst.add_bullet(
				S3KCamera.inst._fps_clocator.transform.position,
				bullet_vel
			);
			
			_bullet_cooldown = 4;
			EffectManager.inst.add_effect(
				(new Effect(
					"Sparks",
					Util.vec_add(
						S3KCamera.inst._fps_clocator.transform.position,
						S3KCamera.inst._fps_clocator.transform.forward.normalized),
					20
				)).set_rotation(gameObject.transform.eulerAngles)
			);
		}
	}

	public int _jump_cooldown;
	public int _move_cooldown;
	public Vector3 _ground_normal;

	void jump() {
		if (on_ground() && _jump_cooldown == 0 && Input.GetKey(KeyCode.Space)) {
			_jump_cooldown = 20;
			_move_cooldown = 20;
			Vector3 jump_dir = _ground_normal;
			jump_dir.Normalize();
			jump_dir.Scale(new Vector3(JUMP_FORCE,JUMP_FORCE,JUMP_FORCE));
			_body.AddForce(jump_dir);
		}
		
		if (_jump_cooldown > 0) _jump_cooldown--;
		if (_move_cooldown > 0) _move_cooldown--;
	}

	void move() {
		Vector3 neu_vel = _body.velocity;
		if (_move_cooldown <= 0 && on_ground()) {
			Vector3 forward = gameObject.transform.forward;
			forward.y = 0;
			forward.Normalize();
			
			bool move_ws = false;
			Vector3 ws_v = forward;
			ws_v = Util.vec_scale(ws_v,MOVE_SPEED);
			if (Input.GetKey(KeyCode.W)) {
				move_ws = true;
			} else if (Input.GetKey(KeyCode.S)) {
				move_ws = true;
				ws_v = Util.vec_scale(ws_v,-1);
			}
			
			bool move_ad = false;
			Vector3 ad_v = Util.vec_cross(forward,new Vector3(0,1,0));
			ad_v.Normalize();
			ad_v = Util.vec_scale(ad_v,MOVE_SPEED);
			
			if (Input.GetKey(KeyCode.A)) {
				move_ad = true;
			} else if (Input.GetKey(KeyCode.D)) {
				move_ad = true;
				ad_v = Util.vec_scale(ad_v,-1);
			}
			
			if (move_ws && move_ad) {
				neu_vel = Util.vec_add(ws_v,ad_v);
			} else if (move_ws) {
				neu_vel = ws_v;
			} else if (move_ad) {
				neu_vel = ad_v;
			}
		}
		
		if (on_ground()) {
			neu_vel = Util.vec_scale(neu_vel,0.9f);
		}
		
		if (Math.Abs(neu_vel.x) < 0.2f) neu_vel.x = 0;
		if (Math.Abs(neu_vel.z) < 0.2f) neu_vel.z = 0;
		_body.velocity = neu_vel;
	}
	
	Vector3 _xy_angle = Vector3.zero;
	static float MAX_X_ANGLE = 45;
	static float FPS_LOOK_SCALE = 2.5f;
	
	void fps_turn() {
		if (!Input.mousePresent) {
			Screen.showCursor = true;
			Screen.lockCursor = false;
			return;
		}
		
		_xy_angle.y += Input.GetAxis("Mouse X") * FPS_LOOK_SCALE;
		_xy_angle.x -= Input.GetAxis("Mouse Y") * FPS_LOOK_SCALE;
		
		if (Math.Abs(_xy_angle.x) > MAX_X_ANGLE) {
			_xy_angle.x = Util.sig(_xy_angle.x) * MAX_X_ANGLE;
		}
		
		gameObject.transform.rotation = Quaternion.Euler(_xy_angle);
	}
	
	public int _collisionct = 0;
	
	public bool on_ground() {
		return _collisionct > 0;
	}
	
	void OnCollisionEnter(Collision col) {
		ContactPoint contact = col.contacts[0];
		_ground_normal = contact.normal;
		Vector3 vel = _body.velocity;
		_body.velocity = vel;
		_ground_normal.Normalize();
		_collisionct++;
	}
	
	void OnCollisionExit(Collision col) {
		_ground_normal = new Vector3(0,1,0);
		_collisionct--;
	}
}
