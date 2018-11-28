using System.Net;
using System.Net.Sockets;

namespace Server.Settings
{
    public static class ServerSettings
    {
        //Socket settings
        public static readonly ProtocolType ProtocolType = ProtocolType.Tcp;
        public static readonly SocketType SocketType = SocketType.Stream;
        public static readonly AddressFamily AddressFamily = AddressFamily.InterNetwork;
        public static readonly IPEndPoint IpEndPoint =
            new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1489);
        public static readonly int MaxConnectionsNumber = 100;

        //Transmitting settings
        public const int BufferSize = 1024;
    }
}