using System.Windows;

namespace VCore.WPF.ViewModels
{
  public class BaseMainWindowViewModel : BaseWindowViewModel
  {
    public BaseMainWindowViewModel()
    {
      Application.Current.MainWindow.Closed += MainWindow_Closed;

    }

   


    private void MainWindow_Closed(object sender, System.EventArgs e)
    {
      Application.Current.Shutdown();
    }
  }
}
