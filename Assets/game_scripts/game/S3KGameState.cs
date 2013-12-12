using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class S3KGameState : MonoBehaviour {

	public static S3KGameState inst;

	void Start(){
		inst = this;
		PlayerInfo._alive = true;
		PlayerInfo._hp = 1;
	}
	void Update(){
		if (!PlayerInfo._logged_in) return;
		
		if (!PlayerInfo._alive) {
			S3KCamera.inst.set_active_zoomed();
			PlayerInfo._respawn_ct--;
			if (PlayerInfo._respawn_ct <= 0) {
				PlayerInfo._alive = true;
				List<GameObject> respawn_points = ObjTag._tagged_objs["Respawn"];
				gameObject.transform.position = respawn_points[Util.rand.Next(respawn_points.Count)].transform.position;

			}
		} else {
			S3KCamera.inst.set_active_fps();
		}

	}
	
	void OnTriggerEnter(Collider col) {
		Bullet b = col.gameObject.GetComponent<Bullet>();
		if (b != null && b._playerid != PlayerInfo._id) {
			hit_by_bullet(b);
		}
	}
	
	void hit_by_bullet(Bullet b) {
		if (!PlayerInfo._alive) return;
		PlayerInfo._hp--;
		if (PlayerInfo._hp <= 0) {
			PlayerInfo._alive = false;
			PlayerInfo._respawn_ct = 125;
		}
	}
	
}