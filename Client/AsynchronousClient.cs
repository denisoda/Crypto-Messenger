using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server.Settings;

namespace Client
{
    public class AsynchronousClient : IAsyncClient, IDisposable
    {
        private const int port = ClientSettings.Port;
        private static readonly TcpClient client = new TcpClient();
        private static readonly string response;
        private static readonly IPAddress ipAddress = ClientSettings.IpAddress;
        private static Task receiveDateTask;
        private static NetworkStream ns;

        public async Task StartClient()
        {
            try
            {
                await client.ConnectAsync(ipAddress, port);
                Console.WriteLine($"Connected to {ClientSettings.RemoteEp}");
                receiveDateTask = new Task(() => ReceiveData(client));
                receiveDateTask.Start();
                ns = client.GetStream();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveData(TcpClient client)
        {
            NetworkStream networkStream = client.GetStream();
            byte[] receivedBytes = new byte[1024];
            int byte_count;

            while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                Console.Write($"Response from server: {Encoding.ASCII.GetString(receivedBytes, 0, byte_count)}");
            }
        }

        public async Task Send(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new Exception("Empty string");
            }
            var s = message;
            var buffer = Encoding.ASCII.GetBytes(s);
            await ns.WriteAsync(buffer, 0, buffer.Length);
        }

        public void Dispose()
        {
            receiveDateTask.Dispose();
            ns.Close();
            client.Close();
        }
    }
}