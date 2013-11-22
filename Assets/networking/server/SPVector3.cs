using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

[Serializable]
public class SPVector3 {
	public int _x,_y,_z;
	public static SPVector3 ZERO() {
		return new SPVector3(0,0,0);
	}

	public SPVector3(int x, int y, int z) {
		_x = x;
		_y = y;
		_z = z;
	}

	public SPVector3 set(int x, int y, int z) {
		_x = x;
		_y = y;
		_z = z;
		return this;
	}



}
