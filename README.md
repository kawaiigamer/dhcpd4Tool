## dhcpd4Tool

**dhcpd4Tool** its a simple tool for testing `DHCP` servers. 


```
dhcpd4Tool 1.0.0
Copyright (C) 2021 dhcpd4Tool

  -v, --verbose          (Default: true) Prints all datagrams data to stdout

  -s, --server           (Default: 192.168.1.255) Server to send request

  -p, --port             (Default: 67) Server port

  -t, --timeout          (Default: 10000) Recive timeout (ms)

  -h, --htype            (Default: 1) Hardware address type, for Ethernet - 0x01

  -l, --hlen             (Default: 6) The length of the hardware address in bytes. For Ethernet MAC address - 0x06.
  
  -x, --xid              (Default: 66) Transaction uid
  
  -f, --flags            (Default: 30962) DHCP flags
  
  -m, --mac              (Default: 00:00:00:00:00:00) Client hardware address usually a mac address (CHADDR)
  
  -y, --мessage-type     (Default: DHCPDISCOVER) DHCP Message type
  
  -o, --options          List OF DHCP options
  
  --send-reply           (Default: false) Send BOOTREPLY instead BOOTREQUEST

  --hops                 (Default: 0) The number of intermediate routers (called DHCP relay agents) through which the
                         message passed

  --secs                 (Default: 0) Time in seconds since the start of the address acquisition process. May not be
                         used

  --ciadr                (Default: 0.0.0.0) Client IP Address

  --yiaddr               (Default: 0.0.0.0) Your IP Address

  --siaddr               (Default: 0.0.0.0) Server IP Address

  --giaddr               (Default: 0.0.0.0) Gateway IP Address switched by relay 

  --sname                An optional server name as null terminated string

  --file                 An optional file name on the server, used by diskless workstations for remote
                         booting. Like sname, it is represented as a null-terminated string. 

  --option-overload      Used in option 52 to indicate that the DHCP 'sname' or 'file' fields are being overloaded by using                              them to carry DHCP options.

  --netbios-node-type    The NetBIOS node type (option 46)

  --сircuit-id           Agent Circuit ID Sub-option for DHCP option 82

  --remote-id            Agent Remote ID Sub-option for DHCP option 82

  --help                 Display this help screen.

  --version              Display version information.
```

## Usage

The options separator is `space`.

If no type is specified at the beginning of the value, the value will be treated as an ASCII string.

Valid value types:

- **[byte]** - sequence of single bytes, using `,` as separator.
- **[ip]** - sequence of IPv4 addresses, using `,` as separator.

Example:

```bash
dhcpd4Tool -s 192.168.1.100 -p 69 -f 0 -m 8A:F5:85:13:6A:DC \
-o "12=my-workstation 7=[ip]10.245.67.1,225.71.66.12 20=[byte]0x01 26=[byte]0x64,0xC" \
--сircuit-id "[byte]0x56" --remote-id "relay#37"
```

## TODO

In version `1.0.0` some special options are not supported yet or not fully supported.