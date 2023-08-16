using System;
using Ninject;

namespace VCore.Standard.Factories.Views
{
  public class BaseBaseFactory : IBaseFactory
  {
    private readonly IKernel kernel;

    public BaseBaseFactory(IKernel kernel)
    {
      this.kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
    }

    public TView Create<TView>()
    {
      return kernel.Get<TView>();
    }
  }
}
