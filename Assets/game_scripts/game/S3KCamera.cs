using UnityEngine;
using System.Collections;

[System.Serializable]
public class S3KCamera : MonoBehaviour {
	public static S3KCamera inst;

	public GameObject _zoomed_clocator;
	public GameObject _fps_clocator;

	public bool _OVR_mode = false;

	public GameObject _normal_camera;
	public GameObject _ovr_camera;

	void Start () {
		inst = this;

		_zoomed_clocator = ObjTag._tagged_objs["ZoomedOutCamera"][0];
		_fps_clocator = Util.FindInHierarchy(gameObject,"FPSCamera");

		_normal_camera = Util.FindInHierarchy(gameObject,"NormalCamera");
		_ovr_camera = Util.FindInHierarchy(gameObject,"OVRCameraController");
	}

	void Update () {
#if !UNITY_WEBPLAYER
		if (Input.GetKeyDown(KeyCode.P) && PlayerInfo._logged_in && !S3KGUI.inst._in_type_mode) {
			_OVR_mode = !_OVR_mode;
			set_main_or_vr();
		}
#endif
	}

	void set_main_or_vr() {
		if (!_OVR_mode) {
			_normal_camera.SetActive(true);
			_ovr_camera.SetActive(false);
		} else {
			_normal_camera.SetActive(false);
			_ovr_camera.SetActive(true);
		}
	}

	public void set_active_fps() {
		_normal_camera.transform.parent = _fps_clocator.transform;
		_ovr_camera.transform.parent = _fps_clocator.transform;

		_normal_camera.transform.localPosition = Vector3.zero;
		_normal_camera.transform.localEulerAngles = Vector3.zero;

		_ovr_camera.transform.localPosition = Vector3.zero;
		_ovr_camera.transform.localEulerAngles = Vector3.zero;
	}

	public void set_active_zoomed() {
		_normal_camera.transform.parent = _zoomed_clocator.transform;
		_ovr_camera.transform.parent = _zoomed_clocator.transform;
		
		_normal_camera.transform.localPosition = Vector3.zero;
		_normal_camera.transform.localEulerAngles = Vector3.zero;
		
		_ovr_camera.transform.localPosition = Vector3.zero;
		_ovr_camera.transform.localEulerAngles = Vector3.zero;
	}
}
