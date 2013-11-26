using UnityEngine;
using System.Collections;

public class OnlinePlayerManager : MonoBehaviour {

	public static OnlinePlayerManager instance;

	void Start () {
		instance = this;
	}

	public void msg_recieved(SPServerMessage msg) {

	}

	void Update () {
	
	}
}
