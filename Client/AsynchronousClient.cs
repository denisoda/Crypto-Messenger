using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RSATest;
using Server.Abstract;
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
        private static readonly RSACryptoServiceProvider rsa;
        private static byte[] clientPublicKey;
        private static byte[] clientPrivateKey;
        private static byte[] serverPrivateKey;

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
                throw e;
            }
        }

        private async void ReceiveData(TcpClient client)
        {
            NetworkStream networkStream = client.GetStream();
            byte[] receivedBytes = new byte[1024];
            var sb = new StringBuilder();
            int byte_count;

            while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                var message = Encoding.ASCII.GetString(receivedBytes, 0, byte_count);

                Console.Write($"Response from server encrypted: {message}");

                message = await rsaHelper.DecryptAsync(message);

                Console.Write($"Response from server decrypted: {message} \n");
            }
        }

        public async Task Send(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new Exception("Empty string");
            }
            var s = await rsaHelper.EncryptAsync(message);

            var buffer = Encoding.ASCII.GetBytes(s);
            await ns.WriteAsync(buffer, 0, buffer.Length);
        }
        
        
        public async Task KeyExchange()
        {
            ns = client.GetSекуфь()ж
            var buffer = clientPublicKey;
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