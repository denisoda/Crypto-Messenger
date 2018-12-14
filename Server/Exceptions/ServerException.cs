using System;

namespace Server.Exceptions
{
    public class ServerException : Exception
    {
        public ServerException(string message) : base(message)
        {
        }
    }
}