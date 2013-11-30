using UnityEngine;
using System.Collections.Generic;

public class BulletManager : MonoBehaviour {
	
	public static BulletManager inst;
	
	public List<Bullet> _bullets = new List<Bullet>();
	
	void Start () {
		inst = this;
	}
	
	void Update () {
        for (int i = _bullets.Count-1; i >= 0; i--) {
                Bullet b = _bullets[i];
                if (b.should_remove()) {
                        _bullets.RemoveAt(i);
                        b.do_remove();
                }
        }

		List<string> keys_to_remove = new List<string>();
		foreach(string key in _enemy_bullets.Keys) {
			Bullet b = _enemy_bullets[key];
			if (b.should_remove()) keys_to_remove.Add(key);
		}
		foreach(string key in keys_to_remove) {
			_enemy_bullets[key].do_remove();
			_enemy_bullets.Remove(key);
		}
	}
	
	
	int _allocid = 0;

	public void add_bullet(Vector3 position, Vector3 vel) {
		GameObject bullet_object = (GameObject)Instantiate(Resources.Load("bullet"));
		bullet_object.transform.parent = gameObject.transform;
		bullet_object.AddComponent<Bullet>();
		Bullet neu_bullet = bullet_object.GetComponent<Bullet>();
		neu_bullet.start(position,vel,bullet_object,_allocid,PlayerInfo._id);
		_bullets.Add(neu_bullet);
		_allocid++;
	}

	public Dictionary<string,Bullet> _enemy_bullets = new Dictionary<string, Bullet>();

	public void msg_recieved(SPServerMessage msg) {
		foreach(SPBulletObject b in msg._bullets) {
			if (b._playerid == PlayerInfo._id) continue;
			if (!_enemy_bullets.ContainsKey(b.unique_key())) {
				GameObject bullet_object = (GameObject)Instantiate(Resources.Load("bullet"));
				bullet_object.transform.parent = gameObject.transform;
				bullet_object.AddComponent<Bullet>();
				Bullet neu_bullet = bullet_object.GetComponent<Bullet>();
				neu_bullet.start(new Vector3(b._pos._x,b._pos._y,b._pos._z),new Vector3(b._vel._x,b._vel._y,b._vel._z),bullet_object,b._id,b._playerid);
				_enemy_bullets[b.unique_key()] = neu_bullet;
			}

			Bullet tar = _enemy_bullets[b.unique_key()];
			tar._position.x = b._pos._x;
			tar._position.y = b._pos._y;
			tar._position.z = b._pos._z;

			tar._vel.x = b._vel._x;
			tar._vel.y = b._vel._y;
			tar._vel.z = b._vel._z;

			tar.__ct = 25;
		}
	}
}