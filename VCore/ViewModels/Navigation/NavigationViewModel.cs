using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using VCore.Annotations;
using VCore.Standard;

namespace VCore.ViewModels.Navigation
{
  public class NavigationViewModel
  {
    public ObservableCollection<NavigationItem> Items { get; set; } = new ObservableCollection<NavigationItem>();


  }

  public class NavigationItem : ViewModel, INavigationItem
  {
    private readonly INavigationItem navigationItem;

    public NavigationItem([NotNull] INavigationItem navigationItem)
    {
      this.navigationItem = navigationItem ?? throw new ArgumentNullException(nameof(navigationItem));

      navigationItem.PropertyChanged += NavigationItem_PropertyChanged;
    }

    private void NavigationItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (sender is INavigationItem navigationItem && e.PropertyName == nameof(IsActive))
      {
        RaisePropertyChanged(nameof(IsActive));
      }
    
    }

    public string Header => navigationItem.Header;


    #region IsActive

    public bool IsActive
    {
      get { return navigationItem.IsActive; }
      set
      {
        if (value != navigationItem.IsActive)
        {
          navigationItem.IsActive = value;

          
          RaisePropertyChanged();
        }
      }
    }

    #endregion




    public ObservableCollection<INavigationItem> SubItems { get; set; } = new ObservableCollection<INavigationItem>();


  }
}
