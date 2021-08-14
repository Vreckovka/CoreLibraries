using System;
using System.IO;
using System.Windows.Input;
using VCore.Standard.ViewModels.WindowsFile;
using VCore.WPF.Managers;

namespace VCore.WPF.ViewModels.WindowsFiles
{
  public class WindowsItemViewModel<TModel> : WindowsItem<TModel>
    where TModel : FileSystemInfo
  {
    private readonly IWindowManager windowManager;

    public WindowsItemViewModel(TModel fileInfo, IWindowManager windowManager) : base(fileInfo)
    {
      this.windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
    }


  

  }
}