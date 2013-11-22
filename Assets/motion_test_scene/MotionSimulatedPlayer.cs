using UnityEngine;
using System.Collections;

public class MotionSimulatedPlayer : MonoBehaviour {

		public Vector3 _pos;
		public Vector3 _vel;
		public Vector3 _rotation;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
				_pos.x += _vel.x;
				_pos.y += _vel.y;
				_pos.z += _vel.z;

				this.gameObject.transform.position = _pos;
				this.gameObject.transform.eulerAngles = _rotation;
	}
}
