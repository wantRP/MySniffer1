using SharpPcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;

namespace MySniffer1.Model {
	public class Packet {
		public RawCapture p;
		public int Number { get; private set; }
		public PosixTimeval Timeval { get { return p.Timeval; } }
		public LinkLayers LinkLayerType { get { return p.LinkLayerType; } }
		public int Length { get { return p.Data.Length; } }
		public string Source { get; set; }
		public string Destination { get; set; }
		public Packet(RawCapture p, int number) {
			this.p = p;
			this.Number = number;
		}

	}
}
