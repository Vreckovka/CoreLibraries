using VCore.Standard;

namespace VCore.WPF.ViewModels.Navigation
{
  public class MenuViewModel : ViewModel, INavigationItem
  {
    public MenuViewModel(string header)
    {
      Header = header;
    }

    #region IsActive

    private bool isActive;

    public bool IsActive
    {
      get { return isActive; }
      set
      {
        if (value != isActive)
        {
          isActive = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    public string Header { get; } 
  }
}