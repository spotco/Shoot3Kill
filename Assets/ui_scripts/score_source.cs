using UnityEngine;
using System.Collections;


public class score_source : MonoBehaviour {
	bool show_score = false;
	Rect window = new Rect(50,50 , Screen.width - 100, Screen.height - 100);
	public static Rect scoreboard = new Rect(50,50 , Screen.width - 100, Screen.height - 100);
	void OnGUI () {
		
		// Make a background box

		scoreboard = GUI.Window (3, scoreboard, GlobalScoreWindow, "Scores");

				

		
		
	}
	
	public static void GlobalScoreWindow(int id) {
		GUILayout.BeginHorizontal();
		
		GUILayout.Label (player_info.instance._name + ": " + player_info.instance._score as string);
		GUILayout.FlexibleSpace ();
		
		
		GUILayout.EndHorizontal();
		GUILayout.Space(3);
		
	}
}
