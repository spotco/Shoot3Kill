using UnityEngine;
using System.Collections;

public class FacePlayer : MonoBehaviour {

	void Update () {
		gameObject.transform.LookAt(S3KControl.inst.gameObject.transform);
	}
}
