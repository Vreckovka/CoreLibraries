using System;
using System.IO;

namespace APICore.Logging
{

  public interface ILogger
  {
    void Log(Exception exception);
  }

  public class Logger : ILogger
  {
    private readonly string logFilePath;

    public Logger(string logFilePath)
    {
      this.logFilePath = logFilePath ?? throw new ArgumentNullException(nameof(logFilePath));
    }

    private object batton = new object();
    public void Log(Exception exception)
    {
      lock (batton)
      {
        EnsureDirectoryExists(logFilePath);

        using (StreamWriter w = File.AppendText(logFilePath))
        {
          w.Write("\r\nLog Entry : ");
          w.WriteLine($"{DateTime.Now}");
          w.WriteLine("  :");
          w.WriteLine($"  :{exception}");
          w.WriteLine("-------------------------------");
        } 
      }
    }

    private static void EnsureDirectoryExists(string filePath)
    {
      FileInfo fi = new FileInfo(filePath);
      if (!fi.Directory.Exists)
      {
        System.IO.Directory.CreateDirectory(fi.DirectoryName);
      }
    }
  }
}
