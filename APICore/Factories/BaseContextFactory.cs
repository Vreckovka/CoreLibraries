using System;
using Autofac;

namespace APICore.Factories
{
  public class BaseContextFactory<TContext> : IContextFactory<TContext>
  {
    private readonly ILifetimeScope container;
    private readonly string connectionString;

    public BaseContextFactory(ILifetimeScope container, string connectionString)
    {
      this.container = container ?? throw new ArgumentNullException(nameof(container));
      this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }


    public TContext CreateContext()
    {
      return container.Resolve<TContext>(new NamedParameter("connectionString", connectionString));
    }
  }
}