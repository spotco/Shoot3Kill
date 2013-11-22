using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class OnlinePlayerManager : MonoBehaviour
{
		static Hashtable player_table;
		static Hashtable bullet_table;
		static Player the_player;
		static SPMessage global_msg; 
		List<SPBulletObject> player_bullets;
			
		void Start ()
		{
				player_table = new Hashtable ();
				bullet_table = new Hashtable ();
				player_bullets = new List<SPBulletObject>();
		}


		void Update ()
		{
				update_sprites ();
				send_message ();
				kill_player ();
		}

		void send_new_player(string newName){
				SPPlayerObject myPlayer = new SPPlayerObject ();
				myPlayer._id = -1;
				myPlayer._name = newName;
				myPlayer._pos = new Vector3 (0, 0, 0);
				myPlayer._vel = new Vector3 (0, 0, 0);
				myPlayer._rot = new Vector3 (0, 0, 0);
				myPlayer._alive = 1;

				SPMessage new_message = new SPMessage ();

				new_message._players.Add(myPlayer);

				//socketmagicfuckerysendtoserver (new_message);
		}

		void create_myself(SPMessage c_section){
				SPPlayerObject player_obj = (SPPlayerObject)c_section._players [0]; 
				Player thine_player = new Player (player_obj);
				thine_player._player_object = gameObject.GetComponent<RigidBody> ();
				the_player = thine_player;
		}

		void create_bullet(SPBulletObject new_bullet){
				player_bullets.add (new_bullet);
		} 


		void read_message (SPMessage new_message) {
				foreach (SPPlayerObject player_msg in new_message._players) {
						int id = player_msg._id;
						if (!player_table.ContainsKey (id)) {
								Player new_player = new Player (player_msg);
								player_table.Add (id, new_player);
						} else {
								if (player_msg._alive == 0) {
										destroy_player ((Player)player_table [id]);
								} else {
										update_player (id, player_msg);
								}
						}
				}

				foreach (SPBulletObject bullet_msg in new_message._bullets) {
						string bul_id = bullet_msg._id;
						if (!bullet_table.ContainsKey (bul_id)) {
								Bullet new_bullet = new Bullet (bullet_msg);
								bullet_table.Add (bul_id, new_bullet);
						} else {
								update_bullet (bul_id, bullet_msg);
						}

						if (collision ((Player)the_player, (Bullet)bullet_table [bul_id])) {
								kill_player ();
						}
				}
		}
	
		SPMessage send_message(){
				SPMessage new_message = new SPMessage ();

				new_message._players.Add(the_player.convert_to_obj());

				new_message._bullets = player_bullets;

				return new_message; 
		}

		void update_sprites () {
				foreach (Player p in player_table.Values) {

						if (p.get_player_object () == null) {
								GameObject new_player_object = (GameObject)Instantiate (Resources.Load ("SimulatedPlayer"));
								p.set_player_object(new_player_object);
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

		bool collision(Player p, Bullet b){
				Vector3 player_pos = p._pos;
				Vector3 bullet_pos = b._bul_pos;
				if (Util.vec_dist (player_pos, bullet_pos) < 1.5f) {
						return true;
				}
				return false;
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
				player.set_player_object (null);
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
