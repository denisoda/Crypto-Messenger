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

        //Cryptography
        public static readonly byte[] Key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
        public static readonly byte[] IV = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
    }
}