using UnityEngine;
using System.Collections;
using System;


[System.Serializable]
public class GameCollision : MonoBehaviour {

	void OnTriggerEnter(Collider col) {
		Bullet b = col.gameObject.GetComponent<Bullet>();
		if (b != null && b._playerid != PlayerInfo._id) {
			hit_by_bullet(b);
		}
	}

	void hit_by_bullet(Bullet b) {
		PlayerInfo._hp--;
		if (PlayerInfo._hp <= 0) {
			PlayerInfo._alive = false;
			PlayerInfo._respawn_ct = 400;
		}
	}

}