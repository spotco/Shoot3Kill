using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

[Serializable]
public class SPPlayerObject {
	public int _id;
	public string _name;
	public SPVector3 _pos;
	public SPVector3 _vel;
	public SPVector3 _rot;
	public int _alive;
}
