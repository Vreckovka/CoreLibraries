namespace APICore.Factories
{
  public interface IContextFactory<TContext>
  {
    public TContext CreateContext();
  }
}