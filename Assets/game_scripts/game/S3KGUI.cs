using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System;
using System.Text;
using System.Text.RegularExpressions;

[System.Serializable]
public class S3KGUI : MonoBehaviour {

	public static S3KGUI inst;

	public int _latency = 0;
	public string _status = "";

	static string CHATURL = "http://spotcos.com/chat/getchat.php?json=yes";
	static string SENDURL = "http://spotcos.com/chat/sendchat.php";
	static Uri URI = new Uri(CHATURL);
	static Uri SENDURI = new Uri(SENDURL);
	WebClient _webclient = new WebClient();
	WebClient _sendwebclient = new WebClient();

	void Start () {
		inst = this;
		_webclient.DownloadStringCompleted += new DownloadStringCompletedEventHandler((System.Object sender, DownloadStringCompletedEventArgs e) => {
			if (!e.Cancelled && e.Error == null) {
				_chat_display = "";
				string str = (string)e.Result;
				JSONObject jso = JSONObject.Parse(str);
				JSONArray posts = jso.GetArray("posts");
				int ct = 0;
				for (int i = posts.Length-1; i >= 0 && ct < 8; i--,ct++) {
					JSONObject post = posts[i].Obj;
					string msg = post.GetString("message");
					Regex r = new Regex("\n");
					msg = Regex.Unescape(msg);
					msg = r.Replace(msg,"");

					string name = post.GetString("name");
					if (name.Length == 0) name = "Anonymous";
					_chat_display+=(name+":"+msg+"\n");
					
				}
				
			}
		});
		_webclient.UploadValuesCompleted += new UploadValuesCompletedEventHandler((System.Object sender, UploadValuesCompletedEventArgs e) => {
			if (!e.Cancelled && e.Error == null) {
				update_chat();
				Debug.Log (ASCIIEncoding.ASCII.GetString(e.Result));
			}
		});
		update_chat();
	}

	static int CHATWIN_WID = 200, CHATWIN_HEI = 130;

	string _chat_entry_buffer = "";
	string _chat_display = ""; //8 lines
	public bool _in_type_mode = false;

	void OnGUI() {
		GUI.skin.label.normal.textColor = Color.black;
		GUI.Label(
			new Rect(0, 0, 100, 20), 
			_latency+"ms"
		);
		GUI.Label(
			new Rect(0, 20, 200, 20), 
			_status
		);


		if (!PlayerInfo._logged_in) return;

#if !UNITY_WEBPLAYER
		GUI.Label(
			new Rect(Screen.width-CHATWIN_WID, Screen.height-CHATWIN_HEI, CHATWIN_WID, CHATWIN_HEI), 
			_chat_display,
			GUI.skin.textArea
		);
#endif

		if (Event.current.type == EventType.KeyDown) {
			if (!_in_type_mode && Event.current.keyCode == KeyCode.Return) {
				_in_type_mode = true;

			} else if (_in_type_mode && Event.current.keyCode == KeyCode.Escape) {
				_chat_entry_buffer = "";
				_in_type_mode = false;

			} else if (_in_type_mode && Event.current.keyCode == KeyCode.Return) {
				if (_chat_entry_buffer.Length > 0) {
					send_chat_message(_chat_entry_buffer);
				}
				_chat_entry_buffer = "";
				_in_type_mode = false;

			} else if (_in_type_mode && Event.current.keyCode == KeyCode.Backspace && _chat_entry_buffer.Length > 0) {
				_chat_entry_buffer = _chat_entry_buffer.Substring(0,_chat_entry_buffer.Length-1);

			} else if (_in_type_mode && (int)Event.current.character != 0 && (int)Event.current.character != 10) {
				_chat_entry_buffer += Event.current.character;
			}

		}

		if (_in_type_mode) {
			GUI.Label(
				new Rect(Screen.width-CHATWIN_WID,Screen.height-CHATWIN_HEI-20,CHATWIN_WID,20),
				"Say:"+_chat_entry_buffer,
				GUI.skin.textArea
			);
		}
	}

	int _ct;
	void Update() {
		_ct++;
		if (_ct % 200 == 0) {
			update_chat();
		}
	}

	void update_chat() {
		if (!_webclient.IsBusy) _webclient.DownloadStringAsync(URI);
	}

	void send_chat_message(string msg) {
		_sendwebclient.UploadValuesAsync(SENDURI,new NameValueCollection() {
			{ "name", PlayerInfo._name },
			{ "msg", msg }
		});
	}
}
