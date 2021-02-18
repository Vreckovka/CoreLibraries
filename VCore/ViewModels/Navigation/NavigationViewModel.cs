using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using VCore.Annotations;
using VCore.Helpers;
using VCore.Standard;

namespace VCore.ViewModels.Navigation
{
  public class NavigationViewModel
  {
    public ObservableCollection<NavigationItem> Items { get; set; } = new ObservableCollection<NavigationItem>();


  }

  public class NavigationItem : ViewModel, INavigationItem
  {
    public readonly INavigationItem navigationItem;

    public NavigationItem([NotNull] INavigationItem navigationItem)
    {
      this.navigationItem = navigationItem ?? throw new ArgumentNullException(nameof(navigationItem));

      //navigationItem.ObservePropertyChange(x => x.IsActive).Subscribe(x => IsActive = x);
    }


    public string Header => navigationItem.Header;


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


    public override void Dispose()
    {
      base.Dispose();

      navigationItem?.Dispose();
    }



    public ObservableCollection<INavigationItem> SubItems { get; set; } = new ObservableCollection<INavigationItem>();


  }
}
