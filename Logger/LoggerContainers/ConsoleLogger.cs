﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Logger
{
  public class ConsoleLogger : ILoggerContainer
  {
    public ConsoleLogger()
    {

    }

    #region Methods

    private static object batton = new object();
    private bool isDisposed;
    public Task Log(MessageType messageType, string message)
    {
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

              case MessageType.Inform:
                Console.ForegroundColor = ConsoleColor.Gray;
                break;

              case MessageType.Inform2:
                Console.ForegroundColor = ConsoleColor.Cyan;
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