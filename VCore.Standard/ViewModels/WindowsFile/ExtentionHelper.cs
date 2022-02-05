namespace VCore.Standard.ViewModels.WindowsFile
{
  public static class ExtentionHelper
  {
    public static FileType GetFileType(this string extention)
    {
      switch (extention)
      {
        case ".mp4":
        case ".mkv":
        case ".avi":
        case ".ts":
        case ".wmv":
          {
            return FileType.Video;
          }
        case ".flac":
        case ".wav":
        case ".mp3":
        case ".ogg":
        case ".m4a":
          {
            return FileType.Sound;
          }
        case ".jpg":
        case ".gif":
        case ".png":
          {
            return FileType.Image;
          }
        case ".txt":
          {
            return FileType.TextFile;
          }
        case ".srt":
        case ".lrc":
          {
            return FileType.Subtitles;
          }
        case ".zip":
        case ".rar":
          {
            return FileType.CompressFile;
          }
        default:
          {
            return FileType.Other;
          }
      }
    }

  }
}