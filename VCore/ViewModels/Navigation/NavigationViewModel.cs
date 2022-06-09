using System;
using System.Reactive.Linq;
using VCore.ItemsCollections;
using VCore.Standard;

namespace VCore.WPF.ViewModels.Navigation
{
  public class NavigationViewModel : ViewModel
  {
    public NavigationViewModel()
    {
      Items.ItemUpdated.Where(x => x.EventArgs.PropertyName == nameof(INavigationItem.IsActive) && ((INavigationItem)x.Sender).IsActive).ObserveOnDispatcher().Subscribe(x =>
      {
        Actual = (INavigationItem)x.Sender;
      });
    }

    public RxObservableCollection<INavigationItem> Items { get; set; } = new RxObservableCollection<INavigationItem>();

    #region Actual

    private INavigationItem actual;

    public INavigationItem Actual
    {
      get { return actual; }
      set
      {
        if (value != actual)
        {
          actual = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    
   
  }
}
