using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Logger
{
  public class Log
  {
    public MessageType MessageType { get; set; }
    public string Message{ get; set; }
    public DateTime Time { get; set; }
  }

  public class BaseLogger : ILoggerContainer
  {
    public virtual IList<Log> Logs { get; } = new List<Log>();

    #region Log

    public virtual Task Log(MessageType messageType, string message)
    {
      Logs.Add(new Log()
      {
        Message = message,
        MessageType = messageType,
        Time = DateTime.Now
      });

      return Task.CompletedTask;
    }

    #endregion

    public void Dispose()
    {
    }
  }
}