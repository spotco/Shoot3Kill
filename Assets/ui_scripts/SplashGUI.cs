using UnityEngine;
using System.Collections;

public class SplashGUI : MonoBehaviour {

	//username
	string username = "";

	void OnGUI () {
		// Make a background box
		GUI.Box(new Rect(Screen.width - 100, 3 * Screen.height / 4 , 99, Screen.height / 4), "Chat");

		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if(GUI.Button(new Rect(Screen.width / 2 - 100, 2 * Screen.height / 3, 200, 20), "Start the Game!")) {
			player_info.instance._name = username;
			Application.LoadLevel(0);
		}

		//username entry

		username = GUI.TextField(new Rect(Screen.width / 2 - 100, Screen.height / 3, 200, 20), username);

		//label username
		GUI.Label(new Rect(Screen.width /2 - 50, Screen.height / 3 - 20, 100, 20), "Username");


	}
}
