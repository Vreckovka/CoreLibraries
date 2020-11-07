namespace VCore.Standard.Modularity.Interfaces
{
  public interface IView
  {
    object DataContext { get; set; }
  }

  public interface IView<TViewModel> : IView
    where TViewModel : IViewModel
  {
    TViewModel ViewModel { get; set; }
  }
}
