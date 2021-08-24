using System;
using System.Threading;
using System.Windows;
using VCore.Standard.Modularity.Interfaces;
using VCore.WPF.Interfaces.Managers;
using VCore.WPF.ViewModels.Windows;
using VCore.WPF.Views;

namespace VCore.WPF.Managers
{
  public static class SplashScreenManager 
  {
    private static SplashScreenWindow splashScreen;
    private static SplashScreenViewModel viewModel = new SplashScreenViewModel();
    private static Thread splashScreenThread;

    #region ShowSplashScreen


    public static void ShowSplashScreen<TView>() where TView : IView, new()
    {
      viewModel = new SplashScreenViewModel();

      splashScreenThread = new Thread(() =>
      {
        splashScreen = new SplashScreenWindow();
        splashScreen.DataContext = viewModel;
        splashScreen.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        splashScreen.WindowStyle = WindowStyle.None;
        splashScreen.AllowsTransparency = true;
        splashScreen.ResizeMode = ResizeMode.NoResize;
        splashScreen.ShowInTaskbar = true;
        splashScreen.SizeToContent = SizeToContent.WidthAndHeight;

        splashScreen.Content = new TView();

        if (splashScreen != null)
        {
          splashScreen.Show();

          EventHandler closedEventHandler = null;

          closedEventHandler = (o, s) =>
          {
            splashScreen.Closed -= closedEventHandler;
            splashScreen.Dispatcher.InvokeShutdown();
          };

          splashScreen.Closed += closedEventHandler;

          System.Windows.Threading.Dispatcher.Run();
        }
      });

      splashScreenThread.SetApartmentState(ApartmentState.STA);
      splashScreenThread.Start();
    }

    #endregion

    public static void SetText(string text)
    {
      viewModel.Message = text;
    }

    public static void AddProgress(double progress)
    {
      viewModel.Progress += progress;
    }

    #region CloseActualSplashScreen

    public static void CloseActualSplashScreen()
    {
      splashScreen.Dispatcher.Invoke(() =>
      {
        splashScreen.Close();
      });
    }

    #endregion
  }
}