using System;
using System.Windows;
using VCore.Standard.Modularity.Interfaces;
using VCore.WPF.Interfaces.Managers;
using VCore.WPF.ViewModels.Windows;

namespace VCore.WPF.Managers
{
  public class SplashScreenManager : ISplashScreenManager
  {
    private readonly IWindowManager windowManager;
    private Window actualSplashScreen;

    public SplashScreenManager(IWindowManager windowManager)
    {
      this.windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
    }

    public void ShowSplashScreen<TView>() where TView : IView, new()
    {
      if (actualSplashScreen != null)
      {
        actualSplashScreen.Close();
      }

      actualSplashScreen = windowManager.GetWindowView<TView>(new SplashScreenViewModel(), false);

      actualSplashScreen.Show();
    }

    public void PushText(string text)
    {
      throw new NotImplementedException();
    }

    public void CloseActualSplashScreen()
    {
      actualSplashScreen.Close();
    }
  }
}