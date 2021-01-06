using System;
using System.Linq;
using System.Text;

namespace dhcpd4Tool
{
    public static class DhcpConvertor
    {        
        public static byte[] IpToBytes(string val) => val.Split('.').Select(x => Convert.ToByte(x, 10)).ToArray();
        public static byte[] MacToBytes(string val) => val.Split(':').Select(x => Convert.ToByte(x, 16)).ToArray();
        public static byte[] AsciiToBytes(string val) => Encoding.ASCII.GetBytes(val);
        public static string BytesToAscii(byte[] val) => new String(Encoding.ASCII.GetString(val).Where<char>( x => x > 31).ToArray());
        public static string BytesToIP(byte[] val) => String.Join('.', val);
    }
}
