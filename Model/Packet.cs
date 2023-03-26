using SharpPcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PacketDotNet;
using System.Windows;
using System.Text.RegularExpressions;
using MySniffer1.Model;
namespace MySniffer1.Model {
	public class RawPacket {
		public RawCapture p;
		public int Number { get; private set; }
		//public PosixTimeval Timeval { get { return p.Timeval; } }
		//public LinkLayers LinkLayerType { get { return p.LinkLayerType; } }
		public int Length { get { return p.Data.Length; } }
		public string Source { get; set; }
		public string Destination { get; set; }
		public int SourcePort { get; set; }
		public int DestinationPort { get; set; }
		public string Details { get; set; }
		public string Type { get; set; }
		public string HexString { get; set; }
		public string PlainString { get; set; }
		public byte[] Data { get; set; }
		public string Base64 { get { return Convert.ToBase64String(Data); } }
		private TimeSpan time;
		public string Time { get { return time.ToString("hh':'mm':'ss'.'ff"); } }
		public RawPacket(RawCapture p, int number) {
			this.p = p;
			this.Data = p.Data;
			this.Number = number;
			this.time = DateTime.Now.TimeOfDay;
			EthernetPacket packet = new EthernetPacket(new PacketDotNet.Utils.ByteArraySegment(p.Data));
			Details = packet.ToString(StringOutputType.Verbose);
			this.Type = packet.Type.ToString();
			EthernetType type = packet.Type;
			HexString = Regex.Replace(Model.Utils.BytetoHex(this.Data), ".{2}", "$0 ");
			PlainString = Model.Utils.BytetoVisibleString(this.Data);
			IPPacket ippacket;
			switch (type) {
				case EthernetType.IPv4: {
						ippacket = new IPv4Packet(packet.HeaderDataSegment);

						break;
					}
				case EthernetType.IPv6: {
						ippacket = new IPv6Packet(packet.HeaderDataSegment);
						break;
					}
				default: {
						ippacket = new IPv4Packet(packet.HeaderDataSegment);
						break;
					}
			}

			this.Type = ippacket.Protocol.ToString();
			this.Source = ippacket.SourceAddress.ToString();
			Destination = ippacket.DestinationAddress.ToString();
			TransportPacket transportPacket = new TcpPacket(0, 0);
			InternetPacket internetPacket;
			switch (ippacket.Protocol) {
				case ProtocolType.Tcp: {
						transportPacket = new TcpPacket(ippacket.HeaderDataSegment);
						break;
					}
				case ProtocolType.Udp: {
						transportPacket = new UdpPacket(ippacket.HeaderDataSegment);
						break;
					}
				case ProtocolType.Icmp: {
						//transportPacket = new TcpPacket(0, 0);
						internetPacket = new IcmpV4Packet(ippacket.HeaderDataSegment);
						break;
					}
					default : {
						transportPacket = new TcpPacket(ippacket.HeaderDataSegment);
						break;
					}
			}
			ApplicationPacket applicationPacket;

			if (ippacket.Protocol == ProtocolType.Tcp||ippacket.Protocol==ProtocolType.Udp) {
				string s = Utils.BytetoVisibleString(transportPacket.PayloadData);
				if (s.StartsWith("GET")||s.StartsWith("POST")||s.Contains("HTTP/")) this.Type = "HTTP";
			}
			this.SourcePort = transportPacket.SourcePort;
			this.DestinationPort = transportPacket.DestinationPort;
			//MessageBox.Show(packet.PayloadPacket.GetType().ToString());


		}

	}
}
