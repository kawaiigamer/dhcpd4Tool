using CommandLine;
using System;
using System.Net;

namespace dhcpd4Tool.cli
{
    public static class DhcpTester
    {
        private static void Main(string[] args)
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
            {
                WriteColored(packet.ToString(), ConsoleColor.Blue);
            }

            long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            DhcpClient.DatagramRecivedEvent += (t, l, p) =>
            {
                Console.WriteLine($"Result from {p.GetServerInformation()} length {l} via {t - startTime} ms");
                if (options.Value.Verbose)
                {
                    WriteColored(p.ToString(), ConsoleColor.DarkRed);
                }

            };
            DhcpClient.DatagramReciveErrorEvent += (e) =>
            {
                WriteColored($"Exception while sending {e.Message}", ConsoleColor.Red);
            };

            DHCPPacket[]  results = DhcpClient.SendDhcpRequest(serverEndPoint, packet, options.Value.Timeout);

            Console.WriteLine($"{results.Length} packets recived via {DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime} ms");
        }

        private static void WriteColored(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static DHCPPacket BuildPacketFromArguments(ParserResult<Options> options)
        {
            DHCPPacket packet = new DHCPPacket();

            if (options.Value.OPRequestType)
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

            foreach (string element in options.Value.OptionsList)
            {
                string[] option = element.Split('=');
                byte optionNumber = byte.Parse(option[0]);
                if (optionNumber == 255 | optionNumber == 53 | optionNumber == 82 |
                    optionNumber == 46 | optionNumber == 54)
                {
                    continue;
                }
                packet.Options[optionNumber] = new DhcpOptionData(ResolveOptionContent(option[1]));
            }

            if (options.Value.CircuitID != null | options.Value.RemoteID != null)
            {
                packet.SetOption82(ResolveOptionContent(options.Value.CircuitID), ResolveOptionContent(options.Value.RemoteID));
            }
            if (options.Value.OptionOverload != 0)
                packet.SetOptionOverload(options.Value.OptionOverload);
            if (options.Value.NetBIOSNodeType != 0)
                packet.SetNetBIOSNodeType(options.Value.NetBIOSNodeType);
            return packet;
        }

        const string IP_MARK = "[ip]";
        const string BYTE_MARK = "[byte]";

        private static byte[] ResolveOptionContent(string optionValue) => optionValue switch
        {
            string v when v.StartsWith(IP_MARK) => ProcessOptionContent(IP_MARK, v, 4, (elements, data, i) =>
            {
                DhcpConvertor.IpToBytes(elements[i]).CopyTo(data, i * 4);
            }),

            string v when v.StartsWith(BYTE_MARK) => ProcessOptionContent(BYTE_MARK, v, 1, (elements, data, i) =>
            {
                data[i] = Convert.ToByte(elements[i], 16);
            }),
            
            null => DhcpConvertor.AsciiToBytes(String.Empty),

            _ => DhcpConvertor.AsciiToBytes(optionValue)
            
        };
        private static byte[] ProcessOptionContent(string mark, string val, int size, Action<string[],byte[],int> action)
        {
            string[] elements = val.Remove(0, mark.Length).Split(',');
            byte[] data = new byte[elements.Length * size];
            for (int i = 0; i < elements.Length; i++)
            {
                action(elements,data,i);
            }
            return data;
        }
    }    
}
