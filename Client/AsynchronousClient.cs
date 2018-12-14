﻿using System;
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

        static AsynchronousClient()
        {
            rsa = new RSACryptoServiceProvider();
            clientPublicKey = rsa.ExportParameters(false).Modulus;
            clientPrivateKey = rsa.ExportParameters(true).Modulus;
            rsa.ImportParameters(new RSAParameters(){ Modulus = clientPublicKey });
            rsa.ImportParameters(new RSAParameters(){ Modulus = clientPrivateKey });
        }

        public async Task StartClient()
        {
            try
            {
                await client.ConnectAsync(ipAddress, port);
                Console.WriteLine($"Connected to {ClientSettings.RemoteEp}");
                await KeyExchange();
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
            rsa.ImportParameters(new RSAParameters() { Modulus = serverPrivateKey });

            while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                var message = Encoding.ASCII.GetString(receivedBytes, 0, byte_count);

                Console.Write($"Response from server encrypted: {message}");

                var decryptedBytes = rsa.Decrypt(receivedBytes, false);
                message = Encoding.Default.GetString(decryptedBytes);

                Console.Write($"Response from server decrypted: {message} \n");
            }
        }

        public async Task Send(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new Exception("Empty string");
            }
            var buffer = Encoding.ASCII.GetBytes(message);
            buffer = rsa.Encrypt(buffer, false);

            await ns.WriteAsync(buffer, 0, buffer.Length);
        }


        public async Task KeyExchange()
        {
            ns = client.GetStream();
            await ns.WriteAsync(clientPublicKey, 0, clientPublicKey.Length);
            ns = client.GetStream();
            ns.Read(serverPrivateKey, 0, serverPrivateKey.Length);
            serverPrivateKey = rsa.Decrypt(serverPrivateKey, false);
        }

        public void Dispose()
        {
            receiveDateTask.Dispose();
            ns.Close();
            client.Close();
        }
    }
}