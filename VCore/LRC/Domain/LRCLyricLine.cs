using System;

namespace VCore.WPF.LRC.Domain
{
  public class LRCLyricLine
  {
    public TimeSpan? Timestamp { get; set; }
    public string Text { get; set; }
    public string OriginalLine { get; set; }

    public override string ToString()
    {
      return $"[{Timestamp}]{Text}";
    }
  }
}