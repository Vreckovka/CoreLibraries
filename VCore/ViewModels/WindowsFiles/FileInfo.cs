using VCore.Standard.ViewModels.WindowsFile;

namespace VCore.WPF.ViewModels.WindowsFiles
{
  public class FileInfo : IFolderHierarchyItem
  {
    public string Extension { get; }
    public string Indentificator { get; set; }
    public string Name { get; set; }
    public string FullName { get; set; }
    public string Source { get; set; }
    public long Length { get; set; }

    public FileInfo(string fullName, string source)
    {
      FullName = fullName;
      Source = source;
      Extension = System.IO.Path.GetExtension(fullName);
    }
  }
}