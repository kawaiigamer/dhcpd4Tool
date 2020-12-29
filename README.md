## dhcpd4Tool

**dhcpd4Tool** its a simple tool for testing `DHCP` servers.


```
dhcpd4Tool 1.0.0
Copyright (C) 2020 dhcpd4Tool

  -v, --verbose         (Default: false) Prints all debug messages

  -s, --server          (Default: 192.168.1.255) Server to send request

  -p, --port            (Default: 67) Server port

  -r, --reply-send      (Default: false) Send BOOTREPLY instead BOOTREQUEST

  -h, --htype           (Default: 1) Hardware address type, for Ethernet - 0x01

  -l, --hlen            (Default: 6) The length of the hardware address in bytes. For Ethernet MAC address - 0x06.

  --hops                (Default: 0) The number of intermediate routers (called DHCP relay agents) through which the
                        message passed

  -x, --xid             (Default: 66) Transaction uid

  --secs                (Default: 0) Time in seconds since the start of the address acquisition process. May not be used

  -f, --flags           (Default: 30962) DHCP flags

  --ciadr               (Default: 0.0.0.0) Client IP Address

  --yiaddr              (Default: 0.0.0.0) Your IP Address

  --siaddr              (Default: 0.0.0.0) Server IP Address

  --giaddr              (Default: 0.0.0.0) Gateway IP Address switched by relay

  -m, --mac             (Default: 00:00:00:00:00:00) Client hardware address usually a mac address (CHADDR)

  --sname               An optional server name as null terminated string

  --file                An optional file name on the server, used by diskless workstations for remote
                        booting. Like sname, it is represented as a null-terminated string.

  -t, --мessage-type    (Default: DHCPDISCOVER) DHCP Message type

  -o, --options         List OF DHCP options

  --сircuit-id          Agent Circuit ID Sub-option for DHCP option 82

  --remote-id           Agent Remote ID Sub-option for DHCP option 82

  --help                Display this help screen.

  --version             Display version information.
```

## Usage

```bash
dhcpd4Tool -s 192.168.1.100 -p 69 -f 0 -v -m 8A:F5:85:13:6A:DC \
-o "12=my-workstation 7=[ip]10.245.67.1,225.71.66.12 20=[byte]0x01 26=[byte]0x64,0xC" \
--сircuit-id "245" --remote-id "relay#37"
```

