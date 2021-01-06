using System.Collections.Generic;
using CommandLine;


namespace dhcpd4Tool.cli
{
    class Options
    {
        // server 

        [Option('v', "verbose", HelpText = "Prints all datagrams data to stdout", Default = true)]
        public bool Verbose { get; set; }

        [Option('s', "server", HelpText = "Server to send request", Default = "192.168.1.255")]
        public string Server { get; set; }

        [Option('p', "port", HelpText = "Server port", Default = (ushort)67)]
        public ushort Port { get; set; }

        [Option('t', "timeout", HelpText = "Recive timeout (ms)", Default = 1000 * 10)]
        public int Timeout { get; set; }

        // Header values

        [Option("send-reply", HelpText = "Send BOOTREPLY instead BOOTREQUEST", Default = false)]
        public bool OPRequestType { get; set; }

        [Option('h', "htype", HelpText = "Hardware address type, for Ethernet - 0x01", Default = (byte)0x01)]
        public byte Htype { get; set; }

        [Option('l', "hlen", HelpText = "The length of the hardware address in bytes. For Ethernet MAC address — 0x06.", Default = (byte)0x06)]
        public byte Hlen { get; set; }

        [Option("hops", HelpText = "The number of intermediate routers (called DHCP relay agents) through which the message passed", Default = (byte)0x00)]
        public byte Hops { get; set; }

        [Option('x', "xid", HelpText = "Transaction uid", Default = (uint)0x42)]
        public uint XID { get; set; }

        [Option("secs", HelpText = "Time in seconds since the start of the address acquisition process. May not be used", Default = (ushort)0x00)]
        public ushort Secs { get; set; }

        [Option('f', "flags", HelpText = "DHCP flags", Default = (ushort)0x78f2)]
        public ushort Flags  { get; set; }


        // Header ip addresses


        [Option("ciadr", HelpText = "Client IP Address", Default = "0.0.0.0")]
        public string Ciadr { get; set; }

        [Option("yiaddr", HelpText = "Your IP Address", Default = "0.0.0.0")]
        public string Yiaddr { get; set; }

        [Option("siaddr", HelpText = "Server IP Address", Default = "0.0.0.0")]
        public string Siaddr { get; set; }

        [Option("giaddr", HelpText = "Gateway IP Address switched by relay", Default = "0.0.0.0")]
        public string Giaddr { get; set; }

        // Header strings

        [Option('m', "mac", HelpText = "Client hardware address usually a mac address (CHADDR)", Default = "00:00:00:00:00:00")]
        public string Mac { get; set; }

        [Option("sname", HelpText = "An optional server name as null terminated string", Default = "")]
        public string Sname { get; set; }

        [Option("file", HelpText = "An optional file name on the server, used by diskless workstations for remote booting. Like sname, it is represented as a null-terminated string.", Default = "")]
        public string File { get; set; }


        // Options


        [Option('y', "мessage-type", HelpText = "DHCP Message type", Default = DHCPMessageType.DHCPDISCOVER)]
        public DHCPMessageType MessageType { get; set; }

        [Option('o', "options", HelpText = "List OF DHCP options", Separator = ' ')]
        public IEnumerable<string> OptionsList { get; set; }


        [Option("option-overload", HelpText = "Used in option 52 to indicate that the DHCP 'sname' or 'file' fields are being overloaded by using them to carry DHCP options")]
        public DHCPOptionOverload OptionOverload { get; set; }

        [Option("netbios-node-type", HelpText = "The NetBIOS node type (option 46)")]
        public DHCPNetBIOSNodeType NetBIOSNodeType { get; set; }



        [Option("сircuit-id", HelpText = "Agent Circuit ID Sub-option for DHCP option 82")]
        public string CircuitID { get; set; }

        [Option("remote-id", HelpText = "Agent Remote ID Sub-option for DHCP option 82")]
        public string RemoteID { get; set; }



    }
}
