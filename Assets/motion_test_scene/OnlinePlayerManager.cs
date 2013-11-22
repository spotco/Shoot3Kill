using UnityEngine;
using System;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

public class OnlinePlayerManager : MonoBehaviour
{
		static Hashtable player_table;
		static Hashtable bullet_table;
		static string player_name;
		static int player_id;
		static SPMessage global_msg; 

		void Start ()
		{
				player_id = 0;
				player_name = "player" + player_id;
				player_table = new Hashtable ();
				bullet_table = new Hashtable ();

				create_message ();
		}

		void create_message() {
				global_msg = new SPMessage ();
				List<SPPlayerObject> players = new List<SPPlayerObject> ();
				SPPlayerObject player = new SPPlayerObject ();
				player._alive = 1;
				player._id = 1;
				player._name = "luke";

				SPVector3 pos_vector = new SPVector3 (0,0,0);
				SPVector3 vel_vector = new SPVector3 (0,0,0);
				SPVector3 rot_vector = new SPVector3 (0,0,0);

				player._pos = pos_vector;
				player._vel = vel_vector;
				player._rot = rot_vector;

				players.Add (player);

				global_msg._players = players;

				List<SPBulletObject> bullets = new List<SPBulletObject> ();

				global_msg._bullets = bullets;

		}
	

		void Update ()
		{
				read_message ();
				update_sprites ();
				update_message ();
		}

		void update_message(){
				List<SPPlayerObject> players = global_msg._players;
				var player = players [0];
				SPVector3 vel_vector = new SPVector3 (0.05f,0f,0f);
				player._vel = vel_vector;
		}

		void read_message() {
				SPMessage msg = global_msg; // Mock, will be incoming from server
				foreach (SPPlayerObject player_msg in msg._players) {
						int id = player_msg._id;
						if (!player_table.ContainsKey (id)) {
								Player new_player = new Player (player_msg);
								player_table.Add (id, new_player);
						} else {
								if (player_msg._alive == 0) {
										destroy_player ((Player)player_table[id]);
								} else {
										update_player (id, player_msg);
								}
						}
				}

				foreach (SPBulletObject bullet_msg in msg._bullets) {
						int bul_id = bullet_msg._id;
						if (!bullet_table.ContainsKey (bul_id)) {
								Bullet new_bullet = new Bullet (bullet_msg);
								bullet_table.Add (bul_id, new_bullet);
						} else {
								update_bullet (bul_id, bullet_msg);
						}
				}
		}

		void update_sprites () {
				foreach (Player p in player_table.Values) {

						if (p.get_player_object () == null) {
								GameObject new_player_object = (GameObject)Instantiate (Resources.Load ("SimulatedPlayer"));
								p.set_player_object(new_player_object);
								Debug.Log (new_player_object);
						} else {
								p.update ();
						}
				}

				foreach (Bullet b in bullet_table.Values) {
						if (b.get_bullet_object () == null) {
								//GameObject new_player_object = (GameObject)Instantiate (Resources.Load ("SimulatedBullet"));
						}
				}
		}


		void destroy_player(Player player) {
				GameObject.Destroy (player.get_player_object ());
				player.set_player_object (null);

		}

		void update_player (int id, SPPlayerObject msg) {
				Player player_to_update = (Player) player_table [id];
				player_to_update.set_pos (msg._pos);
				player_to_update.set_vel (msg._vel);
				player_to_update.set_rot (msg._rot);
		}

		void update_bullet (int id, SPBulletObject bullet_msg)
		{
				Player bullet_to_update = (Player) bullet_table [id];
				bullet_to_update.set_pos (bullet_msg._pos);
				bullet_to_update.set_vel (bullet_msg._vel);
				bullet_to_update.set_rot (bullet_msg._rot);
		}

		public class Player {
				GameObject _player_object;
				int _id;
				string _name;
				SPVector3 _pos;
				SPVector3 _vel;
				SPVector3 _rotation;
				int _alive;

				public Player(SPPlayerObject player_message) {
						this._id = player_message._id;
						this._name = player_message._name;
						this._pos = player_message._pos;
						this._vel = player_message._vel;
						this._rotation = player_message._rot;
						this._alive = player_message._alive;						                                   
				}

				public int get_id() {
						return this._id;
				}
	
				public GameObject get_player_object() {
						return _player_object;
				}

				public void set_player_object(GameObject player_object) {
						this._player_object = player_object;
				}

				public void set_pos(SPVector3 pos) {
						this._pos = pos;
				}

				public void set_vel(SPVector3 vel) {
						this._vel = vel;
				}

				public void set_rot(SPVector3 rot) {
						this._rotation = rot;
				}

				public void set_alive(int alive){
						this._alive = alive;
				}

				public void update() {
						MotionSimulatedPlayer player_component = _player_object.GetComponent<MotionSimulatedPlayer> ();

						player_component._pos.x = _pos._x;
						player_component._pos.y = _pos._y;
						player_component._pos.z = _pos._z;

						player_component._vel.x = _vel._x;
						player_component._vel.y = _vel._y;
						player_component._vel.z = _vel._z;

						player_component._rotation.x = _rotation._x;
						player_component._rotation.y = _rotation._y;
						player_component._rotation.z = _rotation._z;
				}

				public void respawn(){
						if (_alive == 0) {

						}
				}
		}

		public class Bullet{
				GameObject _bullet_object;
				int _bul_id;
				int _player_id;
				SPVector3 _bul_pos;
				SPVector3 _bul_vel;
				SPVector3 _bul_rotation;

				public Bullet(SPBulletObject bullet_message) {
						this._bul_id = bullet_message._id;
						this._player_id = bullet_message._playerid;
						this._bul_pos = bullet_message._pos;
						this._bul_vel = bullet_message._vel;
						this._bul_rotation = bullet_message._rot;
				}

				public GameObject get_bullet_object(){
						return _bullet_object;
				}

				public void set_bullet_object(GameObject bullet_object){
						this._bullet_object = bullet_object;
				}

				public void set_bul_pos(SPVector3 pos) {
						this._bul_pos = pos;
				}

				public void set_bul_vel(SPVector3 vel) {
						this._bul_vel = vel;
				}

				public void set_bul_rot(SPVector3 rot) {
						this._bul_rotation = rot;
				}
		}
}
