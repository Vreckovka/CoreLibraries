using System;
using System.Runtime.CompilerServices;

namespace Logger
{
    public interface ILogger : IDisposable
    {
      public ILoggerContainer LoggerContainer { get;  }
      void Log(
        MessageType type,
        string message,
        bool logToFile = false,
        bool logErrorToFile = true,
        bool simpleMessage = false,
        [CallerFilePath] string callerFilePath = null,
        [CallerMemberName] string methodName = "");

      void Log(Exception ex, bool logToFile = true, bool logErrorToFile = true);
    }
}
