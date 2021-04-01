using System;
using System.IO;
using VCore.Standard.ViewModels.TreeView;

namespace VCore.Standard.ViewModels.WindowsFile
{

  public class WindowsItem<TModel> : TreeViewItemViewModel<TModel> where TModel : FileSystemInfo
  {
   public WindowsItem(TModel model) : base(model)
    {
      Model = model ?? throw new ArgumentNullException(nameof(model));
      Name = Model.Name;

      HighlitedText = Name;
    }

 
    #region Path

    public string Path
    {
      get { return Model.FullName; }

    }

    #endregion
    
  }
}