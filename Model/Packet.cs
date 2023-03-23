﻿using SharpPcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;
using System.Windows;

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
		public byte[] Data { get; set; }
		public string Base64 { get { return Convert.ToBase64String(Data); } }
		private TimeSpan time;
		public string Time { get { return time.ToString("hh':'mm':'ss'.'ff"); } }
		public RawPacket(RawCapture p, int number) {
			this.p = p;
			this.Number = number;
			this.time = DateTime.Now.TimeOfDay;
			EthernetPacket packet = new EthernetPacket(new PacketDotNet.Utils.ByteArraySegment(p.Data));
			Details = packet.ToString(StringOutputType.Normal);
			this.Type = packet.Type.ToString();
			EthernetType type = packet.Type;
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
			this.Source=ippacket.SourceAddress.ToString();
			this.Destination=ippacket.DestinationAddress.ToString();
			 TransportPacket transportPacket=new TcpPacket(0,0) ;
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
			}
			this.SourcePort = transportPacket.SourcePort;
			this.DestinationPort = transportPacket.DestinationPort;
				//MessageBox.Show(packet.PayloadPacket.GetType().ToString());

				this.Data = p.Data;
		}

	}
}
