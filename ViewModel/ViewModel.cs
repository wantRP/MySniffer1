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
using MySniffer1.Model;
namespace MySniffer1.ViewModel {
	public class ViewModel {
		public ObservableCollection<IPNetworkInterface> InterfaceList { get; set; }
		public class IPNetworkInterface {
			public string InterfaceName { get; set; }
			public ILiveDevice Interface { get; set; }
		}
		public IPNetworkInterface SelectedInterface { get; set; }
		public RawPacket SelectedPacket { get; set; }
		private PacketArrivalEventHandler arrivalEventHandler;
		private ObservableCollection<RawCapture> PacketQueue = new ObservableCollection<RawCapture>();
		public ObservableCollection<RawPacket> Packets { get; set; }//binded
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
				IPNetworkInterface ipf = new IPNetworkInterface { InterfaceName = iface.Description, Interface = iface };
				InterfaceList.Add(ipf);
			}
		}
		private List<IPAddress> ipAddresses = new List<IPAddress>();
		private int packetCount = 0;
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
		public ICommand IStopSniffing { get; private set; }
		public ICommand IClearList { get; private set; }
		private ICaptureDevice device;
		private void StartSniffing() {
			if (SelectedInterface == null) return;
			 device = SelectedInterface.Interface;
			Queue<RawPacket> packetStrings = new Queue<RawPacket>();
			arrivalEventHandler = new PacketArrivalEventHandler(device_OnPacketArrival);
			device.OnPacketArrival += arrivalEventHandler;
			device.Open();
			device.StartCapture();
		}
		void device_OnPacketArrival(object sender, PacketCapture e) {
			// print out periodic statistics about this device
			++packetCount;
			//DateTime now = DateTime.Now.TimeOfDay // cache 'DateTime.Now' for minor reduction in cpu overhead
			// lock QueueLock to prevent multiple threads accessing PacketQueue at
			// the same time
			RawCapture pac = e.GetPacket();
			//PacketQueue.Add(pac);
			System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
				Packets.Add(new RawPacket(pac, packetCount));
			}));
			//Console.WriteLine(Convert.ToBase64String(pac.Data) + "\n");
		}

		public ViewModel() {
			IStartSniffing = new RelayCommand(() => StartSniffing());
			IStopSniffing = new RelayCommand(() => StopSniffing());
			IClearList = new RelayCommand(() => ClearList());
			InterfaceList = new ObservableCollection<IPNetworkInterface>();
			Packets = new ObservableCollection<RawPacket>();
			fetchNetworkInterfaceName();
		}

		private void ClearList() {
			this.Packets.Clear();
		}

		private void StopSniffing() {
			device.StopCapture();
			device.Close();
		}
	}
}
