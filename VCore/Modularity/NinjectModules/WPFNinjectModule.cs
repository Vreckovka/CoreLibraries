using Ninject;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using VCore.Modularity.Navigation;
using VCore.Modularity.RegionProviders;
using VCore.Standard;
using VCore.Standard.Modularity.NinjectModules;
using VCore.ViewModels.Navigation;
using VCore.WPF.Interfaces.Managers;
using VCore.WPF.Managers;
using VCore.WPF.ViewModels.Navigation;

namespace VCore.Modularity.NinjectModules
{
  public class WPFNinjectModule : BaseNinjectModule
  {
    public override void RegisterProviders()
    {
      Kernel.Bind<IRegionProvider>().To<RegionProvider>().InSingletonScope();
      Kernel.Bind<INavigationProvider>().To<NavigationProvider>().InSingletonScope();
      Kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
      Kernel.Bind<ISplashScreenManager>().To<SplashScreenManager>().InSingletonScope();
    }

    public override void RegisterViewModels()
    {
      base.RegisterViewModels();

      Kernel.Bind<NavigationItem>().ToSelf();
    }
  }
}
