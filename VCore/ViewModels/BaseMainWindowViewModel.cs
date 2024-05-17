using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using VCore.Standard.Factories.ViewModels;
using VCore.Standard.Providers;
using VCore.WPF.ViewModels.Navigation;
using Application = System.Windows.Application;

namespace VCore.WPF.ViewModels
{
  public class BaseMainWindowViewModel : BaseWindowViewModel
  {
    public BaseMainWindowViewModel(IViewModelsFactory viewModelsFactory)
    {
      ViewModelsFactory = viewModelsFactory ?? throw new ArgumentNullException(nameof(viewModelsFactory));

      if (Application.Current.MainWindow != null)
        Application.Current.MainWindow.Closed += MainWindow_Closed;

      Window = Application.Current.MainWindow;

      var assembly = System.Reflection.Assembly.GetEntryAssembly();

      BuildVersion = BasicInformationProvider.GetFormattedBuildVersion(assembly);
    }

    public IViewModelsFactory ViewModelsFactory { get; set; }
    public NavigationViewModel NavigationViewModel { get; set; } = new NavigationViewModel();

    public string BuildVersion { get; }


    private Visibility originalVisibility = Visibility.Visible;

    protected override void OnWindowStateChanged(WindowState windowState)
    {
      base.OnWindowStateChanged(windowState);

      if (windowState != WindowState.Minimized && 
          !ShowInTaskBar && 
          Window.Visibility == Visibility.Collapsed)
      {
        Window.Visibility = originalVisibility;
      }
      else if(windowState == WindowState.Minimized && !ShowInTaskBar && Window.Visibility != Visibility.Collapsed)
      {
        originalVisibility = Window.Visibility;

        Window.Visibility = Visibility.Collapsed;
      }
    }


    private void MainWindow_Closed(object sender, System.EventArgs e)
    {
      Application.Current.Shutdown();
    }

    #region SetupTray

    protected void SetupTray(string title, string iconPath)
    {
      NotifyIcon ni = new NotifyIcon();
      ni.Icon = new System.Drawing.Icon(iconPath);
      ni.Text = title;
      ni.ContextMenuStrip = new ContextMenuStrip();

      ni.ContextMenuStrip.Items.Add("Open");
      ni.ContextMenuStrip.Items[0].Click += (x, y) =>
      {
        Window.Show();
        Window.Visibility = Visibility.Visible;
        Window.WindowState = WindowState.Normal;
        WindowState = WindowState.Normal;
        Window.Topmost = true;
        Window.Topmost = false;
      };


      ni.ContextMenuStrip.Items.Add("Close");
      ni.ContextMenuStrip.Items[1].Click += (x, y) =>
      {
        Window.Close();
      };



      ni.Visible = true;
      ni.DoubleClick += (x, y) =>
      {
        Window.Show();
        this.WindowState = WindowState.Normal;
        Window.Visibility = Visibility.Visible;

        Window.Topmost = true;
        Window.Topmost = false;
      };
    }

    #endregion
  }
}
