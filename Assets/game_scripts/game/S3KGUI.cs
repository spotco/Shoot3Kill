using UnityEngine;
using System.Collections;

[System.Serializable]
public class S3KGUI : MonoBehaviour {

	public static S3KGUI inst;

	public int _latency = 0;
	public string _status = "";

	void Start () {
		inst = this;
	}

	void OnGUI() {
		GUI.skin.label.normal.textColor = Color.white;
		GUI.Label(
			new Rect(0, 0, 100, 20), 
			_latency+"ms"
		);
		GUI.Label(
			new Rect(0, 20, 200, 20), 
			_status
		);
		GUI.Label(
			new Rect(0, 40, 200, 20), 
			"score: "+PlayerInfo._score
		);
	}

	void Update () {
	
	}
}
