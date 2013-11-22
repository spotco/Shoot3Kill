using UnityEngine;
using System.Collections;
using System;

public class chat_entry : MonoBehaviour, IComparable<chat_entry> {

	public string content;
	public DateTime time_sent;
	public bool kill_message;

	public chat_entry(string _content, DateTime _time_sent, bool _kill_message) {
		content = _content;
		time_sent = _time_sent;
		kill_message = _kill_message;
	}

	public int CompareTo(chat_entry compare_chat) {
		return this.time_sent.CompareTo(compare_chat.time_sent);
	}

//	public override bool Equals(object obj) {
//		if (obj == null) return false;
//		chat_entry objAsChatEntry = obj as chat_entry;
//		if (objAsChatEntry == null) return false;
//		else return Equals
//
//	}

//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
}
