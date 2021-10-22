using VCore.Standard.Modularity.NinjectModules;
using VCore.WPF.Interfaces.Managers;
using VCore.WPF.Managers;
using VCore.WPF.Modularity.Navigation;
using VCore.WPF.Modularity.RegionProviders;
using VCore.WPF.ViewModels.Navigation;

namespace VCore.WPF.Modularity.NinjectModules
{
  public class WPFNinjectModule : BaseNinjectModule
  {
    public override void RegisterProviders()
    {
      Kernel.Bind<IRegionProvider>().To<RegionProvider>().InSingletonScope();
      Kernel.Bind<INavigationProvider>().To<NavigationProvider>().InSingletonScope();
      Kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
    }

    public override void RegisterViewModels()
    {
      base.RegisterViewModels();

      Kernel.Bind<NavigationItem>().ToSelf();
    }
  }
}
