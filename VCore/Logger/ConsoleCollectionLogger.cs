using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Logger;

namespace VCore.WPF.Logger
{
  public class ConsoleCollectionLogger : ConsoleLogger
  {
    public ConsoleCollectionLogger()
    {

    }
    public ObservableCollection<Log> ObservableLogs { get; } = new ObservableCollection<Log>();

    #region Methods

    public override async Task Log(MessageType messageType, string message)
    {
      await base.Log(messageType, message);

      VSynchronizationContext.PostOnUIThread(() => ObservableLogs.Add(new Log()
      {
        Message = message,
        MessageType = messageType,
        Time = DateTime.Now
      }));
    }

    #endregion
  }
}