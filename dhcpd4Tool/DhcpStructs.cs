using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;


namespace dhcpd4Tool
{
    public enum DHCPMessageType: byte
    {
        DHCPDISCOVER = 1,
        DHCPOFFER,
        DHCPREQUEST,
        DHCPDECLINE,
        DHCPACK,
        DHCPNAK,
        DHCPRELEASE,
        DHCPINFORM,
        DHCPFORCERENEW,
        DHCPLEASEQUERY,
        DHCPLEASEUNASSIGNED,
        DHCPLEASEUNKNOWN,
        DHCPLEASEACTIVE,
        DHCPBULKLEASEQUERY,
        DHCPLEASEQUERYDONE,
        DHCPACTIVELEASEQUERY,
        DHCPLEASEQUERYSTATUS,
        DHCPTLS
    }
    public enum DHCPMessageOP : byte
    {
        BOOTREQUEST = 1, BOOTREPLY
    }

    public class DhcpOptionData
    {
        private byte[] _data;
        public byte[] Data
        {
            get { return _data; }
            set
            {
                if (value.Length > byte.MaxValue)
                    throw new InvalidDataException($"Dhcp option length is too big {value.Length}");
                _data = value;
            }
        }

        public DhcpOptionData(params byte[] val) => Data = val;
        public DhcpOptionData(string ascii) => Data = DhcpConvertor.AsciiToBytes(ascii);
        public DhcpOptionData(params IPAddress[] addresses)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            foreach (IPAddress address in addresses)
            {
                writer.Write(address.GetAddressBytes());
            }
            Data = stream.ToArray();
        }
    }


    public class DHCPPacket
    {
        public DHCPMessageOP OP = DHCPMessageOP.BOOTREQUEST; // BOOTREQUEST (0x01, запрос от клиента к серверу) и BOOTREPLY (0x02, ответ от сервера к клиенту)
        public byte HTYPE = 0x01;              // Тип аппаратного адреса. Например, для MAC-адреса Ethernet это поле принимает значение 0x01
        public byte HLEN = 0x06;               // Длина аппаратного адреса в байтах. Для MAC-адреса Ethernet 0x06.
        public byte HOPS = 0x00;               // Количество промежуточных маршрутизаторов (так называемых агентов ретрансляции DHCP), через которые прошло сообщение. Клиент устанавливает это поле в 0x00.
        public uint XID = 0x42;                // Уникальный идентификатор транзакции в 4 байта, генерируемый клиентом в начале процесса получения адреса.
        public ushort SECS = 0x0000;           // Время в секундах с момента начала процесса получения адреса. Может не использоваться(в этом случае оно устанавливается в 0x0000).
        public ushort FLAGS = 0x78f2;          // Поле для флагов — специальных параметров протокола DHCP. 1 - для широковещательной рассылки.
        public byte[] CIADDR = new byte[4];    // IP-адрес клиента. Заполняется только в том случае, если клиент уже имеет собственный IP-адрес, Если GIADDR = 0, а поле CIADDR отлично от нуля, сервер передает сообщения DHCPOFFER и DHCPACK по индивидуальному адресу CIADDR. 
        public byte[] YIADDR = new byte[4];    // Новый IP-адрес клиента, предложенный сервером.
        public byte[] SIADDR = new byte[4];    // IP-адрес сервера. Возвращается в предложении DHCP (см. ниже).
        public byte[] GIADDR = new byte[4];    // IP-адрес агента ретрансляции, если таковой участвовал в процессе доставки сообщения DHCP до сервера. Если поле GIADDR в сообщении DHCP от клиента отлично от 0, сервер передает все отклики на сообщения в порт DHCP server транслятору BOOTP, адрес которого указан в поле GIADDR.
        public byte[] CHADDR = new byte[16];   // Аппаратный адрес (обычно MAC-адрес) клиента.
        public byte[] SNAME = new byte[64];    // Необязательное имя сервера в виде нуль-терминированной строки.
        public byte[] FILE = new byte[128];    // Необязательное имя файла на сервере, используемое бездисковыми рабочими станциями
        public byte[] OPTION_COOKIE = new byte[] { 99, 130, 83, 99 };    // DHCP option magic cookie
        public SortedDictionary<byte, DhcpOptionData> Options = new SortedDictionary<byte, DhcpOptionData>(); // Опции

        public DHCPPacket()
        {
            SetMessageType(DHCPMessageType.DHCPDISCOVER);
            Options[255] = new DhcpOptionData(0);
        }

        public byte[] ToArray()
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);

            writer.Write((byte)OP);
            writer.Write(HTYPE);
            writer.Write(HLEN);
            writer.Write(HOPS);
            writer.Write(XID);
            writer.Write(SECS);
            writer.Write(FLAGS);
            writer.Write(CIADDR);
            writer.Write(YIADDR);
            writer.Write(SIADDR);
            writer.Write(GIADDR);
            writer.Write(CHADDR);
            writer.Write(SNAME);
            writer.Write(FILE);
            writer.Write(OPTION_COOKIE);
 
            foreach (KeyValuePair<byte, DhcpOptionData> option in Options)
            {
                writer.Write(option.Key);
                writer.Write((byte)option.Value.Data.Length);
                writer.Write(option.Value.Data);
            }

            return stream.ToArray();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("\n");
            sb.AppendLine("------------");
            sb.AppendLine("DHCPPacket");
            sb.AppendLine("------------");
            sb.AppendLine($@"{OP} - 0x{OP.ToString("X")}");
            sb.AppendLine($@"HTYPE  - 0x{HTYPE.ToString("X")}");
            sb.AppendLine($@"HOPS   - 0x{HOPS.ToString("X")}");
            sb.AppendLine($@"HLEN   - 0x{HLEN.ToString("X")}");
            sb.AppendLine($@"XID    - 0x{XID.ToString("X")}");
            sb.AppendLine($@"SECS   - 0x{SECS.ToString("X")}");
            sb.AppendLine($@"FLAGS  - 0x{FLAGS.ToString("X")}");
            sb.AppendLine($"CIADDR - {DhcpConvertor.BytesToIP(CIADDR)}");
            sb.AppendLine($"YIADDR - {DhcpConvertor.BytesToIP(YIADDR)}");
            sb.AppendLine($"SIADDR - {DhcpConvertor.BytesToIP(SIADDR)}");
            sb.AppendLine($"GIADDR - {DhcpConvertor.BytesToIP(GIADDR)}");
            sb.AppendLine($"CHADDR - {String.Concat(CHADDR)}");
            sb.AppendLine($"SNAME  - {String.Concat(SNAME)}");
            sb.AppendLine($"FILE   - {String.Concat(FILE)}");

            foreach (KeyValuePair<byte, DhcpOptionData> option in Options)
            {
               sb.AppendLine($"Option #{option.Key} len({option.Value.Data.Length}) - {String.Join(' ', option.Value.Data)} ASCII({DhcpConvertor.BytesToAscii(option.Value.Data)})");
            }

            return sb.ToString();
        }

        public static DHCPPacket FromArray(byte[] data)
        {
            const int HEADER_LENGTH = 236 + 4; // Header + option cookie
            DHCPPacket result = new DHCPPacket
            {
                OP = (DHCPMessageOP)data[0], 
                HTYPE = data[1],
                HLEN = data[2],
                HOPS = data[3],
                XID = BitConverter.ToUInt32(data,4),
                SECS = BitConverter.ToUInt16(data, 8),
                FLAGS = BitConverter.ToUInt16(data, 10),
                CIADDR = data.Skip(12).Take(4).ToArray(),
                YIADDR = data.Skip(16).Take(4).ToArray(),
                SIADDR = data.Skip(20).Take(4).ToArray(),
                GIADDR = data.Skip(24).Take(4).ToArray(),
                CHADDR = data.Skip(28).Take(16).ToArray(),
                SNAME = data.Skip(44).Take(64).ToArray(),
                FILE = data.Skip(108).Take(128).ToArray()
            };

            for (int i = HEADER_LENGTH; i < data.Length; i++)
            {
                if (data[i] == 255)
                    break;
                result.Options[data[i]] = new DhcpOptionData(data.Skip(i + 2).Take(data[i + 1]).ToArray());
                i += data[i + 1] + 1;
            }
            return result;
        }

        public void SetMessageType(DHCPMessageType T) => Options[53] = new DhcpOptionData((byte)T);
        
        public DHCPMessageType GetMessageType() => (DHCPMessageType)Options[53].Data[0];

        public string GetServerInformation() => DhcpConvertor.BytesToIP(Options[54].Data) ?? "unknown";

        public void SetCHADDR(string val) => SetCHADDR(DhcpConvertor.MacToBytes(val));

        public void SetSNAME(string val) => SetSNAME(DhcpConvertor.AsciiToBytes(val));

        public void SetFILE(string val) => SetFILE(DhcpConvertor.AsciiToBytes(val));

        public void SetCHADDR(params byte[] val)
        {
            if (val.Length > CHADDR.Length)
                throw new InvalidDataException($"CHADDR({CHADDR.Length}) length is too big {val.Length}");
            val.CopyTo(CHADDR, 0);
        }

        public void SetSNAME(params byte[] val)
        {
            if (val.Length > SNAME.Length)
                throw new InvalidDataException($"SNAME({SNAME.Length}) length is too big {val.Length}");
            val.CopyTo(SNAME, 0);
        }

        public void SetFILE(params byte[] val)
        {
            if (val.Length > FILE.Length)
                throw new InvalidDataException($"FILE({FILE.Length}) length is too big {val.Length}");
            val.CopyTo(FILE, 0);
        }

        public void SetOption82(string CircuitID, string RemoteID)
        {
            SetOption82(DhcpConvertor.AsciiToBytes(CircuitID), DhcpConvertor.AsciiToBytes(RemoteID));
        }

        public void SetOption82(byte[] CircuitID, byte[] RemoteID)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write((byte)0x01);
            writer.Write((byte)CircuitID.Length);
            writer.Write(CircuitID);
            writer.Write((byte)0x02);
            writer.Write((byte)RemoteID.Length);
            writer.Write(RemoteID);
            Options[82] = new DhcpOptionData(stream.ToArray());
        }

    }
}
