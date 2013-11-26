using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class OnlinePlayerManager : MonoBehaviour
{
		public static OnlinePlayerManager instance;
		static OnlineClient online_client;
		static Hashtable player_table;
		static Player the_player;
			
		void Start ()
		{
				instance = this;
				online_client = new OnlineClient ();
				player_table = new Hashtable ();
		}

		void Update ()
		{
				update_sprites ();
		}

		public void read_message (SPMessage new_message) {
			HashSet<int> players_in = new HashSet<int>();
			foreach (SPPlayerObject player_msg in new_message._players) {
				int id = player_msg._id;
				players_in.Add(id);
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
			}
	
			foreach(int key in player_table.Keys) {
				if (!players_in.Contains(key)) {
					SPVector sptv = ((Player)player_table[key])._pos;
					EffectManager.instance.add_effect(new Effect("Shockwave",new Vector3(sptv._x,sptv._y,sptv._z),20));
					player_table.Remove(key);
					
				}
			}
		}

		void spawn_sprite(Player new_sprite){
			GameObject new_player_object = (GameObject)Instantiate (Resources.Load ("onlineplayer"));
			new_sprite._player_object = new_player_object;
		}

		void update_sprites () {
				foreach (Player p in player_table.Values) {
					p.Update();
				}
		}

		void update_player (int id, SPPlayerObject msg) {
				Player player_to_update = (Player) player_table [id];
				player_to_update._pos = msg._pos;
				player_to_update._vel = msg._vel;
				player_to_update._rotation = msg._rot;
		}
}
