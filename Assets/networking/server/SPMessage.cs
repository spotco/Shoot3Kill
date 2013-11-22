using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Collections.Generic;

[Serializable]
public class SPMessage {

	public List<SPPlayerObject> _players;
	public List<SPBulletObject> _bullets;

		public SPMessage create_local_message(Player p) {
			
		}
}
