using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SharpPcap;

namespace MySniffer1.ViewModel {
	public class ViewModel {
		public ObservableCollection<IPNetworkInterface> InterfaceList { get; set; }
		public class IPNetworkInterface {
			public string InterfaceName { get; set; }
			public ILiveDevice Interface { get; set; }
		}
		public IPNetworkInterface selectedInterface { get; set; }
		private void fetchNetworkInterfaceName() {
			/*
			NetworkInterface[] ni = NetworkInterface.GetAllNetworkInterfaces();
			string[] strl = new string[ni.Length];
			for (int i = 0; i < ni.Length; ++i) {
				string ipstr = ni[i].Name;
				//Console.WriteLine(ipstr);
				IPNetworkInterface ipf = new IPNetworkInterface { InterfaceName = ipstr, Interface = ni[i] };
				//Console.WriteLine(i);
				InterfaceList.Add(ipf);
				strl[i] = ni[i].Name;
			}*/
			var thel = CaptureDeviceList.Instance;
			foreach (var iface in thel) {
				IPNetworkInterface ipf = new IPNetworkInterface { InterfaceName = iface.Description, Interface =iface };
				InterfaceList.Add(ipf);
			}
		}
		private List<IPAddress> ipAddresses = new List<IPAddress>();
		/*private void fetchIPofInterface(IPNetworkInterface ini) {
			UnicastIPAddressInformationCollection c = ini.Interface.GetIPProperties().UnicastAddresses;
			foreach (UnicastIPAddressInformation ip in c) {
				ipAddresses.Add(ip.Address);
			}
		}*/

		public abstract class Listener {
			protected IPAddress IP;
			protected byte[] byteBufferData;
			protected Socket socket;

			public abstract void StartListening();

		}
		public class Listener4 : Listener {
			public Listener4(IPAddress ip) {
				this.IP = ip;
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
				byteBufferData = new byte[1024 * 64];
			}
			public override void StartListening() {
				//from ip layer
				socket.Bind(new IPEndPoint(IP, 0));
				socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
				byte[] byteTrue = new byte[4] { 1, 0, 0, 0 };
				byte[] byteOut = new byte[4];
				/* ReceiveAll implies that all incoming and outgoing packets on the interface are captured.
				 * Second option should be TRUE */
				socket.IOControl(IOControlCode.ReceiveAll, byteTrue, byteOut);
				byteBufferData = new byte[1024 * 64];
				//didn't use thread because it is expensive
				socket.BeginReceive(byteBufferData, 0, byteBufferData.Length,
													SocketFlags.None, new AsyncCallback(this.ReceiveData), null);
			}
			private void ReceiveData(IAsyncResult asyncResult) {
				Console.WriteLine("called4");
				int bytesReceived = socket.EndReceive(asyncResult);
				byte[] receivedData = new byte[bytesReceived];

				//copy bytebufferdata to received data, length=bytesrreceived;
				Array.Copy(byteBufferData, 0, receivedData, 0, bytesReceived);
				//MessageBox.Show(Convert.ToBase64String(receivedData));
				Console.WriteLine(Convert.ToBase64String(receivedData));
				//tailed recurrence to next packet
				socket.BeginReceive(byteBufferData, 0, byteBufferData.Length,
														SocketFlags.None, new AsyncCallback(this.ReceiveData), null);

			}
		}
		public class Listener6 : Listener {
			public Listener6(IPAddress ip) {
				this.IP = ip;
				socket = new System.Net.Sockets.Socket(AddressFamily.InterNetworkV6, SocketType.Raw, ProtocolType.IPv6);
				byteBufferData = new byte[1024 * 64];

			}
			public override void StartListening() {

				Console.WriteLine(IP.ToString());
				socket.Bind(new IPEndPoint(IP, 0));
				socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.HeaderIncluded, true);
				byte[] byteTrue = new byte[4] { 1, 0, 0, 0 };
				byte[] byteOut = new byte[4];
				/* ReceiveAll implies that all incoming and outgoing packets on the interface are captured.
				 * Second option should be TRUE */
				socket.IOControl(IOControlCode.ReceiveAll, byteTrue, byteOut);
				byteBufferData = new byte[1024 * 64];
				//didn't use thread because it is expensive
				socket.BeginReceive(byteBufferData, 0, byteBufferData.Length,
													SocketFlags.None, new AsyncCallback(this.ReceiveData), null);
			}
			private void ReceiveData(IAsyncResult asyncResult) {
				Console.WriteLine("6called");
				//from tcp
				int bytesReceived = socket.EndReceive(asyncResult);
				byte[] receivedData = new byte[bytesReceived];
				//copy bytebufferdata to received data, length=bytesrreceived;
				Array.Copy(byteBufferData, 0, receivedData, 0, bytesReceived);
				//MessageBox.Show(Convert.ToBase64String(receivedData));
				Console.WriteLine(Convert.ToBase64String(receivedData));
				//tailed recurrence to next packet
				socket.BeginReceive(byteBufferData, 0, byteBufferData.Length,
														SocketFlags.None, new AsyncCallback(this.ReceiveData), null);

			}
		}
		public Listener GetListener(IPAddress ip) {
			if (ip.AddressFamily == AddressFamily.InterNetwork)
				return new Listener4(ip);
			else return new Listener6(ip);
		}
		public ICommand IStartSniffing { get; private set; }
		public void StartSniffing() {
			//TODO: multi threads
			/*
			if (Interface == null) return;
			fetchIPofInterface(Interface);
			foreach (var ip in ipAddresses) {
				if (ip.AddressFamily == AddressFamily.InterNetworkV6 && ip.Equals(IPAddress.Parse("2400:dd01:103a:4053::10e5"))) {
					//continue;
					Console.WriteLine("6666");
					Listener listener = GetListener(ip);
					listener.StartListening();
					break;
				} else {
					Console.WriteLine(ip.ToString());
					Listener listener = GetListener(ip);
					listener.StartListening();
					//&&ip.Equals(IPAddress.Parse("2400:dd01:103a:4053::10e5"))
					break;
				}
			}	*/
			
		}
		public ViewModel() {
			IStartSniffing = new RelayCommand(() => StartSniffing());
			InterfaceList = new ObservableCollection<IPNetworkInterface>();
			fetchNetworkInterfaceName();
		}
	}
}
