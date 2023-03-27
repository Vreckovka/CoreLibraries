using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PCloudClient.Domain;
using PCloudClient.Protocol;

namespace PCloudClient.Api
{
  /// <summary>Folder listing RPCs</summary>
  public static class ListFolder
  {
    /// <summary>List directories, and optionally files. This is relatively low-level method, direct wrapper over the RPC.</summary>
    public static async Task<FolderInfo> listFolder(this Connection conn, long id = 0, bool recursive = false, bool noFiles = false)
    {
      RequestBuilder req = conn.newRequest("listfolder");
      req.add("folderid", id);
      req.add("recursive", recursive);
      req.add("nofiles", noFiles);
      req.unixTimestamps();
      var response = await conn.send(req);

      if (response != null)
      {
        var meta = response.dict["metadata"] as IReadOnlyDictionary<string, object>;
        if (null == meta)
          return null;
        return (FolderInfo)FileBase.create(meta);
      }

      return null;
    }

    /// <summary>Get folder by name, the name is case sensitive. Returns null if not exist.</summary>
    public static async Task<FolderInfo[]> getFolders(this Connection conn, long idFolder)
    {
      var folder = await conn.listFolder(idFolder, false, false);

      var result = folder.children.OfType<FolderInfo>().ToArray();
      // Console.WriteLine( "getFiles( {0:x} ) - got {1} files", idFolder, result.Length );
      return result;
    }

    /// <summary>List all files in the folder, the folder is specified with ID</summary>
    public static async Task<FileInfo[]> getFiles(this Connection conn, long idFolder, bool recursive = false)
    {
      var folder = await conn.listFolder(idFolder, recursive, false);
      var result = folder.children.OfType<FileInfo>().ToList();

      if (recursive)
      {
        result.AddRange(GetRecursiveFiles(folder));
      }

      return result.ToArray();
    }

    private static IEnumerable<FileInfo> GetRecursiveFiles(FolderInfo folder)
    {
      var folders = folder.children.OfType<FolderInfo>();
      List<FileInfo> fileInfos = folder.children?.OfType<FileInfo>().ToList();

      foreach (var pFolder in folders)
      {
        fileInfos.AddRange(GetRecursiveFiles(pFolder));
      }

      return fileInfos;
    }

    /// <summary>List all files in the folder</summary>
    public static Task<FileInfo[]> getFiles(this Connection conn, FolderInfo fi)
    {
      return conn.getFiles(fi.id);
    }
  }
}