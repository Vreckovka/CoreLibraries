using System;
using System.Windows;
using VCore.Standard.Factories.ViewModels;
using VCore.WPF.ViewModels.Navigation;

namespace VCore.WPF.ViewModels
{
  public class BaseMainWindowViewModel : BaseWindowViewModel
  {
    public BaseMainWindowViewModel(IViewModelsFactory viewModelsFactory)
    {
      ViewModelsFactory = viewModelsFactory ?? throw new ArgumentNullException(nameof(viewModelsFactory));


      if (Application.Current.MainWindow != null)
        Application.Current.MainWindow.Closed += MainWindow_Closed;
    }

    public IViewModelsFactory ViewModelsFactory { get; set; }
    public NavigationViewModel NavigationViewModel { get; set; } = new NavigationViewModel();
  

    private void MainWindow_Closed(object sender, System.EventArgs e)
    {
      Application.Current.Shutdown();
    }
  }
}
