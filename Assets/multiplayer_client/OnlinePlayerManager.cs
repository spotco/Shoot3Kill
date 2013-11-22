using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class OnlinePlayerManager : MonoBehaviour
{
		public static OnlinePlayerManager instance;
		static OnlineClient online_client;
		static Hashtable player_table;
		static Hashtable bullet_table;
		static Player the_player;
		static List<SPBulletObject> player_bullets;
			
		void Start ()
		{
				instance = this;
				online_client = new OnlineClient ();
				player_table = new Hashtable ();
				bullet_table = new Hashtable ();
				player_bullets = new List<SPBulletObject>();
		}

		void Update ()
		{
				update_sprites ();
		}

		void create_bullet(SPBulletObject new_bullet){
				player_bullets.Add (new_bullet);
		} 

		public void read_message (SPMessage new_message) {
				foreach (SPPlayerObject player_msg in new_message._players) {
						int id = player_msg._id;
						if (!player_table.ContainsKey (id)) {
								Player new_player = new Player (player_msg);
								player_table.Add (id, new_player);

								spawn_sprite((Player)player_table[id]);

						}
						//Already added to table
						Player tar = (Player)player_table[id];

						
						tar._pos = player_msg._pos;
						tar._rotation = player_msg._rot;
						tar._vel = player_msg._vel;
						tar._player_object.transform.position = new Vector3(tar._pos._x,tar._pos._y,tar._pos._z);
						tar._player_object.transform.eulerAngles = new Vector3(tar._rotation._x,tar._rotation._y,tar._rotation._z);

								/*
								if (player_msg._alive == 0) {
										destroy_player ((Player)player_table [id]);
								} else {
										update_player (id, player_msg);
								}
								*/
						
						
				}

				foreach (SPBulletObject bullet_msg in new_message._bullets) {
						string bul_id = bullet_msg._id + "_" + bullet_msg._playerid;
						if (!bullet_table.ContainsKey (bul_id)) {
								Bullet new_bullet = new Bullet (bullet_msg);
								bullet_table.Add (bul_id, new_bullet);
						} else {
								update_bullet (bul_id, bullet_msg);
						}

				}
		}

		void spawn_sprite(Player new_sprite){
			GameObject new_player_object = (GameObject)Instantiate (Resources.Load ("onlineplayer"));
			new_sprite._player_object = new_player_object;
		}

		void update_sprites () {
				foreach (Player p in player_table.Values) {
						if (p._player_object == null) {
								GameObject new_player_object = (GameObject)Instantiate (Resources.Load ("onlineplayer"));
								p._player_object = new_player_object;
						} else {
								p.update ();
						}
				}

				foreach (Bullet b in bullet_table.Values) {
						if (b.get_bullet_object () == null) {
								//GameObject new_player_object = (GameObject)Instantiate (Resources.Load ("SimulatedBullet"));
						}else{
								if(b.is_out_of_bounds()){ 
										destroy_bullet(b);
								}
						}
				}
		}


		void kill_player(){
				Player myPlayer = the_player;
				player_table.Remove (the_player._id);
				myPlayer._alive = 0;
				destroy_player (myPlayer);
				myPlayer.respawn();
				player_table.Add (the_player._id, myPlayer);
		}

		void destroy_player(Player player) {
				GameObject.Destroy (player.get_player_object ());
				player._player_object = null;
		}

		void destroy_bullet (Bullet bullet){
				GameObject.Destroy (bullet.get_bullet_object ());
				bullet.set_bullet_object (null);
		}

		void update_player (int id, SPPlayerObject msg) {
				Player player_to_update = (Player) player_table [id];
				player_to_update._pos = msg._pos;
				player_to_update._vel = msg._vel;
				player_to_update._rotation = msg._rot;
		}

		void update_bullet (string id, SPBulletObject bullet_msg)
		{
		
				Bullet bullet_to_update = (Bullet) bullet_table [id];
				bullet_to_update._bul_pos = bullet_msg._pos;
				bullet_to_update._bul_vel = bullet_msg._vel;
				bullet_to_update._bul_rotation = bullet_msg._rot;
		}
}
