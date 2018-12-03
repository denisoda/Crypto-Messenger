namespace Cryptography.Abstract
{
    public interface IPublicKeyProvider<T>
    {
        T GetPublicKey();
    }
}