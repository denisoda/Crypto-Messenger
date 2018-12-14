using System;
using System.Text;
using Server.Abstract;
using Server.Settings;

namespace Server.Infrastructure
{
    public class KeyExchangeChecker: IKeyExchangeChecker
    {
        public  bool IsKeyExchange(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return false;
            }         
            byte[] copyArray = new byte[ServerSettings.KeyExchangePhrase.Length];
            
            Array.Copy(data, 0, copyArray, 0, data.Length);
            
            var exchangePhrase = Encoding.Default.GetString(data);
            
            return string.Equals(exchangePhrase, ServerSettings.KeyExchangePhrase);
        }
    }
}