using System;
using System.IO;
using System.Threading.Tasks;

namespace Logger
{
  public class FileLoggerContainer : ILoggerContainer
  {
    private static object batton = new object();
    private readonly string logFilePath;

    public FileLoggerContainer(string logFilePath)
    {
      this.logFilePath = string.IsNullOrEmpty(logFilePath) ? throw new ArgumentNullException(nameof(logFilePath)) : logFilePath;
    }

    public Task Log(MessageType messageType, string message)
    {
      Console.WriteLine("LOGGING TO FILE");

      return Task.Run(() =>
      {
        lock (batton)
        {
          EnsureDirectoryExists(logFilePath);

          using (StreamWriter w = File.AppendText(logFilePath))
          {
            w.Write("\r\nLog Entry : ");
            w.WriteLine($"{DateTime.Now}");
            w.WriteLine($"{messageType}:");
            w.WriteLine($"\t{message}");
            w.WriteLine("-------------------------------");
          }
        }
      });
    }

    public void EnsureDirectoryExists(string filePath)
    {
      FileInfo fi = new FileInfo(filePath);

      if (!fi.Directory.Exists)
      {
        System.IO.Directory.CreateDirectory(fi.DirectoryName);
      }
    }



  }
}