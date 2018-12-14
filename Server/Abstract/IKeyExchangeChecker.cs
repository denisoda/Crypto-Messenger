namespace Server.Abstract
{
    public interface IKeyExchangeChecker
    {
        bool IsKeyExchange(byte[] data);
    }
}