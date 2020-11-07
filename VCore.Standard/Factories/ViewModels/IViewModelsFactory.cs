namespace VCore.Standard.Factories.ViewModels
{
  public interface IViewModelsFactory
  {
    TModel Create<TModel>(params object[] parameters);
  }
}