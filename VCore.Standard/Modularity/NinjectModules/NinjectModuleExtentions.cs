using Ninject;
using Ninject.Syntax;

namespace VCore.Standard.Modularity.NinjectModules
{
  public static class NinjectModuleExtentions
  {
    #region BindToSelfInSingletonScope

    public static IBindingOnSyntax<TInitializable> BindToSelfInSingletonScope<TInitializable>(this IKernel kernel)
    {
        return kernel.Bind<TInitializable>().ToSelf().InSingletonScope();
    }

    #endregion

    #region InitializeOnActivation

    public static IBindingOnSyntax<TInitializable> InitializeOnActivation<TInitializable>(this IBindingOnSyntax<TInitializable> kernelBinding)
      where TInitializable : IInitializable
    {
      return kernelBinding.OnActivation(x => x.Initialize());
    }

    #endregion

    #region BindToSelf

    public static IBindingWhenInNamedWithOrOnSyntax<TInitializable> BindToSelf<TInitializable>(this IKernel kernel)
      where TInitializable : IInitializable
    {
      return kernel.Bind<TInitializable>().ToSelf();
    }

    #endregion


  }
}
