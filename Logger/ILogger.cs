using System;
using System.Runtime.CompilerServices;

namespace Logger
{
    public interface ILogger
    {
      void Log(
        MessageType type,
        string message,
        bool logToFile = false,
        [CallerFilePath] string callerFilePath = null,
        [CallerMemberName] string methodName = "");

      void Log(Exception ex, bool logToFile = false);
    }
}
