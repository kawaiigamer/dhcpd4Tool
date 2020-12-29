using CommandLine;
using System;
using System.Net;

namespace dhcpd4Tool.cli
{
    public static class DhcpTester
    {

        static void Main(string[] args)
        {



            var options = CommandLine.Parser.Default.ParseArguments<Options>(args);
            if (options.Value == null)
            {
                return;
            }
            DHCPPacket packet = BuildPacketFromArguments(options);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(options.Value.Server), options.Value.Port);

            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} Sending {options.Value.MessageType} request via dhcpd4Tool-> {serverEndPoint}");
            if (options.Value.Verbose)
                Console.WriteLine(packet.ToString());




        }

        static DHCPPacket BuildPacketFromArguments(ParserResult<Options> options)
        {
            DHCPPacket packet = new DHCPPacket();

            if (options.Value.RequestType)
            {
                packet.OP = DHCPMessageOP.BOOTREPLY;
            }

            packet.HTYPE = options.Value.Htype;
            packet.HLEN = options.Value.Hlen;
            packet.HOPS = options.Value.Hops;
            packet.XID = options.Value.XID;
            packet.SECS = options.Value.Secs;
            packet.FLAGS = options.Value.Flags;

            packet.CIADDR = DhcpConvertor.IpToBytes(options.Value.Ciadr);
            packet.YIADDR = DhcpConvertor.IpToBytes(options.Value.Yiaddr);
            packet.SIADDR = DhcpConvertor.IpToBytes(options.Value.Siaddr);
            packet.GIADDR = DhcpConvertor.IpToBytes(options.Value.Giaddr);

            packet.SetCHADDR(options.Value.Mac);
            packet.SetSNAME(options.Value.Sname);
            packet.SetFILE(options.Value.File);

            packet.SetMessageType(options.Value.MessageType);

            const string IP_MARK = "[ip]";
            const string BYTE_MARK = "[byte]";

            foreach (string element in options.Value.OptionsList)
            {
                string[] option = element.Split('=');
                byte optionNumber = byte.Parse(option[0]);
                if (optionNumber == 255 | optionNumber == 53 | optionNumber == 82)
                    continue;
                if(option[1].StartsWith(IP_MARK))
                {
                    string[] ips = option[1].Remove(0, IP_MARK.Length).Split(',');
                    byte[] data = new byte[ips.Length * 4];
                    for (int i = 0; i < ips.Length; i++)
                    {
                        DhcpConvertor.IpToBytes(ips[i]).CopyTo(data, i * 4);
                    }
                    packet.Options[optionNumber] = new DhcpOptionData(data);
                    continue;
                }
                if (option[1].StartsWith(BYTE_MARK))
                {
                    string[] bytes = option[1].Remove(0, BYTE_MARK.Length).Split(',');
                    byte[] data = new byte[bytes.Length];
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        data[i] = Convert.ToByte(bytes[i], 16);
                    }
                    packet.Options[optionNumber] = new DhcpOptionData(data);
                    continue;
                }
                packet.Options[optionNumber] = new DhcpOptionData(DhcpConvertor.AsciiToBytes(option[1]));
            }

            if (options.Value.CircuitID != null | options.Value.RemoteID != null)
            {
                packet.SetOption82(options.Value.CircuitID == null ? String.Empty : options.Value.CircuitID,
                                   options.Value.RemoteID == null ? String.Empty : options.Value.RemoteID);
            }
            return packet;
        }

    }
}



