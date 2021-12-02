using System;
using System.Threading;
using System.Threading.Tasks;

namespace Logger
{
  public class ConsoleLogger : ILoggerContainer
  {
    #region Methods

    private static object batton = new object();
    public Task Log(MessageType messageType, string message)
    {
      lock (batton)
      {
        return Task.Run(() =>
        {
          try
          {
            Console.Write("");

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

              case MessageType.Inform:
                Console.ForegroundColor = ConsoleColor.Gray;
                break;
            }

            //… makes beep sound
            var fixedMessage = message.Replace("…", "");

            Console.Write("");
            Console.Write("");
            Console.Write("");

            Console.WriteLine(fixedMessage);

            Console.Write("");
            Console.Write("");
            Console.Write("");
          }
          finally
          {
            Console.ResetColor();
          }
        });
      }
    }

    #endregion Methods
  }
}