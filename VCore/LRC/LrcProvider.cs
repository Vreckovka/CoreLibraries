using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ninject;
using VPlayer.AudioStorage.InfoDownloader.LRC.Domain;

namespace VPlayer.AudioStorage.InfoDownloader.LRC
{
  public enum LRCProviders
  {
    NotIdentified,
    Google,
    Local
  }



  public abstract class LrcProvider<TFileOutput> : ILrcProvider, IInitializable
  {
    #region ParseLRCFile

    public LRCFile ParseLRCFile(string[] lines)
    {
      var parser = new LRCParser();

      var lrcFile = parser.Parse(lines.ToList());

      if (lrcFile == null)
      {
        throw new Exception("Could not parse");
      }

      return lrcFile;
    }

    #endregion

    #region GetFileName

    public string GetFileName(string artistName, string songName)
    {
      return $"{artistName} - {songName}";
    }

    #endregion

    #region TryGetLrcAsync

    public async Task<ILRCFile> TryGetLrcAsync(string songName, string artistName, string albumName)
    {
      var output = await GetLinesLrcFileAsync(songName, artistName, albumName);

      ILRCFile ilRCFile = output.Value;
      var lines = output.Key;

      if (lines != null && ilRCFile != null)
      {
        var lRCFile = ParseLRCFile(lines);

        if (lRCFile == null && LrcFileExists(songName, artistName, albumName))
        {
          throw new Exception("FAILED TO PARSE " + songName + " " + artistName + " " + albumName);
        }

        ilRCFile.Update(lRCFile);
      }

      return ilRCFile;
    }

    public abstract void Update(ILRCFile lRCFile);

    #endregion

    #region LrcFileExists

    protected bool LrcFileExists(string songName, string artistName, string albumName)
    {
      var fileName = GetFile(songName, artistName, albumName);

      if (fileName != null)
      {
        return true;
      }

      return false;
    }

    #endregion

    protected abstract Task<KeyValuePair<string[], ILRCFile>> GetLinesLrcFileAsync(string songName, string artistName, string albumName);
    protected abstract TFileOutput GetFile(string songName, string artistName, string albumName);

    public abstract LRCProviders LRCProvider { get; }

    public void Initialize()
    {
    }
  }
}