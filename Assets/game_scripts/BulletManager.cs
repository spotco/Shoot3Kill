using UnityEngine;
using System.Collections.Generic;

public class BulletManager : MonoBehaviour {
	
	public static BulletManager instance;
	
	public List<Bullet> _bullets = new List<Bullet>();
	
	void Start () {
		instance = this;
	}
	
	void Update () {
        for (int i = _bullets.Count-1; i >= 0; i--) {
                Bullet b = _bullets[i];
                b.update();
                if (b.should_remove()) {
                        _bullets.RemoveAt(i);
                        b.do_remove();
                }
        }

		List<string> keys_to_remove = new List<string>();
		foreach(string key in _enemy_bullets.Keys) {
			Bullet b = _enemy_bullets[key];
			b.update();
			if (b.should_remove()) keys_to_remove.Add(key);
		}
		foreach(string key in keys_to_remove) {
			_enemy_bullets[key].do_remove();
			_enemy_bullets.Remove(key);
		}
	}
	
	
	int _allocid = 0;

	public void add_bullet(Vector3 position, Vector3 vel) {
		GameObject bullet_object = (GameObject)Instantiate(Resources.Load("Bullet"));
		bullet_object.transform.parent = gameObject.transform;
		_bullets.Add(new Bullet(position,vel,bullet_object,_allocid));
		_allocid++;
	}

	Dictionary<string,Bullet> _enemy_bullets = new Dictionary<string, Bullet>();

	public void msg_recieved(SPServerMessage msg) {
		foreach(SPBulletObject b in msg._bullets) {
			if (b._playerid == PlayerInfo._id) continue;
			if (!_enemy_bullets.ContainsKey(b.unique_key())) {
				GameObject bullet_object = (GameObject)Instantiate(Resources.Load("Bullet"));
				bullet_object.transform.parent = gameObject.transform;
				_enemy_bullets[b.unique_key()] = new Bullet(new Vector3(),new Vector3(b._vel._x,b._vel._y,b._vel._z),bullet_object,-1);
			}

			Bullet tar = _enemy_bullets[b.unique_key()];
			tar._position.x = b._pos._x;
			tar._position.y = b._pos._y;
			tar._position.z = b._pos._z;

			tar._vel.x = b._vel._x;
			tar._vel.y = b._vel._y;
			tar._vel.z = b._vel._z;

			tar.__ct = 50;
		}
	}
}


public class Bullet {

	public int _id;
	public Vector3 _position;
	public Vector3 _vel;
	public GameObject _obj;

	public int __ct = 0;
	
	public Bullet(Vector3 position, Vector3 vel,GameObject obj, int id) {
		_position = position;
		_vel = vel;
		_obj = obj; 
		_id = id;
		__ct = 50;
		_obj.transform.position = position;
		_obj.transform.forward = _vel;
	}
	
	public void update() {
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
	
	
}