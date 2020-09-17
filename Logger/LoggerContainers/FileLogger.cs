using System;
using System.IO;
using System.Threading.Tasks;

namespace Logger
{
  public class FileLogger : ILoggerContainer
  {
    private readonly string logFilePath;

    public FileLogger(string logFilePath)
    {
      this.logFilePath = logFilePath ?? throw new ArgumentNullException(nameof(logFilePath));
    }

    public Task Log(MessageType messageType, string message)
    {
      return Task.Run(() =>
      {
        lock (this)
        {
          EnsureDirectoryExists(logFilePath);

          using (StreamWriter w = File.AppendText(logFilePath))
          {
            w.Write("\r\nLog Entry : ");
            w.WriteLine($"{DateTime.Now}");
            w.WriteLine($"{messageType}  :");
            w.WriteLine($"  :{message}");
            w.WriteLine("-------------------------------");
          }
        }
      });
    }

    private void EnsureDirectoryExists(string filePath)
    {
      FileInfo fi = new FileInfo(filePath);

      if (!fi.Directory.Exists)
      {
        System.IO.Directory.CreateDirectory(fi.DirectoryName);
      }
    }

  }
}