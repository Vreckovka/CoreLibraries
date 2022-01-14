using System;
using System.Threading.Tasks;

namespace Logger
{
  public interface ILoggerContainer : IDisposable
  {
    Task Log(MessageType messageType, string message);
  }
}