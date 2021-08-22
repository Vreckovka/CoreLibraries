using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using VCore.Standard.ViewModels.WindowsFile;
using VCore.WPF.Managers;

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


    public abstract void OnOpenContainingFolder();

    #endregion
  }
}