using NUnit.Framework;
using System.Net;
using dhcpd4Tool;


namespace dhcpd4ToolTests
{
    public class ServerTests
    {
        public IPEndPoint server;

        [OneTimeSetUp]
        public void Setup()
        {
            this.server = new IPEndPoint(IPAddress.Parse("192.168.1.101"), 67);
        }

        [Test]
        /// <summary> ��������� ��� �� ������ </summary>
        public void AliveTest() 
        {
            DHCPPacket pack = new DHCPPacket();
            DHCPPacket result = DhcpClient.SendDhcpRequest(server, pack)[0];
            Assert.AreEqual(result.OP, DHCPMessageOP.BOOTREPLY);
        }

        [Test]
        /// <summary> ��������� ������ �� ������ DHCPOFFER � ����� 53 </summary>
        public void OfferTest()
        {
            DHCPPacket pack = new DHCPPacket();
            DHCPPacket result = DhcpClient.SendDhcpRequest(server, pack)[0];
            Assert.AreEqual(result.GetMessageType(), DHCPMessageType.DHCPOFFER);
        }

        [Test]
        /// <summary> ��������� ��������� �� ������ ������ ����������� � ���� </summary>
        public void MACBindingTest()
        {
            byte[] expectedAdress = { 192, 168, 1, 29 };
            DHCPPacket pack = new DHCPPacket();
            pack.SetCHADDR("8A:F5:85:13:6A:DC");
            DHCPPacket result = DhcpClient.SendDhcpRequest(server, pack)[0];
            Assert.AreEqual(expectedAdress, result.YIADDR); 
        }

        [Test]
        /// <summary> ��������� ��������� �� ������ ������ �����������  � CircuitID � AgentRemoteID </summary>
        public void Option82BindingTest()
        {
            byte[] expectedAdress = { 192, 168, 1, 50 };
            DHCPPacket pack = new DHCPPacket();
            pack.SetOption82("circuit_id_test", "remote_id_test");
            DHCPPacket result = DhcpClient.SendDhcpRequest(server, pack)[0];
            Assert.AreEqual(expectedAdress, result.YIADDR);
        }

        [Test]
        /// <summary> ���������� ��������� ����������, ��������� broadcast ���� � ������, ���� ������ ������������ �� ������ � CIADDR</summary>
        public void BroadcastTest()
        {
            DHCPPacket pack = new DHCPPacket();
            pack.FLAGS = 0x00;
            pack.CIADDR = DhcpConvertor.IpToBytes("192.168.1.10");
            DHCPPacket result = DhcpClient.SendDhcpRequest(new IPEndPoint(IPAddress.Parse("192.168.1.255"), 67), pack)[0];
            Assert.AreEqual(result.GetMessageType(), DHCPMessageType.DHCPOFFER);
        }

        [Test]
        /// <summary> ��������� � ���� �� ������� ������ �����</summary>
        public void TargetTest()
        {
            DHCPPacket pack = new DHCPPacket();
            DHCPPacket result = DhcpClient.SendDhcpRequest(server, pack)[0];
            Assert.AreEqual(result.GetServerInformation(), server.Address.ToString());            
        }
    }
}