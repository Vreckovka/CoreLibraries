using VCore.WPF.LRC.Domain;

namespace VCore.WPF.LRC
{
  public interface ILrcLyricsProvider
  {
    string GetFileName(string artistName, string songName);
    LRCFile ParseLRCFile(string lrcFilePath);
    LRCFile TryGetLrc(string songName, string artistName, string albumName);

  }
}