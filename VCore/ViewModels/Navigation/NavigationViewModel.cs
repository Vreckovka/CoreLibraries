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
      Items.ItemUpdated.Where(x => x.EventArgs.PropertyName == nameof(NavigationItem.IsActive) && ((NavigationItem)x.Sender).IsActive).ObserveOnDispatcher().Subscribe(x =>
      {
        Actual = (NavigationItem)x.Sender;
      });
    }

    public RxObservableCollection<NavigationItem> Items { get; set; } = new RxObservableCollection<NavigationItem>();

    #region Actual

    private NavigationItem actual;

    public NavigationItem Actual
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
