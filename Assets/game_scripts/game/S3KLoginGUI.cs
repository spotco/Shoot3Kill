using UnityEngine;
using System.Collections;

public class S3KLoginGUI : MonoBehaviour {

	Texture _logo_resc;
	static float INPUTWID = 200, INPUTHEI = 35;
	void Start () {
		_logo_resc = (Texture)Resources.Load("logo");
	}

	string _input_text = "";
	bool _pressed_logged_in = false;

	void OnGUI () {
		if (!PlayerInfo._logged_in) {
			GUI.skin.textField.fontSize = 20;

			GUI.DrawTexture(new Rect(
				Screen.width*0.5f-_logo_resc.width*0.5f, 
				Screen.height*0.25f-_logo_resc.height*0.5f, 
				_logo_resc.width, 
				_logo_resc.height
			   ), 
			_logo_resc);

			GUI.skin.label.normal.textColor = Color.black;
			GUI.Label(
				new Rect(Screen.width*0.5f - INPUTHEI*0.5f - 100, Screen.height*0.65f - 20*0.5f, 100, 20), 
				"Username:"
			);

			_input_text = GUI.TextField(
				new Rect(Screen.width*0.5f - INPUTWID*0.5f, Screen.height*0.75f - INPUTHEI*0.5f, INPUTWID, INPUTHEI), 
				_input_text
			);

			if (GUI.Button(new Rect(
					Screen.width*0.675f,
					Screen.height*0.75f - 40*0.5f,
					40,
					40
				),"Play!")) {

				if (_input_text.Length == 0) _input_text = "Anonymous";
				if (_input_text.Length > 16) _input_text = _input_text.Substring(0,16);
				_pressed_logged_in = true;

			}
		}
	}

	void Update () {
		if (!PlayerInfo._logged_in) {
			PlayerInfo._alive = false;
			S3KControl.inst.gameObject.transform.position = new Vector3(0,-10000,0);
			S3KControl.inst.gameObject.rigidbody.velocity = Vector3.zero;
			S3KCamera.inst.set_active_zoomed();

			if (_pressed_logged_in) {
				PlayerInfo._name = _input_text;
				PlayerInfo._alive = true;
				PlayerInfo._logged_in = true;
				S3KControl.inst.gameObject.transform.position = new Vector3(0,0,0);
			}
		}
	}
}
