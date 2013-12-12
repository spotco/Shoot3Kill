﻿using UnityEngine;
using System.Collections;

public class PlayerInfo {

	public static int _id;
	public static string _name = "Anonymous";
	public static int _hp = 1;
	public static bool _alive;
	public static int _respawn_ct;
	public static int _score = 0;

	public static bool _logged_in = false;

}

/**
 * TODO --
 * better motion prediction:
 * 		more lastframe polls on client side
 * 		smooth interpolation to msg recieved point client side
 * scoreboard
 * chat board
 * hud gui with latency and kills/deaths
 * thread for spbullet timeout on server
 * particle effects for player death, bullet hit
 * unified copy-values-to-spobjects code
 **/