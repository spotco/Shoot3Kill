﻿using UnityEngine;
using System.Collections;

public class player_info : MonoBehaviour {
	public static player_info instance;

	public string _name = "";

	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
