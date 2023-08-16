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

    public async void Log(
      MessageType type,
      string message,
      bool logToFile = false,
      bool logErrorToFile = true,
      [CallerFilePath] string callerFilePath = null,
      [CallerMemberName] string methodName = "")
    {
      if (isDisposed)
        return;

      try
      {
        var className = Path.GetFileNameWithoutExtension(callerFilePath);

        var log = $"[{type}|{DateTime.Now.ToString("hh:mm:ss")}]\t{className}.{methodName}()\t{message}";

        if (type == MessageType.Success && !LogSuccess)
          return;

        await loggerContainer.Log(type, log);

        Logs.Add(log);

        if (logToFile || (type == MessageType.Error && logErrorToFile))
        {
          await fileLoggerContainer.Log(type, message);
        }
      }
      catch (Exception ex)
      {
        await loggerContainer.Log(MessageType.Error, ex.Message);

        Logs.Add(ex.Message);
      }
    }

    public void Log(Exception ex, bool logToFile = true, bool logErrorToFile = true)
    {
#if DEBUG
      logToFile = false;
      logErrorToFile = false;
#endif

      Log(MessageType.Error, ex.ToString(), logToFile, logErrorToFile);
    }

    #endregion

    private bool isDisposed;

    public void Dispose()
    {
      loggerContainer.Dispose();
      fileLoggerContainer.Dispose();

      isDisposed = true;
    }
  }

  public class FakeLogger : ILogger
  {
    public void Dispose()
    {
    }

    public void Log(
      MessageType type,
      string message,
      bool logToFile = false,
      bool logErrorToFile = true,
      string callerFilePath = null,
      string methodName = "")
    {
    }

    public void Log(Exception ex, bool logToFile = true, bool logErrorToFile = true)
    {
    }
  }
}

