using UnityEngine;
using System.Collections;



public class big_score : MonoBehaviour {
	bool show_score = false;

	void OnGUI () {


		if(Input.GetKey(KeyCode.Tab)){
			score_source.scoreboard = GUI.Window (5, score_source.scoreboard, GlobalScoreWindow, "Scores");

		
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
