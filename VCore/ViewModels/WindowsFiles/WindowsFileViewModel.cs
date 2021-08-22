using System.Diagnostics;
using System.IO;

namespace VCore.WPF.ViewModels.WindowsFiles
{
  public class WindowsFileViewModel : FileViewModel
  {
    public WindowsFileViewModel(FileInfo fileInfo) : base(fileInfo)
    {
    }

    #region OnOpenContainingFolder

    public override void OnOpenContainingFolder()
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