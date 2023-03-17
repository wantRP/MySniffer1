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
using System.Threading;
using MySniffer1.ViewModel;
namespace MySniffer1 {
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	
	public partial class MainWindow : Window {
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
			//string[] str = GetNetworkInterfaceName();
			

		}

		private void comboBoxInterfaces_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			//Console.WriteLine(SelectedInterface);
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			//SelectedInterface.ToString();
			
		}
	}
}
