using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;

public class SPMessage {
	public List<SPPlayerObject> _players = new List<SPPlayerObject>();
	public List<SPBulletObject> _bullets = new List<SPBulletObject>();
}
