using System.Threading.Tasks;
using VCore.WPF.LRC.Domain;

namespace VCore.WPF.LRC
{
  public interface ILrcProvider
  {
    LRCProviders LRCProvider { get; }

    string GetFileName(string artistName, string songName);
    LRCFile ParseLRCFile(string[] lines);
    Task<ILRCFile> TryGetLrcAsync(string songName, string artistName, string albumName);
    Task<bool> Update(ILRCFile lRCFile);
  }
}