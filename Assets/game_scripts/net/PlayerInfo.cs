using UnityEngine;
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
 * refactor networking code:
 * 		-unified send-ack based event system per user
 * 		-object to represent user connection on server
 * 		-push bullet only once client, into all other clients on server
 * unified copy-values-to-spobjects code
 * 
 * socket chat server too
 * 
 * better motion prediction:
 * 		more lastframe polls on client side
 * 		smooth interpolation to msg recieved point client side
 **/
