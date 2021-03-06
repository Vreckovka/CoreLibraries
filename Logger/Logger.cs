using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Logger
{
  public class Logger : ILogger
  {
    private readonly ILoggerContainer loggerContainer;
    private readonly FileLoggerContainer fileLoggerContainer;

    #region Constructors

    public Logger(ILoggerContainer loggerContainer, FileLoggerContainer fileLoggerContainer)
    {
      this.loggerContainer = loggerContainer ?? throw new ArgumentNullException(nameof(loggerContainer));
      this.fileLoggerContainer = fileLoggerContainer ?? throw new ArgumentNullException(nameof(fileLoggerContainer));

      AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
    }
    
    #endregion 

    #region Properties

    public List<string> Logs { get; } = new List<string>();

    public bool LogSuccess = true;

    #endregion Properties

    #region AppDomain_UnhandledException

    private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      if (e.ExceptionObject is Exception ex)
      {
        Log(ex, true);
      }
    }

    #endregion

    #region Log

    public void Log(
      MessageType type,
      string message, 
      bool logToFile = false,
      [CallerFilePath]string callerFilePath = null, 
      [CallerMemberName]string methodName = "")
    {
      try
      {
        var className = Path.GetFileNameWithoutExtension(callerFilePath);

        var log = $"[{type}|{DateTime.Now.ToString("hh:mm:ss")}]\t{className}.{methodName}()\t{message}";

        if (type == MessageType.Success && !LogSuccess)
          return;

        loggerContainer.Log(type, log);

        Logs.Add(log);

        if (logToFile)
        {
          fileLoggerContainer.Log(type, message);
        }
      }
      catch (Exception ex)
      {
        loggerContainer.Log(MessageType.Error, ex.Message);

        Logs.Add(ex.Message);
      }
    }

    public void Log(Exception ex, bool logToFile = false)
    {
      Log(MessageType.Error, ex.ToString(), logToFile);
    }

    #endregion

  }
}