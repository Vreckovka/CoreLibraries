using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Logger;

namespace VCore.WPF.Logger
{
  public class CollectionLogger : BaseLogger, ILoggerContainer
  {
    public CollectionLogger()
    {

    }
    public override  IList<Log> Logs { get; } = new ObservableCollection<Log>();

    #region Methods

    public override Task Log(MessageType messageType, string message)
    {
      VSynchronizationContext.PostOnUIThread(() => base.Log(messageType, message));

      return Task.CompletedTask;
    }

    #endregion
  }
}
