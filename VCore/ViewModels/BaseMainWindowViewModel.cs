using VCore.Standard;

namespace VCore.ViewModels
{
  public class BaseMainWindowViewModel : ViewModel
  {
    public BaseMainWindowViewModel()
    {
    }

    public virtual string Title { get; set; } = "BASE WPF APP";
  }
}
