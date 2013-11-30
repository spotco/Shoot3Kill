using UnityEngine;
using System.Collections.Generic;

public class ObjTag : MonoBehaviour {

	public static Dictionary<string,List<GameObject>> _tagged_objs = new Dictionary<string, List<GameObject>>();

	void Start () {
		if (!_tagged_objs.ContainsKey(gameObject.name)) _tagged_objs[gameObject.name] = new List<GameObject>();
		_tagged_objs[gameObject.name].Add(gameObject);
	}
}
