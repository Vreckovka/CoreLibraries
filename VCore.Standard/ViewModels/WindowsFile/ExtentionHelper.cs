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
        {
          return FileType.Video;
        }
        case ".flac":
        case ".wav":
        case ".mp3":
        {
          return FileType.Sound;
        }
        case ".jpg":
        {
          return FileType.Image;
        }
        case ".txt":
        {
          return FileType.TextFile;
        }
        case ".srt":
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