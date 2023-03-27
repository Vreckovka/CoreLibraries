using VCore.Standard;

namespace VCore.WPF
{
  public class LoadingStatus : ViewModel
  {
    #region IsLoading

    private bool isLoading;

    public bool IsLoading
    {
      get { return isLoading; }
      set
      {
        if (value != isLoading)
        {
          isLoading = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region ShowProcessCount

    private bool showProcessCount = false;

    public bool ShowProcessCount
    {
      get { return showProcessCount; }
      set
      {
        if (value != showProcessCount)
        {
          showProcessCount = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region NumberOfProcesses

    private int numberOfProcesses;

    public int NumberOfProcesses
    {
      get { return numberOfProcesses; }
      set
      {
        if (value != numberOfProcesses)
        {
          numberOfProcesses = value;
          RaisePropertyChanged();
          RaisePropertyChanged(nameof(Progress));
        }
      }
    }

    #endregion

    #region ProcessedCount

    private int processedCount;

    public int ProcessedCount
    {
      get { return processedCount; }
      set
      {
        if (value != processedCount)
        {
          processedCount = value;
          RaisePropertyChanged();
          RaisePropertyChanged(nameof(Progress));
        }
      }
    }

    #endregion

    #region Progress

    public double Progress
    {
      get
      {
        if (NumberOfProcesses != 0)
          return ProcessedCount * 100.0 / NumberOfProcesses;

        return 0;
      }

    }

    #endregion

    #region Message

    private string message = "Loading...";

    public string Message
    {
      get { return message; }
      set
      {
        if (value != message)
        {
          message = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

  }
}