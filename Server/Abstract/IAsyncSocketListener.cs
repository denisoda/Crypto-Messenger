using System.Threading.Tasks;

namespace Server.Abstract
{
    public interface IAsyncSocketListener
    {
        Task StartListening();
    }
}