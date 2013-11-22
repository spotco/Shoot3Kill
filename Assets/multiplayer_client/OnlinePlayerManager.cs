using UnityEngine;
using System;
using System.Collections;
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
		/*
				global_msg = new SPMessage ();
				List<SPPlayerObject> players = new List<SPPlayerObject> ();
				SPPlayerObject player = new SPPlayerObject ();
				player._alive = 1;
				player._id = 1;
				player._name = "luke";
	
				
				Vector3 pos_vector = new Vector3 (0,0,0);
				Vector3 vel_vector = new Vector3 (0,0,0);
				Vector3 rot_vector = new Vector3 (0,0,0);

				player._pos = pos_vector;
				player._vel = vel_vector;
				player._rot = rot_vector;

				players.Add (player);

				global_msg._players = players;

				List<SPBulletObject> bullets = new List<SPBulletObject> ();

				global_msg._bullets = bullets;
		 */
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
				Vector3 vel_vector = new Vector3 (0.05f,0f,0f);
				//player._vel = vel_vector;
		}

		void read_message() {
				SPMessage msg = global_msg; // Mock, will be incoming from server
				foreach (SPPlayerObject player_msg in msg._players) {
						int id = player_msg._id;
						if (!player_table.ContainsKey (id)) {
								Player new_player = new Player (player_msg);
								player_table.Add (id, new_player);
						} else {
				/*
								if (player_msg._alive == 0) {
										destroy_player ((Player)player_table[id]);
								} else {
										update_player (id, player_msg);
								}*/
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
				/*foreach (Player p in player_table.Values) {

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
						}else{

								if(b.is_out_of_bounds() || collision((Player)player_table[player_id], b)){ 
										destroy_bullet(b);
								}
						}
				}*/
		}

		bool collision(Player p, Bullet b){
				Vector3 player_pos = p._pos;
				Vector3 bullet_pos = b._bul_pos;
				if (Util.vec_dist (player_pos, bullet_pos) < 1.5f) {
						return true;
				}
				return false;
		}


		void destroy_player(Player player) {
				GameObject.Destroy (player.get_player_object ());
				player.set_player_object (null);
		}

		void destroy_bullet (Bullet bullet){

				GameObject.Destroy (bullet.get_bullet_object ());
				bullet.set_bullet_object (null);
		}

		void update_player (int id, SPPlayerObject msg) {
				/*Player player_to_update = (Player) player_table [id];
				player_to_update._pos = msg._pos;
				player_to_update._vel = msg._vel;
				player_to_update._rotation = msg._rot;*/
		}

		void update_bullet (int id, SPBulletObject bullet_msg)
		{
		/*
				Bullet bullet_to_update = (Bullet) bullet_table [id];
				bullet_to_update._bul_pos = bullet_msg._pos;
				bullet_to_update._bul_vel = bullet_msg._vel;
				bullet_to_update._bul_rotation = bullet_msg._rot;*/
		}

}
