using UnityEngine;
using System.Collections.Generic;
using System;

public class chat_entry_manager : MonoBehaviour {
	
	public static chat_entry_manager instance;
	
	public List<chat_entry> added_chats = new List<chat_entry>();
	public List<chat_entry> new_chats = new List<chat_entry>();
	
	void Start () {
		instance = this;
	}

	void Update () {
		added_chats.AddRange(new_chats);
		new_chats.Clear();
		added_chats.Sort();
	
	}
	

}