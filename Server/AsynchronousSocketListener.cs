using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Server.Abstract;
using Server.Model;
using Server.Settings;

namespace Server
{
    public class AsynchronousSocketListener : IAsyncSocketListener
    {
        public static ManualResetEvent AllDone = new ManualResetEvent(false);
        private static TcpListener _listener;
        private static ILogger<AsynchronousSocketListener> _logger;
        private static readonly Dictionary<int, TcpClient> Connections = new Dictionary<int, TcpClient>();
        private static int _connectionCount;
        private static readonly object Lock = new object();

        public AsynchronousSocketListener(ILogger<AsynchronousSocketListener> logger)
        {
            _logger = logger;
        }

        public static async Task StartListening()
        {
            _listener = new TcpListener(ServerSettings.IpEndPoint);
            try
            {
                await Task.Run(async () =>
                {
                    _listener.Start();
                    Console.WriteLine($"Server started on {_listener.Server.LocalEndPoint}");
                    _logger.LogInformation($"{nameof(AsynchronousSocketListener)} has bound");

                    while (true)
                    {
                        var client = await _listener.AcceptTcpClientAsync();
                        lock (Lock) Connections.Add(_connectionCount, client);

                        Console.WriteLine($"{client.Client.LocalEndPoint} connected");
                        _logger.LogInformation($"{client.Client.LocalEndPoint} connected");

                        Task t = new Task(() => handle_clients(_connectionCount));
                        t.Start();
                        Interlocked.Increment(ref _connectionCount);
                    }
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }
        }

        public static void handle_clients(object o)
        {
            var id = (int)o;
            TcpClient client;

            lock (Lock) client = Connections[id];

            while (true)
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                var byteCount = stream.Read(buffer, 0, buffer.Length);

                if (byteCount == 0)
                {
                    break;
                }

                string data = Encoding.ASCII.GetString(buffer, 0, byteCount);
                broadcast(data);
                Console.WriteLine(data);
            }

            lock (Lock) Connections.Remove(id);
            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        public static void broadcast(string data)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(data + Environment.NewLine);

            lock (Lock)
            {
                foreach (TcpClient c in Connections.Values)
                {
                    NetworkStream stream = c.GetStream();

                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        Task IAsyncSocketListener.StartListening()
        {
            return StartListening();
        }
    }
}