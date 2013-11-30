using UnityEngine;
using System.Collections;

[System.Serializable]
public class S3KGUI : MonoBehaviour {

	public static S3KGUI inst;

	public int _latency;

	void Start () {
		inst = this;
	}

	void Update () {
	
	}
}
