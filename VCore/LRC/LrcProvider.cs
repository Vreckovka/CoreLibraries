using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ninject;
using VCore.WPF.LRC.Domain;

namespace VCore.WPF.LRC
{
  public enum LRCProviders
  {
    NotIdentified,
    Google,
    Local,
    MiniLyrics,
    PCloud
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

    #region GetPathValidName

    protected string GetPathValidName(string name)
    {
      if (!string.IsNullOrEmpty(name))
      {
        string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

        foreach (char c in invalid)
        {
          name = name.Replace(c.ToString(), "-");
        }

        return name.Trim();
      }

      return null;
    }

    #endregion

    #region GetFileName

    public string GetFileName(string artistName, string songName)
    {
      if (!string.IsNullOrEmpty(artistName) && !string.IsNullOrEmpty(songName))
        return $"{GetPathValidName(artistName)} - {GetPathValidName(songName)}";

      return null;
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


    #endregion

    public abstract Task<bool> Update(ILRCFile lRCFile);

    #region LrcFileExists

    protected bool LrcFileExists(string songName, string artistName, string albumName)
    {
      var fileName = GetFile(songName, artistName, albumName, ".lrc");

      if (fileName != null)
      {
        return true;
      }

      return false;
    }

    #endregion

    protected abstract Task<KeyValuePair<string[], ILRCFile>> GetLinesLrcFileAsync(string songName, string artistName, string albumName);
    protected abstract Task<TFileOutput> GetFile(string songName, string artistName, string albumName, string extesion);

    public abstract LRCProviders LRCProvider { get; }

    public void Initialize()
    {
    }
  }
}