using VCore.Standard.Modularity.Interfaces;

namespace VCore.WPF.Interfaces.Managers
{
  public interface ISplashScreenManager
  {
    public void ShowSplashScreen<TView>() where TView : IView, new();
    public void PushText(string text);
    public void CloseActualSplashScreen();
  }
}