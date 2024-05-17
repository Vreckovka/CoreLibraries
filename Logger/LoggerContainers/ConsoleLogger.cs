using System;
using System.Threading;
using System.Threading.Tasks;

namespace Logger
{
  public class ConsoleLogger : BaseLogger, ILoggerContainer
  {
    public ConsoleLogger()
    {

    }

    #region Methods

    private static object batton = new object();
    private bool isDisposed;
    public override Task Log(MessageType messageType, string message)
    {
      base.Log(messageType, message);

      return Task.Run(() =>
      {
        lock (batton)
        {
          if (isDisposed)
          {
            return;
          }

          try
          {
            switch (messageType)
            {
              case MessageType.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
              case MessageType.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
              case MessageType.Success:
                Console.ForegroundColor = ConsoleColor.Green;
                break;
              case MessageType.Success2:
                Console.ForegroundColor = ConsoleColor.Magenta;
                break;
              case MessageType.Inform:
                Console.ForegroundColor = ConsoleColor.Gray;
                break;
              case MessageType.Inform2:
                Console.ForegroundColor = ConsoleColor.Cyan;
                break;
              default:
                Console.ForegroundColor = ConsoleColor.Gray;
                break;
            }

            //… makes beep sound
            var fixedMessage = message.Replace("…", "");

            //Console.Write("");
            //Console.Write("");
            //Console.Write("");

            Console.WriteLine(fixedMessage);

            //Console.Write("");
            //Console.Write("");
            //Console.Write("");
          }
          finally
          {
            Console.ResetColor();
          }
        }
      });
    }

    #endregion

    public void Dispose()
    {
      lock (batton)
      {
        isDisposed = true;
      }
    }
  }
}