using System.Collections.ObjectModel;
using System.ComponentModel;

namespace VCore.ViewModels.Navigation
{
  public class NavigationViewModel
  {
    public ObservableCollection<NavigationItem> Items { get; set; } = new ObservableCollection<NavigationItem>();


  }
}
