using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Server.Abstract;
using Server.Exceptions;
using Server.Infrastructure;
using Server.Model;
using Server.Settings;

namespace Server
{
    public class AsynchronousSocketListener : IAsyncSocketListener
    {
        private static TcpListener listener;
        private static ILogger<AsynchronousSocketListener> logger;
        private IKeyExchangeChecker keyExchangeChecker;
        private static readonly Dictionary<int, TcpClient> Connections = new Dictionary<int, TcpClient>();
        private static int connectionCount;
        private static readonly object @lock = new object();
        private static readonly RSACryptoServiceProvider rsa;
        private static RSAParameters serverPrivateKey;
        private RSAParameters clientPublicKey;

        static AsynchronousSocketListener()
        {
            rsa = new RSACryptoServiceProvider();
            serverPrivateKey = rsa.ExportParameters(true);
        }

        public AsynchronousSocketListener(ILogger<AsynchronousSocketListener> logger, IKeyExchangeChecker _keyExchangeChecker)
        {
            AsynchronousSocketListener.logger = logger;
            keyExchangeChecker = _keyExchangeChecker;
        }

        public async Task StartListening()
        {
            listener = new TcpListener(ServerSettings.IpEndPoint);
            try
            {
                await Task.Run(async () =>
                {
                    listener.Start();
                    Console.WriteLine($"Server started on {listener.Server.LocalEndPoint}");
                    logger.LogInformation($"{nameof(AsynchronousSocketListener)} has bound");

                    while (true)
                    {
                        var client = await listener.AcceptTcpClientAsync();
                        lock (@lock) Connections.Add(connectionCount, client);

                        Console.WriteLine($"{client.Client.LocalEndPoint} connected");
                        logger.LogInformation($"{client.Client.LocalEndPoint} connected");

                        Task t = new Task(() => handle_clients(connectionCount));
                        t.Start();
                        Interlocked.Increment(ref connectionCount);
                    }
                });
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
                throw;
            }
        }

        private void handle_clients(int id)
        {
            TcpClient client;
            lock (@lock) client = Connections[id - 1];
            
            NetworkStream stream = client.GetStream(); 
            
            byte[] buffer = new byte[client.ReceiveBufferSize];
            if (stream.Read(buffer, 0, client.ReceiveBufferSize) == 0)
            {
                logger.LogError($"key exchange with {client.Client.LocalEndPoint} failed");
                throw new ServerException("Key exchange failed.");
            };
            clientPublicKey.Modulus = buffer;
            
            rsa.ImportParameters(clientPublicKey);
            
            buffer = rsa.Encrypt(serverPrivateKey.Modulus, false);
            
            stream.Write(buffer, 0, buffer.Length);
            
            logger.LogInformation($"key exchange with {client.Client.LocalEndPoint} succeed");
            
            while (true)
            {
                stream = client.GetStream();
                buffer = new byte[client.ReceiveBufferSize];
                var byteCount = stream.Read(buffer, 0, client.ReceiveBufferSize);
                if (byteCount == 0)
                {
                    break;
                }
                buffer = rsa.Decrypt(buffer, false);              
                string data = Encoding.ASCII.GetString(buffer, 0, byteCount);
                Console.WriteLine($"message: '{data}' from {client.Client.RemoteEndPoint}");
                Send(client, buffer);
            }

            lock (@lock) Connections.Remove(id);
            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();
            Console.WriteLine($"{client.Client.RemoteEndPoint} disconnected");
        }

        public async void Send(TcpClient client, byte[] data)
        {
            var buffer = Encoding.ASCII.GetBytes(data + Environment.NewLine);
            buffer = rsa.Encrypt(buffer, false);
            var stream = client.GetStream();

            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
        
        public static void Broadcast(string data)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(data + Environment.NewLine);

            lock (@lock)
            {
                foreach (TcpClient c in Connections.Values)
                {
                    NetworkStream stream = c.GetStream();

                    stream.Write(buffer, 0, buffer.Length);

                    Console.WriteLine($"Message was sent to {c.Client.RemoteEndPoint} via broadcasting");
                    logger.LogInformation($"Message was sent to {c.Client.RemoteEndPoint} via broadcasting");
                }
            }
        }

        Task IAsyncSocketListener.StartListening()
        {
            return StartListening();
        }
    }
}
