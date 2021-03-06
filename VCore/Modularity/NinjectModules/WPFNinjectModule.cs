using VCore.Modularity.Navigation;
using VCore.Modularity.RegionProviders;
using VCore.Standard.Modularity.NinjectModules;
using VCore.WPF.Managers;

namespace VCore.Modularity.NinjectModules
{
  public class WPFNinjectModule : BaseNinjectModule
  {
    public override void RegisterProviders()
    {
      Kernel.Bind<IRegionProvider>().To<RegionProvider>().InSingletonScope();
      Kernel.Bind<INavigationProvider>().To<NavigationProvider>().InSingletonScope();
      Kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
    }
  }
}
