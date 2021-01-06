using NUnit.Framework;
using dhcpd4Tool;


namespace dhcpd4ToolTests
{
    public class InternalTests 
    {
        [Test]
        /// <summary> Проверяет DHCPPacket == DHCPPacket.FromArray(DHCPPacket.ToArray())  </summary>
        public void ReverseTest()
        {
            DHCPPacket pack = new DHCPPacket();
            pack.SetMessageType(DHCPMessageType.DHCPREQUEST);
            pack.Options[12] = new DhcpOptionData("test hostname");
            pack.SetOption82("test", "test2");
            Assert.AreEqual(pack.ToString(), DHCPPacket.FromArray(pack.ToArray()).ToString());
        }
    }
}