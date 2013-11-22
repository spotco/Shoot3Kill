using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class chat : MonoBehaviour {



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
		if (Input.GetKeyDown(KeyCode.Return) && GUIUtility.keyboardControl == 0)
		{
			GUI.FocusControl("Chat input field");
				

		}
		window = GUI.Window (1, window, GlobalChatWindow, "Chat");

		
	}

	void GlobalChatWindow (int id) {
		
		// Begin a scroll view. All rects are calculated automatically - 
		// it will use up any available screen space and make sure contents flow correctly.
		// This is kept small with the last two parameters to force scrollbars to appear.
		scrollPosition = GUILayout.BeginScrollView (scrollPosition);
		
		foreach (chat_entry entry in chat_entry_manager.instance.added_chats)
		{
			GUILayout.BeginHorizontal();

			GUILayout.Label (entry.content);
			GUILayout.FlexibleSpace ();

			
			GUILayout.EndHorizontal();
			GUILayout.Space(3);
			
		}
		// End the scrollview we began above.
		GUILayout.EndScrollView ();
		
		if (Event.current.type == EventType.keyDown && Event.current.character == '\n' && inputField.Length > 0)
		{
			//@TODO: This should be dependent on who actually sent the message
			//var mine = entries.Count % 2 == 0;
			ApplyGlobalChatText(player_info.instance._name + ": " + inputField, 1);
			inputField = "";
			GUIUtility.keyboardControl = 0;
			GUIUtility.hotControl = 0;
			GUI.UnfocusWindow();
		}
		GUI.SetNextControlName("Chat input field");
		inputField = GUILayout.TextField(inputField);
		
		GUI.DragWindow();
	}
	

	void ApplyGlobalChatText (string str, int id)
	{
		chat_entry entry = new chat_entry(str, DateTime.Now, false);

		chat_entry_manager.instance.new_chats.Add(entry);

		scrollPosition.y = 1000000;	
	}
}