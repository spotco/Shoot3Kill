using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

[Serializable]
public class SPBulletObject {
	public int _id;
	public int _playerid;
	public SPVector3 _pos;
	public SPVector3 _vel;
	public SPVector3 _rot;
}
