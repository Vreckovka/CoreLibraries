﻿
using Ninject.Extensions.Factory;
using VCore.Standard.Factories.ViewModels;
using VCore.Standard.Factories.Views;
using VCore.Standard.Providers;

namespace VCore.Standard.Modularity.NinjectModules
{
  public class CommonNinjectModule : BaseNinjectModule
  {
    #region Methods

    public override void RegisterFactories()
    {
      Kernel.Bind<IViewModelsFactory>().To<BaseViewModelsFactory>();
      Kernel.Bind<IBaseFactory>().To<BaseBaseFactory>(); 
    }


    public override void RegisterProviders()
    {
      base.RegisterProviders();

      Kernel.Bind<ISettingsProvider>().To<SettingsProvider>().InSingletonScope().WithConstructorArgument("settingsPath","settings\\settings.txt"); 
    }

    #endregion Methods
  }
}