using System;
using System.Collections.ObjectModel;
using System.Linq;
using VCore.Standard;
using VCore.WPF.Helpers;

namespace VCore.WPF.ViewModels.Navigation
{
  public class BaseNavigationItem<TModel> : BaseNavigationItem where TModel : class
  {
    public BaseNavigationItem(TModel model)
    {
      Model = model ?? throw new ArgumentNullException(nameof(model));
    }

    #region Model

    private TModel model;

    public TModel Model
    {
      get { return model; }
      set
      {
        if (value != model)
        {
          model = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion
  }

  public class BaseNavigationItem : ViewModel, INavigationItem
  {
    #region IsActive

    private bool isActive;
    public virtual bool IsActive
    {
      get { return isActive; }
      set
      {
        if (value != isActive)
        {
          isActive = value;

          OnActivationChanged();
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region Header

    private string hader;

    public string Header
    {
      get { return hader; }
      set
      {
        if (value != hader)
        {
          hader = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    protected virtual void OnActivationChanged()
    {

    }
  }

  public class NavigationItem : BaseNavigationItem
  {
    public NavigationItem(INavigationItem navigationItem)
    {
      Model = navigationItem ?? throw new ArgumentNullException(nameof(navigationItem));

      Model.ObservePropertyChange(x => x.IsActive).Subscribe(x => IsActive = x);

      SubItems.CollectionChanged += SubItems_CollectionChanged;
    }


    public INavigationItem Model { get; }
    public new string Header => Model.Header;
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
    public override bool IsActive
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

    #region Methods

    #region SubItems_CollectionChanged

    private void SubItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems != null)
      {
        foreach (INavigationItem newItem in e.NewItems)
        {
          newItem.ObservePropertyChange(x => x.IsActive).Subscribe(x => IsActive = x);
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

    #endregion
  }
}