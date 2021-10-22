using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using VCore.Standard.ViewModels.WindowsFile;
using VCore.WPF.Managers;
using VCore.WPF.Misc;

namespace VCore.WPF.ViewModels.WindowsFiles
{
  public abstract class FolderHierarchyItemViewModel<TModel> : FolderHierarchyItem<TModel> where TModel : class, IFolderHierarchyItem
  {
    public FolderHierarchyItemViewModel(TModel fileInfo) : base(fileInfo)
    {
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


    public virtual void OnOpenContainingFolder()
    {
      if (!string.IsNullOrEmpty(Model.Indentificator))
      {
        var folder = Model.Indentificator;

        if (!Directory.Exists(Model.Indentificator))
        {
          folder = System.IO.Path.GetDirectoryName(Model.Indentificator);
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