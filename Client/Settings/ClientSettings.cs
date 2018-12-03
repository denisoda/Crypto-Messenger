using System.Net;
using System.Net.Sockets;

namespace Server.Settings
{
    public static class ClientSettings
    {
        //Socket settings
        public const int Port = 1489;
        public static readonly SocketType SocketType = SocketType.Stream;
        public static readonly ProtocolType ProtocolType = ProtocolType.Tcp;
        public static readonly IPHostEntry IpHostInfo = Dns.GetHostEntry("localhost");
        public static readonly IPAddress IpAddress = IPAddress.Parse("127.0.0.1");
        public static readonly AddressFamily AddressFamily = AddressFamily.InterNetwork;
        public static readonly IPEndPoint RemoteEp = new IPEndPoint(IpAddress, Port);
        public static readonly int MaxConnectionsNumber = 100;

        //Receiving Settings
        public const int BufferSize = 256;

        //Cryptography
        public static readonly byte[] PublicKey = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
    }
}