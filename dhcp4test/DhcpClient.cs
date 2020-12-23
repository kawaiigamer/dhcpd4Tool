using System.Net;
using System.Net.Sockets;
using System;


namespace dhcpd4Client
{
    public static class DhcpClient
    {

        public static DHCPPacket SendDhcpRequest(IPEndPoint endPoint, DHCPPacket packet)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

            IPEndPoint bindPoint = new IPEndPoint(IPAddress.Any, 68);
            sock.Bind(bindPoint);

            sock.SendTo(packet.ToArray(), endPoint);
            EndPoint ep = (EndPoint)bindPoint;
            byte[] data = new byte[512];
            int recv = sock.ReceiveFrom(data, ref ep);
            Array.Resize(ref data, recv);

            sock.Shutdown(SocketShutdown.Both);
            sock.Close();

            return DHCPPacket.FromArray(data);
        }
    }
}
