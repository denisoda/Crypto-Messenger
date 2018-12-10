using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Server.Abstract
{
    public interface IRSAHelper
    {
        Task<string> EncryptAsync(string text);
        Task<string> DecryptAsync(string cipherText);
    }
}