using UnityEngine;
using System.Collections;



public class big_score : MonoBehaviour {
	bool show_score = false;
	Rect window = new Rect(50,50 , Screen.width - 100, Screen.height - 100);

	void OnGUI () {

		// Make a background box
		if (Event.current.type == EventType.keyDown && Event.current.character == '\t') show_score = true;
		if (Event.current.type == EventType.keyUp && Event.current.character == '\t') show_score = false;
		if(Input.GetKey(KeyCode.Tab)){
			window = GUI.Window (2, window, GlobalScoreWindow, "Scoreboard");

		
		/*// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if(GUI.Button(new Rect(Screen.width / 2 - 100, 2 * Screen.height / 3, 200, 20), "Start the Game!")) {
			player_info.instance._name = username;
			Application.LoadLevel(1);
		}*/
			
			//username entry
			
			//username = GUI.TextField(new Rect(Screen.width / 2 - 100, Screen.height / 3, 200, 20), username);
			
			//label username
			
		}
		
		
	}

	void GlobalScoreWindow(int id) {
		GUILayout.BeginHorizontal();
		
		GUILayout.Label (player_info.instance._name + ": " + player_info.instance._score as string);
		GUILayout.FlexibleSpace ();
		
		
		GUILayout.EndHorizontal();
		GUILayout.Space(3);

	}
}
