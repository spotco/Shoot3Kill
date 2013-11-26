using UnityEngine;
using System.Collections;
using System;


[System.Serializable]
public class PlayerControl : MonoBehaviour {
	
	public static PlayerControl instance;
	
	public Rigidbody _body;
	public float JUMP_FORCE = 220f;
	public float MOVE_SPEED = 4f;

	public int _jump_cooldown;
	public int _move_cooldown;
	public Vector3 _ground_normal;
	
	public Transform _camera_transform;


	void Start () {
		_body = gameObject.GetComponent<Rigidbody>();
		_body.freezeRotation = true;
		instance = this;
		
		_bullet_cooldown = 0;
		
		_camera_transform = Util.FindInHierarchy(gameObject,"Main Camera").transform;
	}
	
	int _bullet_cooldown = 0;
	void Update () {
		if (Input.GetKey(KeyCode.P)) {
			GameObject maincam = Util.FindInHierarchy(gameObject,"Main Camera");
			GameObject vrcam = Util.FindInHierarchy(gameObject,"OVRCameraController");
			maincam.SetActive(true);
			vrcam.SetActive(false);
		} else if (Input.GetKey(KeyCode.O)) {
			GameObject maincam = Util.FindInHierarchy(gameObject,"Main Camera");
			GameObject vrcam = Util.FindInHierarchy(gameObject,"OVRCameraController");
			maincam.SetActive(false);
			vrcam.SetActive(true);	
		}
		
		
		if (Input.mousePosition.x > Screen.width || Input.mousePosition.y > Screen.height) return;
		
		if (Input.GetMouseButton(0) && _bullet_cooldown <= 0) {
			BulletManager.instance.add_bullet(_camera_transform.position,_camera_transform.forward);
			EffectManager.instance.add_effect((new Effect("Sparks",Util.vector_add(_camera_transform.position,_camera_transform.forward),20)).set_rotation(gameObject.transform.eulerAngles));
		}
		
		
		if (on_ground() && _jump_cooldown == 0 && Input.GetKey(KeyCode.Space)) {
			_jump_cooldown = 20;
			_move_cooldown = 20;
			Vector3 jump_dir = new Vector3(0,1,0);
			jump_dir.Normalize();
			jump_dir.Scale(new Vector3(JUMP_FORCE,JUMP_FORCE,JUMP_FORCE));
			_body.AddForce(jump_dir);
		}

		if (_jump_cooldown > 0) _jump_cooldown--;
		if (_move_cooldown > 0) _move_cooldown--;


		Vector3 neu_vel = _body.velocity;
		if (_move_cooldown <= 0 && on_ground()) {
			Vector3 forward = gameObject.transform.forward;
			forward.y = 0;
			forward.Normalize();

			bool move_ws = false;
			Vector3 ws_v = forward;
			ws_v = Util.vector_scale(ws_v,MOVE_SPEED);
			if (Input.GetKey(KeyCode.W)) {
				move_ws = true;
			} else if (Input.GetKey(KeyCode.S)) {
				move_ws = true;
				ws_v = Util.vector_scale(ws_v,-1);
			}

			bool move_ad = false;
			Vector3 ad_v = Util.vec_cross(forward,new Vector3(0,1,0));
			ad_v.Normalize();
			ad_v = Util.vector_scale(ad_v,MOVE_SPEED);

			if (Input.GetKey(KeyCode.A)) {
				move_ad = true;
			} else if (Input.GetKey(KeyCode.D)) {
				move_ad = true;
				ad_v = Util.vector_scale(ad_v,-1);
			}

			if (move_ws && move_ad) {
				neu_vel = Util.vector_add(ws_v,ad_v);
			} else if (move_ws) {
				neu_vel = ws_v;
			} else if (move_ad) {
				neu_vel = ad_v;
			}

		}

		if (on_ground()) {
			neu_vel = Util.vector_scale(neu_vel,0.9f);
		}

		if (Math.Abs(neu_vel.x) < 0.2f) neu_vel.x = 0;
		if (Math.Abs(neu_vel.z) < 0.2f) neu_vel.z = 0;
		_body.velocity = neu_vel;

		if (Input.GetKey(KeyCode.R)) {
			gameObject.transform.position = Vector3.zero;
			_body.velocity = Vector3.zero;
		}

		fps_turn();
	}

	Vector3 _last_mouse_position;
	Vector3 _xy_angle = Vector3.zero;
	static float MAX_X_ANGLE = 45;
	static float FPS_LOOK_SCALE = 0.3f;

	void fps_turn() {
		if (!Input.mousePresent) return;
		Screen.showCursor = false;

		Vector3 mouse_delta = new Vector3(Input.mousePosition.x - _last_mouse_position.x, Input.mousePosition.y - _last_mouse_position.y);
		_last_mouse_position = Input.mousePosition;

		_xy_angle.y += mouse_delta.x * FPS_LOOK_SCALE;
		_xy_angle.x -= mouse_delta.y * FPS_LOOK_SCALE;

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
		_collisionct++;
	}

	void OnCollisionExit(Collision col) {
		_ground_normal = new Vector3(0,1,0);
		_collisionct--;
	}
}