using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
