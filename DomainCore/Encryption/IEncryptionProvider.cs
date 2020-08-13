namespace DomainCore.Encryption
{
  public interface IEncryptionProvider
  {
    string Encrypt(string text);
    string Decrypt(string text);
  }
}