using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MySniffer1.ViewModel {
	public class ViewModel {
		public ObservableCollection<IPNetworkInterface> InterfaceList { get; set; }
		public class IPNetworkInterface {
			public string InterfaceName { get; set; }
			public NetworkInterface Interface  { get; set; }
		}
		public IPNetworkInterface Interface { get; set; }
		private void GetNetworkInterfaceName() {
			NetworkInterface[] ni = NetworkInterface.GetAllNetworkInterfaces();
			string[] strl = new string[ni.Length];
			for (int i = 0; i < ni.Length; ++i) {
				string ipstr = ni[i].Name;
				//Console.WriteLine(ipstr);
				IPNetworkInterface ipf = new IPNetworkInterface { InterfaceName = ipstr, Interface = ni[i] };
				//Console.WriteLine(i);
				InterfaceList.Add(ipf);
				strl[i] = ni[i].Name;
			}
		}
		private List<IPAddress> ipAddresses=new List<IPAddress>();
		private void fetchIPofInterface(IPNetworkInterface ini) {
			UnicastIPAddressInformationCollection c = ini.Interface.GetIPProperties().UnicastAddresses;
			foreach (UnicastIPAddressInformation ip in c) {
				ipAddresses.Add(ip.Address);
			}
			 
		}
		public ICommand IStartSniffing { get; private set; }
		public void StartSniffing() {
			if (Interface == null) return;
			fetchIPofInterface(Interface);
		}
		public ViewModel() {
			IStartSniffing = new RelayCommand(() => StartSniffing());
			InterfaceList = new ObservableCollection<IPNetworkInterface>();
			GetNetworkInterfaceName();
		}
	}
}
