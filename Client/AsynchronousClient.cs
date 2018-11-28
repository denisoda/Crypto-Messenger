using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server.Settings;

namespace Client
{
    public class AsynchronousClient 
    {
        private const int _port = ClientSettings.Port;

        private static Socket _client;

        private static readonly ManualResetEvent ConnectDone =
            new ManualResetEvent(false);
        private static readonly ManualResetEvent SendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent ReceiveDone =
            new ManualResetEvent(false);

        private static string Response;

        private static async Task StartClient()
        {
            await Task.Run(async () =>
            {
                try
                {
                    _client = new Socket(ClientSettings.AddressFamily,
                        ClientSettings.SocketType, ClientSettings.ProtocolType);

                    _client.BeginConnect(ClientSettings.RemoteEp,
                        ConnectCallback, _client);
                    ConnectDone.WaitOne();
                    
                    Console.WriteLine($"Connected to {ClientSettings.RemoteEp}");

                    await Send("This is a test data<EOF>");
                    SendDone.WaitOne();

                    Receive();
                    ReceiveDone.WaitOne();

                    Console.WriteLine("Response received : {0}", Response);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            });
        }   

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                ConnectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive()
        {
            try
            {
                var state = new StateObjectTransmittingModel();
                state.workSocket = _client;

                // Begin receiving the data from the remote device.  
                _client.BeginReceive(state.buffer, 0, StateObjectTransmittingModel.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObjectTransmittingModel state = (StateObjectTransmittingModel)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObjectTransmittingModel.BufferSize, 0,
                        ReceiveCallback, state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        Response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    ReceiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static async Task Send(String data)
        {
            await Task.Run(() =>
            {
                // Convert the string data to byte data using ASCII encoding.  
                byte[] byteData = Encoding.ASCII.GetBytes(data);

                // Begin sending the data to the remote device.  
                _client.BeginSend(byteData, 0, byteData.Length, 0,
                    SendCallback, _client);
            });
        }

        private static async void SendCallback(IAsyncResult ar)
        {
            await Task.Run(() =>
            {
                try
                {
                    // Retrieve the socket from the state object.  
                    Socket client = (Socket) ar.AsyncState;

                    // Complete sending the data to the remote device.  
                    int bytesSent = client.EndSend(ar);
                    Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                    // Signal that all bytes have been sent.  
                    SendDone.Set();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            });
        }

        public static async Task<int> Main(String[] args)
        {
            await StartClient();
        try {
                await Send("This is a test 2<EOF>");
                SendDone.WaitOne();

                Receive();
                ReceiveDone.WaitOne();
    
                Console.WriteLine("Response received : {0}", Response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

            return 0;
        }
    }
}