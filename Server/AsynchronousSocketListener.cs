using System;
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
    public class AsynchronousSocketListener: IAsyncSocketListener
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private Socket _listener;
        private static ILogger<AsynchronousSocketListener> _logger;

        public AsynchronousSocketListener(ILogger<AsynchronousSocketListener> logger)
        {
            _logger = logger;
        }

        public Task StartListening()
        {
            _listener = new Socket(ServerSettings.AddressFamily,
                ServerSettings.SocketType, ServerSettings.ProtocolType);

            try
            {
                _listener.Bind(ServerSettings.IpEndPoint);
                _logger.LogInformation($"{nameof(AsynchronousSocketListener)} has bind");
                _listener.Listen(ServerSettings.MaxConnectionsNumber);

                while (true)
                {
                    allDone.Reset();

                    _logger.LogInformation($"{nameof(AsynchronousSocketListener)} is waiting for connections");

                    _listener.BeginAccept(AcceptCallback,
                        _listener);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }
        }

        public static void AcceptCallback(IAsyncResult ar)
        { 
            allDone.Set();
            var listener = (Socket)ar.AsyncState;
            var handler = listener.EndAccept(ar);

            StateObject state = new StateObject();
            state.WorkSocket = handler;
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        { 
            var state = (StateObject)ar.AsyncState;
            var handler = state.WorkSocket;

            var bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                state.Sb.Append(Encoding.Default.GetString(
                    state.Buffer, 0, bytesRead));

                var content = state.Sb.ToString();
                if (content.IndexOf("<EOF>", StringComparison.Ordinal) > -1)
                {
                    _logger.LogInformation("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);

                    Send(handler, content);
                }
                else
                {
                    handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                        ReadCallback, state);
                }
            }
        }

        private static void Send(Socket handler, String data)
        {
            var byteData = Encoding.Default.GetBytes(data);

            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                var handler = (Socket)ar.AsyncState;

                var bytesSent = handler.EndSend(ar);
                _logger.LogInformation("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }
    }
}