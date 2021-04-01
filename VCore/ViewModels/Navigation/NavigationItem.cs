using System;
using System.Collections.ObjectModel;
using VCore.Helpers;
using VCore.Standard;

namespace VCore.ViewModels.Navigation
{
  public class NavigationItem : ViewModel, INavigationItem
  {
    public readonly INavigationItem navigationItem;

    public NavigationItem(INavigationItem navigationItem)
    {
      this.navigationItem = navigationItem ?? throw new ArgumentNullException(nameof(navigationItem));

      navigationItem.ObservePropertyChange(x => x.IsActive).Subscribe(x => IsActive = x);
    }

    public string Header => navigationItem.Header;
    public ObservableCollection<INavigationItem> SubItems { get; set; } = new ObservableCollection<INavigationItem>();
    public string IconPathData { get; set; }
    public string ImagePath { get; set; }

    #region IsBackroundActive

    private bool isBackroundActive;

    public bool IsBackroundActive
    {
      get { return isBackroundActive; }
      set
      {
        if (value != isBackroundActive)
        {
          isBackroundActive = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

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
          navigationItem.IsActive = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region Dispose

    public override void Dispose()
    {
      base.Dispose();

      navigationItem?.Dispose();
    }

    #endregion
  }
}