using System.IO;

namespace VCore.Standard.ViewModels.WindowsFile
{
  public class FileViewModel : WindowsItem<FileInfo>
  {
    public FileViewModel(
      FileInfo fileInfo) : base(fileInfo)
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