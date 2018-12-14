 using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Client;
using Microsoft.Extensions.DependencyInjection;
using RSATest;
using Server.Abstract;

namespace ClientRunner
{
    class Program
    {
        public static async Task Main(String[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IAsyncClient>()
                .BuildServiceProvider();

            var client = serviceProvider.GetService<IAsyncClient>();

            try
            {
                using (client)
                {
                    await client.StartClient();
                    while (true)
                    {
                        var message = Console.ReadLine();
                        await client.Send(message);
                    }

                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Could not connected to the server, please, try again");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            Console.WriteLine("Disconnected from the server");
        }
    }
}
