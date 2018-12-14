using System;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Server;
using Server.Abstract;
using Server.Infrastructure;

namespace ServerBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()                
                .AddSingleton<IServer, Server.Server>()
                .AddTransient<IAsyncSocketListener, AsynchronousSocketListener>()
                .AddTransient<IKeyExchangeChecker, KeyExchangeChecker>()
                .BuildServiceProvider();

            var server = serviceProvider.GetService<IServer>();
            var socketListener = serviceProvider.GetService<IAsyncSocketListener>();

            Server.Server.Instance.AsyncSocketListener = socketListener;
            Server.Server.Instance.Run();

            Console.ReadLine();
        }
    }
}
