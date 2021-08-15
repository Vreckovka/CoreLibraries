using System;
using System.Diagnostics;
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


    #region OpenContainingFolder

    private ActionCommand openContainingFolder;

    public ICommand OpenContainingFolder
    {
      get
      {
        if (openContainingFolder == null)
        {
          openContainingFolder = new ActionCommand(OnOpenContainingFolder);
        }

        return openContainingFolder;
      }
    }


    private void OnOpenContainingFolder()
    {
      if (!string.IsNullOrEmpty(Model.FullName))
      {
        var folder = Model.FullName;

        if (!Directory.Exists(Model.FullName))
        {
          folder = System.IO.Path.GetDirectoryName(Model.FullName);
        }

        if (!string.IsNullOrEmpty(folder))
        {
          Process.Start(new System.Diagnostics.ProcessStartInfo()
          {
            FileName = folder,
            UseShellExecute = true,
            Verb = "open"
          });
        }
      }
    }


    #endregion

  }
}