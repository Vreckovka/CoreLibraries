using System;
using Ninject;

namespace VCore.Standard.Factories.Views
{
  public class BaseViewFactory : IViewFactory
  {
    private readonly IKernel kernel;

    public BaseViewFactory(IKernel kernel)
    {
      this.kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
    }

    public TView Create<TView>()
    {
      return kernel.Get<TView>();
    }
  }
}
