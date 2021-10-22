using System;
using System.Collections.Generic;
using System.Linq;

namespace VCore.WPF.LRC.Domain
{
  public class GoogleLRCFile : LRCFile
  {
    public string GoogleDriveFileId { get; set; }

    public GoogleLRCFile(List<LRCLyricLine> lines) : base(lines)
    {
    }
  }


  public interface ILRCFile
  {
    string Id { get; set; }
    string Artist { get; set; }
    string Title { get; set; }
    string Album { get; set; }
    string By { get; set; }

    TimeSpan? Length { get; set; }
    List<LRCLyricLine> Lines { get;  }
    string GetString();

    void Update(LRCFile lRCFile);
  }

  public class LRCFile : ILRCFile
  {
    public LRCFile(List<LRCLyricLine> lines)
    {
      Lines = lines;
    }

    public List<LRCLyricLine> Lines { get; private set; }

    public string Id { get; set; }
    public string Artist { get; set; }
    public string Title { get; set; }
    public string Album { get; set; }
    public string By { get; set; }
    public TimeSpan? Length { get; set; }

    public string GetString()
    {
      var toString = $"[id:{Id}]";
      toString += $"\n[ar:{Artist}]";
      toString += $"\n[ti:{Title}]";
      toString += $"\n[al:{Album}]";
      toString += $"\n[by:{By}]";
      toString += $"\n[length:{Length}]";

      toString += "\n" + String.Join("\n", Lines.Select(x => x.ToString()).ToArray());

      return toString;
    }

    public void Update(LRCFile other)
    {
      Lines = other.Lines;
      Id = other.Id;
      Artist = other.Artist;
      Title = other.Title;
      Album = other.Album;
      By = other.By;
      Length = other.Length;
    }
  }
}