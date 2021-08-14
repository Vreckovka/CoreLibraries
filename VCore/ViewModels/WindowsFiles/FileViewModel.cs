using System.IO;
using VCore.Standard.ViewModels.WindowsFile;
using VCore.WPF.Managers;

namespace VCore.WPF.ViewModels.WindowsFiles
{
  public class FileViewModel : WindowsItemViewModel<FileInfo>
  {
    public FileViewModel(FileInfo fileInfo, IWindowManager windowManager) : base(fileInfo, windowManager)
    {
      var extention = System.IO.Path.GetExtension(fileInfo.FullName);

      FileType = extention.GetFileType();

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