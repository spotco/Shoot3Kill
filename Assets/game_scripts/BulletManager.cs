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
	}
	
	
	public void add_bullet(Vector3 position, Vector3 vel) {
		GameObject bullet_object = (GameObject)Instantiate(Resources.Load("Bullet"));
		bullet_object.transform.parent = gameObject.transform;
		_bullets.Add(new Bullet(position,vel,bullet_object));
	}

	public void msg_recieved(SPServerMessage msg) {

	}
}


public class Bullet {
	
	public Vector3 _position;
	public Vector3 _vel;
	public GameObject _obj;
	private int _ct = 0;
	
	public Bullet(Vector3 position, Vector3 vel,GameObject obj) {
		_position = position;
		_vel = vel;
		_obj = obj; 
		_ct = 50;
		_obj.transform.position = position;
		_obj.transform.forward = _vel;
	}
	
	public void update() {
		_position.x += _vel.x;
		_position.y += _vel.y;
		_position.z += _vel.z;
		_obj.transform.position = _position;
		_ct--;
	}
	
	public bool should_remove() {
		return _ct <= 0;	
	}
	
	public void do_remove() {
		GameObject.Destroy(_obj);
		_obj = null;
	}
	
	
}