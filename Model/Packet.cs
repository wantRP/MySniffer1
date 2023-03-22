using SharpPcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;

namespace MySniffer1.Model {
	public class RawPacket {
		public RawCapture p;
		public int Number { get; private set; }
		//public PosixTimeval Timeval { get { return p.Timeval; } }
		//public LinkLayers LinkLayerType { get { return p.LinkLayerType; } }
		public int Length { get { return p.Data.Length; } }
		public string Source { get; set; }
		public string Destination { get; set; }
		public string strr { get; set; }
		public byte[] Data { get; set; }
		public string Base64 { get { return Convert.ToBase64String(Data); } }
		private TimeSpan time;
		public string Time { get { return time.ToString("hh':'mm':'ss'.'ff"); } }
		public RawPacket(RawCapture p, int number) {
			this.p = p;
			this.Number = number;
			this.time = DateTime.Now.TimeOfDay;
			Packet packet = Packet.ParsePacket(p.LinkLayerType, p.Data);
			strr=packet.ToString(StringOutputType.Colored);
			this.Data = p.Data;
		}

	}
}
