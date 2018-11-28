using System.Threading.Tasks;

namespace Client
{
    public interface IAsyncClient
    {
        Task StartClient();
    }
}