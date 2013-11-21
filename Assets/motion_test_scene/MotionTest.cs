using UnityEngine;
using System.Collections;


public class MotionTest : MonoBehaviour {

	Vector3 _pos;
	Vector3 _vel;
	Vector3 _rot;
	float theta = 0;

	void Start () {
		_pos = Vector3.zero;
		_vel = new Vector3(0.1f,0,0);
		_rot = new Vector3(1.0f,0,0);
	}

	int _ct = 0;

	void Update () {
		_ct++;
		if (_ct%10==0) {
			parse_server_json(generate_json());
		}

		theta += 0.01f;
		_pos.x += _vel.x;
		_pos.y += _vel.y;
		_pos.z += _vel.z;
		_vel.x = Mathf.Sin(theta);
	}


	string generate_json() {
		return "[{'pos':[0,0,0],'vel':[0,0,0],'rot':[0,0,0],'id':1}]";
	}


	void parse_server_json(string json) {

	}
}
