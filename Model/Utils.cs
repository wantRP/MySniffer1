using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySniffer1.Model {
	internal class Utils {
		static public string BytetoHex(byte[] Bytes) {
      StringBuilder Result = new StringBuilder(Bytes.Length * 2);
      string HexAlphabet = "0123456789ABCDEF";

      foreach (byte B in Bytes) {
        Result.Append(HexAlphabet[(int)(B /16)]);
        Result.Append(HexAlphabet[(int)(B % 16)]);
      }

      return Result.ToString();
    }
    public static string BytetoVisibleString(byte[] Bytes) {
      StringBuilder Result = new StringBuilder(Bytes.Length);
      foreach (byte b in Bytes) { 
        Result.Append(32<=b&&b<=126?(char)b:'.');
      }
      return Result.ToString();
    }
	}
}
