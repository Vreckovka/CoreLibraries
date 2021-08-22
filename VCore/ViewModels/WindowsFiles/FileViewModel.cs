using System.IO;
using VCore.Standard.ViewModels.TreeView;
using VCore.Standard.ViewModels.WindowsFile;
using VCore.WPF.Managers;

namespace VCore.WPF.ViewModels.WindowsFiles
{
  public abstract class FileViewModel : FolderHierarchyItemViewModel<FileInfo>
  {
    public FileViewModel(FileInfo fileInfo) : base(fileInfo)
    {
      FileType = fileInfo.Extension.GetFileType();
    }

    #region FileType

    private FileType fileType;

    public FileType FileType
    {
      get { return fileType; }
      set
      {
        if (value != fileType)
        {
          fileType = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion
   
  }
}