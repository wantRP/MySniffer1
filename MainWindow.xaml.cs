using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;

namespace MySniffer1 {
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>

	public partial class MainWindow : Window {
		public ObservableCollection<IPNetworkInterface> InterfaceList { get; private set; }
		public  class IPNetworkInterface{
			public string InterfaceName { get; set; }
		}
		public string[] GetNetworkInterfaceName() {
			NetworkInterface[] ni = NetworkInterface.GetAllNetworkInterfaces();
			string[] strl = new string[ni.Length];
			for (int i = 0; i < ni.Length; ++i) {
				strl[i] = ni[i].Name;
			}
			return strl;
		}
		public MainWindow() {
			InitializeComponent();
			IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
			socket.Bind(new IPEndPoint(iPAddress, 0));
			socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
			/*foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces()) {
				if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet) {
					Console.WriteLine(ni.Name);
					foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses) {
						if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
							Console.WriteLine(ip.Address.ToString());
						}
					}
				}*/
			string[] str = GetNetworkInterfaceName();
			foreach (string s in str) {
				Console.WriteLine(s);

			}

		}

	}
}
