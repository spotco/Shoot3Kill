using UnityEngine;
using System.Collections;

public class game_scores : MonoBehaviour {

	void OnGUI () {
		// Make a background box
		GUI.Box(new Rect(10,Screen.height - 50 , 100, 50), "Loader Menu");
		
		/*// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if(GUI.Button(new Rect(Screen.width / 2 - 100, 2 * Screen.height / 3, 200, 20), "Start the Game!")) {
			player_info.instance._name = username;
			Application.LoadLevel(1);
		}*/
		
		//username entry
		
		//username = GUI.TextField(new Rect(Screen.width / 2 - 100, Screen.height / 3, 200, 20), username);
		
		//label username
		GUI.Label(new Rect(11, Screen.height - 40, 80, 20), player_info.instance._name);
		
		
	}
}
