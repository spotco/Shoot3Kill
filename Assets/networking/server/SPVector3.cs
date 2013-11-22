using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

[Serializable]
public class SPVector3 {
		public float _x,_y,_z;

		public SPVector3 (float x, float y, float z) {
				this._x = x;
				this._y = y;
				this._z = z;
		}
}
