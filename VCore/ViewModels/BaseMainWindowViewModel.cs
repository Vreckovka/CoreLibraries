using Ninject;
using VCore.Standard;

namespace VCore.ViewModels
{
  public class BaseMainWindowViewModel : ViewModel, IInitializable
  {
    public BaseMainWindowViewModel()
    {
    }

    public virtual string Title { get; set; } = "BASE WPF APP";
  }
}
