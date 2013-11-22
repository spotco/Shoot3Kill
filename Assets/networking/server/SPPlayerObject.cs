using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

[Serializable]
public class SPPlayerObject {
	public int _id = 0;
	public string _name = "lel";
	public SPVector3 _pos = SPVector3.ZERO();
	public SPVector3 _vel = SPVector3.ZERO();
	public SPVector3 _rot = SPVector3.ZERO();
	public int _alive = 0;


	public SPPlayerObject set_id(int id) {
		_id = id;
		return this;
	}

	public SPPlayerObject set_name(string name) {
		_name = name;
		return this;
	}

	public SPPlayerObject set_pos(int x,int y, int z) {
		_pos.set(x,y,z);
		return this;
	}

	public SPPlayerObject set_vel(int x,int y, int z) {
		_vel.set(x,y,z);
		return this;
	}

	public SPPlayerObject set_rot(int x,int y, int z) {
		_rot.set(x,y,z);
		return this;
	}

	public SPPlayerObject set_alive(int alive) {
		_alive = alive;
		return this;
	}
}
