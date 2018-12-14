using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Server.Settings;

namespace Server.Model
{
    public class Client
    {
        public TcpClient Socket { get; set; }
        public RSAParameters PublicKey { get; set; }
    }
}