using VCore.Standard.Modularity.Interfaces;

namespace VCore.Standard.Factories.Views
{
  public interface IBaseFactory
  {
    TView Create<TView>();
  }

  public interface IViewFactory<out TView> 
    where TView : IView
  {
    TView Create();
  }

  public interface IViewFactory<out TView, in TViewModel> : IViewFactory<TView>
    where TViewModel : IViewModel
    where TView : IView
  {
    TView Create(TViewModel viewModel);
  }
}