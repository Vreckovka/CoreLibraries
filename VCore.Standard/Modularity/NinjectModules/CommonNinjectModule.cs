
using Ninject.Extensions.Factory;
using VCore.Standard.Factories.ViewModels;
using VCore.Standard.Factories.Views;

namespace VCore.Standard.Modularity.NinjectModules
{
  public class CommonNinjectModule : BaseNinjectModule
  {
    #region Methods

    public override void RegisterFactories()
    {
      Kernel.Bind<IViewModelsFactory>().ToFactory();
      Kernel.Bind<IViewFactory>().ToFactory(); 

    }

   

    #endregion Methods
  }
}