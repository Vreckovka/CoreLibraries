using System;
using System.Security.Cryptography;
using System.Text;

namespace DomainCore.Encryption
{
  public class EncryptionProvider : IEncryptionProvider
  {
    private string key = "9dda1169-2b42-4ac1-8ff7-68d4528b3c7e";
    public string Encrypt(string text)
    {
      byte[] data = UTF8Encoding.UTF8.GetBytes(text);

      using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
      {
        byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
        using (TripleDESCryptoServiceProvider triDes = new TripleDESCryptoServiceProvider()
        {
          Key = keys,
          Mode = CipherMode.ECB,
          Padding = PaddingMode.PKCS7
        })
        {
          ICryptoTransform transform = triDes.CreateEncryptor();
          byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
          var encryptedText = Convert.ToBase64String(results, 0, results.Length);

          return encryptedText;
        }
      }
    }

    public string Decrypt(string text)
    {
      byte[] data = Convert.FromBase64String(text);

      using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
      {
        byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
        using (TripleDESCryptoServiceProvider triDes = new TripleDESCryptoServiceProvider()
        {
          Key = keys,
          Mode = CipherMode.ECB,
          Padding = PaddingMode.PKCS7
        })
        {
          ICryptoTransform transform = triDes.CreateDecryptor();
          byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
          var deencryptedText = UTF8Encoding.UTF8.GetString(results);

          return deencryptedText;
        }
      }
    }
  }
}
