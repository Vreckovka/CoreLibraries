using VCore.Standard.ViewModels.WindowsFile;

namespace VCore.WPF.ViewModels.WindowsFiles
{
  public class FolderInfo : IFolderHierarchyItem
  {
    public string Name { get; set; }
    public string Indentificator { get; set; }
  }
}