using System;
using System.Threading.Tasks;

namespace Logger
{
  public class ConsoleLogger : ILoggerContainer
  {
    #region Methods

    public Task Log(MessageType messageType, string message)
    {
      lock (Console.Out)
      {
        //… makes beep sound
        return Task.Run(() =>
      {
        try
        {
          Console.ForegroundColor = ConsoleColor.White;

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

            default:
              throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
          }

          var fixedMessage = message.Replace("…", "");

          Console.WriteLine(fixedMessage);
        }
        catch (Exception ex)
        {
        }
        finally
        {
          Console.ForegroundColor = ConsoleColor.White;
        }
      });
      }
    }

    #endregion Methods
  }
}