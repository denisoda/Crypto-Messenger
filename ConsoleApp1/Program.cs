using System;
using System.Security.Cryptography;
using System.Text;

namespace RsaCryptoExample
{
  static class Program
  {
    static void Main()
    {
        var text = "#KEYEXCHANGE#";
        var bytesOfText = Encoding.Default.GetBytes(text);
        
        var csp = new RSACryptoServiceProvider();
  
        var privateKey = csp.ExportParameters(true);
        var publicKey = csp.ExportParameters(false);
  
        csp = new RSACryptoServiceProvider();
        
        csp.ImportParameters(privateKey);

        var encryptedData = csp.Encrypt(bytesOfText, false);

        Console.WriteLine($"Encrypted! \n {Encoding.Default.GetString(encryptedData)}");
        
        
        csp = new RSACryptoServiceProvider();
        
        csp.ImportParameters(privateKey);
        
        var decryptedData = csp.Decrypt(encryptedData, false);

        Console.WriteLine($"Decrypted! \n {Encoding.Default.GetString(decryptedData)}");
    }
  }
}