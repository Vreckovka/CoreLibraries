using System;
using System.Collections.ObjectModel;
using System.Linq;
using VCore.Helpers;
using VCore.Standard;

namespace VCore.ViewModels.Navigation
{
  public class NavigationItem : ViewModel, INavigationItem
  {
    public NavigationItem(INavigationItem navigationItem)
    {
      Model = navigationItem ?? throw new ArgumentNullException(nameof(navigationItem));

      Model.ObservePropertyChange(x => x.IsActive).Subscribe(x => IsActive = x);

      SubItems.CollectionChanged += SubItems_CollectionChanged;
    }

    private void SubItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      if(e.NewItems != null)
      {
        foreach (INavigationItem newItem in e.NewItems)
        {
          newItem.ObservePropertyChange(x => x.IsActive).Subscribe(x => IsActive = x);
        }
      }
    }

    public INavigationItem Model { get; }
    public string Header => Model.Header;
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
          Model.IsActive = value;

          if (SubItems.All(x => x.IsActive != value) && SubItems.Count > 0)
          {
            SubItems[0].IsActive = value;
          }

          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region Dispose

    public override void Dispose()
    {
      base.Dispose();

      Model?.Dispose();
    }

    #endregion
  }
}