using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySniffer1.Model {
	public class ApplicationPacket {
		byte[] Data;
		public ApplicationPacket(byte[] data) {
			this.Data = data;
			Console.WriteLine( Utils.BytetoVisibleString(data));
		}
	}
}
