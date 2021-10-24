using VCore.Standard.Modularity.Interfaces;
using VCore.Standard.ViewModels.WindowsFile;

namespace VCore.WPF.ViewModels.WindowsFiles
{
  public class FileInfo : IUpdateable<FileInfo>, IFolderHierarchyItem
  {
    public string Extension { get; private set; }
    public string Indentificator { get; set; }
    public string Name { get; set; }
    public string FullName { get; set; }
    public string Source { get; set; }
    public long Length { get; set; }

    public string Artist { get; set; }
    public string Album { get; set; }
    public string Title { get; set; }

    public FileInfo()
    {
      
    }

    public FileInfo(string fullName, string source)
    {
      FullName = fullName;
      Source = source;
      Extension = System.IO.Path.GetExtension(fullName.ToLower());
    }

    public void Update(FileInfo other)
    {
      Extension = other.Extension;
      Indentificator = other.Indentificator;
      Name = other.Name;
      FullName = other.FullName;
      Source = other.Source;
      Length = other.Length;
      Artist = other.Artist;
      Album = other.Album;
      Title = other.Title;
    }
  }
}