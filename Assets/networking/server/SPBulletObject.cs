using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

[Serializable]
public class SPBulletObject {
	public int _id = 0;
	public int _playerid = 0;
	public SPVector3 _pos = SPVector3.ZERO();
	public SPVector3 _vel = SPVector3.ZERO();
	public SPVector3 _rot = SPVector3.ZERO();

	public SPBulletObject set_id(int id) {
		_id = id;
		return this;
	}

	public SPBulletObject set_playerid(int playerid) {
		_playerid = playerid;
		return this;
	}

	public SPBulletObject set_pos(int x,int y, int z) {
		_pos.set(x,y,z);
		return this;
	}

	public SPBulletObject set_vel(int x,int y, int z) {
		_vel.set(x,y,z);
		return this;
	}

	public SPBulletObject set_rot(int x,int y, int z) {
		_rot.set(x,y,z);
		return this;
	}
}
