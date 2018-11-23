using System.Net.Sockets;
using System.Text;
using Server.Settings;

namespace Server.Model
{
    public class StateObject
    {
        public Socket WorkSocket = null;
        public const int BufferSize = ServerSettings.BufferSize;
        public byte[] Buffer = new byte[BufferSize];
        public StringBuilder Sb = new StringBuilder();
    }
}