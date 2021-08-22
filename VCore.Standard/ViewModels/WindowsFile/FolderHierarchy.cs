using System;
using System.IO;
using VCore.Standard.ViewModels.TreeView;

namespace VCore.Standard.ViewModels.WindowsFile
{
  public interface IFolderHierarchyItem
  {
    public string Name { get; set; }
    public string Indentificator { get; set; }
  }

  public class FolderHierarchyItem<TModel> : TreeViewItemViewModel<TModel> where TModel : class, IFolderHierarchyItem
  {
   public FolderHierarchyItem(TModel model) : base(model)
    {
      Model = model ?? throw new ArgumentNullException(nameof(model));
      Name = Model.Name;

      HighlitedText = Name;
    }

 
    #region Path

    public string Path
    {
      get { return Model.Indentificator; }

    }

    #endregion
    
  }
}