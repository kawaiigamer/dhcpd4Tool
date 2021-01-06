using System.Net;
using System.Net.Sockets;
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace dhcpd4Tool
{
    public static class DhcpClient
    {
        public delegate void DatagramRecivedNotification(long time, int len, DHCPPacket p);
        public static event DatagramRecivedNotification DatagramRecivedEvent;

        public static DHCPPacket[] SendDhcpRequest(IPEndPoint endPoint, DHCPPacket packet, int timeoutMs = 1000 * 10)
        {
            IList<DHCPPacket> recivedPackets = new List<DHCPPacket>();
            IPEndPoint bindPoint = new IPEndPoint(IPAddress.Any, 68);

            UdpClient udpClient = new UdpClient(AddressFamily.InterNetwork);
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            udpClient.Client.Bind(bindPoint);

            byte[] datagram = packet.ToArray();
            udpClient.Send(datagram, datagram.Length, endPoint);      

            EventWaitHandle waitHandle = new AutoResetEvent(false);
            udpClient.BeginReceive(new AsyncCallback(OnMessageRecieved), null);
            waitHandle.WaitOne(timeoutMs);
            
            void OnMessageRecieved(IAsyncResult ar)
            { 
                Byte[] receivedDatagram = udpClient.EndReceive(ar, ref bindPoint);
                DHCPPacket receivedPacket = DHCPPacket.FromArray(receivedDatagram);
                recivedPackets.Add(receivedPacket);
                DatagramRecivedEvent?.Invoke(DateTimeOffset.Now.ToUnixTimeMilliseconds(), receivedDatagram.Length, receivedPacket);
                if (udpClient.Available == 0)
                {
                    waitHandle.Set();
                    return;
                }
                udpClient.BeginReceive(new AsyncCallback(OnMessageRecieved), null);
            }

            udpClient.Client.Shutdown(SocketShutdown.Both);
            udpClient.Close();

            return recivedPackets.ToArray();
        }
    }
}