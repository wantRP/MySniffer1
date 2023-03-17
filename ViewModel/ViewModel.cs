using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySniffer1.ViewModel {
	public class ViewModel {
		public ObservableCollection<IPNetworkInterface> InterfaceList = new ObservableCollection<IPNetworkInterface>();
		public class IPNetworkInterface {
			public string InterfaceName { get; set; }
		}
		public ViewModel() { }
	}
}
