using System.Net;
using System.Net.Sockets;

namespace Server.Settings
{
    public static class ServerSettings
    {
        //Socket settings
        public static readonly ProtocolType ProtocolType = ProtocolType.Tcp;
        public static readonly SocketType SocketType = SocketType.Stream;
        public static readonly AddressFamily AddressFamily = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].AddressFamily;
        public static readonly IPEndPoint IpEndPoint =
            new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0], 1488);
        public static readonly int MaxConnectionsNumber = 100;

        //StateObject
        public const int BufferSize = 1024;
    }
}