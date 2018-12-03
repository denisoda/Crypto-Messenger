using System;
using System.Threading.Tasks;

namespace Client
{
    public interface IAsyncClient: IDisposable
    {
        Task StartClient();
        Task Send(string message);
    }
}