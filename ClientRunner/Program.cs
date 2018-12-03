using System;
using System.Threading.Tasks;
using Client;

namespace ClientRunner
{
    class Program
    {
        public static async Task Main(String[] args)
        {
            IAsyncClient server = new AsynchronousClient();

            using (server)
            {
                await server.StartClient();
                while (true)
                {
                    var message = Console.ReadLine();
                    await server.Send(message);
                }
            }

            Console.WriteLine("Disconnected from the server");
        }
    }
}
