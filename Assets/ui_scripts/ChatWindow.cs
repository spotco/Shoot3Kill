using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChatWindow : MonoBehaviour {
	
	bool showChat = true;
	string inputField = "";
	bool display = true;
	
	Vector2 scrollPosition = new Vector2();
	
	Rect window = new Rect(Screen.width - 210, Screen.height - 320, 200, 300);
	
	void CloseChatWindow() {
		showChat = false;
		inputField = "";
		
	}
	
	void FocusControl() {
		GUI.FocusControl("Chat input field");
		
	}
	
	void OnGUI () {
		//if (GUILayout.Button(showChat ? "Hide Chat" : "Display Chat"))
		if (Input.GetKeyDown(KeyCode.Return))
		{
			GUI.FocusControl("Chat input field");
			
			
		}
		window = GUI.Window (1, window, GlobalChatWindow, "Chat");
		
		
	}

	public static string TEST_LAST_UPDATE = "";
	
	void GlobalChatWindow (int id) {
		
		// Begin a scroll view. All rects are calculated automatically - 
		// it will use up any available screen space and make sure contents flow correctly.
		// This is kept small with the last two parameters to force scrollbars to appear.
		scrollPosition = GUILayout.BeginScrollView (scrollPosition);
		
		for (int i = 0 ; i < 4; i++)
		{
			GUILayout.BeginHorizontal();
			
			GUILayout.Label (TEST_LAST_UPDATE);
			GUILayout.FlexibleSpace ();
			
			
			GUILayout.EndHorizontal();
			GUILayout.Space(3);
			
		}
		// End the scrollview we began above.
		GUILayout.EndScrollView ();
		
		if (Event.current.type == EventType.keyDown && Event.current.character == '\n' && inputField.Length > 0)
		{

			inputField = "";
			GUIUtility.keyboardControl = 0;
			GUIUtility.hotControl = 0;
			GUI.UnfocusWindow();
		}
		GUI.SetNextControlName("Chat input field");
		inputField = GUILayout.TextField(inputField);
		
		GUI.DragWindow();
	}
	
	
	void ApplyGlobalChatText (string str, string username)
	{
		Debug.Log (str);
		
		scrollPosition.y = 1000000;	
	}
}