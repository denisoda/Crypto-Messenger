using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Server.Abstract;

namespace Server
{
    public sealed class Server : IServer 
    {
        public static Server Instance { get; } = new Server();


        static Server()
        {
        }

        public Task Run()
        {
            throw new NotImplementedException();
        }
    }
}
