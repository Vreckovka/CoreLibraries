using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using VCore.Helpers;
using VCore.Standard;
using VCore.Standard.Factories.ViewModels;
using VCore.WPF.Managers;

namespace VCore.WPF.ViewModels.WindowsFiles
{
  public class WindowsFolderViewModel : FolderViewModel<WindowsFileViewModel>

  {
    private DirectoryInfo directoryInfo;

    public WindowsFolderViewModel(FolderInfo folderInfo, IViewModelsFactory viewModelsFactory) : base(folderInfo, viewModelsFactory)
    {
      directoryInfo = new DirectoryInfo(folderInfo.Indentificator);
    }

  

    #region GetFiles

    public override Task<IEnumerable<FileInfo>> GetFiles()
    {
      return Task.Run(() =>
      {
        try
        {
          var wFileInfos = directoryInfo.GetFiles();

          return wFileInfos.Select(x => new FileInfo(x.FullName, x.FullName)
          {
            Indentificator = x.FullName,
            Name = x.Name,
            Length = x.Length
          });
        }
        catch (UnauthorizedAccessException ex)
        {
          return null;
        }
      });
    }

    #endregion

    #region GetFolders

    public override Task<IEnumerable<FolderInfo>> GetFolders()
    {
      return Task.Run(() =>
      {
        try
        {
          var wFileInfos = directoryInfo.GetDirectories();

          return wFileInfos.Select(x => new FolderInfo()
          {
            Indentificator = x.FullName,
            Name = x.Name
          });
        }
        catch (UnauthorizedAccessException ex)
        {
          return null;
        }
      });
    }

    #endregion

  }
}