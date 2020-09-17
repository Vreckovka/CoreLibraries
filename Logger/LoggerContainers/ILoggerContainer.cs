using System.Threading.Tasks;

namespace Logger
{
  public interface ILoggerContainer
  {
    Task Log(MessageType messageType, string message);
  }
}