using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCore.ViewModels.Navigation;

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
