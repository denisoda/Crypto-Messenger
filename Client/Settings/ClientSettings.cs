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
    }
}