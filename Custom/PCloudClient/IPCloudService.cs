using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PCloudClient.Domain;
using PCloudClient.JsonResponses;
using VCore.Standard;
using FileInfo = PCloudClient.Domain.FileInfo;

namespace PCloudClient
{
  public interface IPCloudService
  {
    Task<FolderInfo> GetFolderInfo(long id);
    Task<IEnumerable<FileInfo>> GetFilesAsync(long folderId);
    Task<IEnumerable<FolderInfo>> GetFoldersAsync(long folderId);
    void SaveLoginInfo(string email, string password);
    Task<bool> ExistsFolderAsync(long id);
    Task<MemoryStream> ReadFile(long id);
    Task<PublicLink> GetFileLink(long id);

    Task<PublicLink> GetAudioLink(long id);
    Task<PCloudResponse<Stats>> GetFileStats(long id);
    Task<FolderInfo> CreateFolder(string name, long? parentId);
    Task<long?> CreateFile(string name, long id);
    Task<bool> WriteToFile(byte[] data, long id);
    Task<bool> CreateFileAndWrite(string name, byte[] data, long folderId);
    bool IsUserLoggedIn();


    AsyncProcess<List<KeyValuePair<long, PublicLink>>> GetAudioLinks(IEnumerable<long> ids, CancellationToken cancellationToken);
    AsyncProcess<List<KeyValuePair<long, PublicLink>>> GetFileLinks(IEnumerable<long> ids, CancellationToken cancellationToken = default);


    Task<bool> CreateUploadLink(long folderId, string comment);
    Task<bool> Uploadtolink(string code, string fileName, byte[] data);

    Task UploadToLinkHttp(string code, string fileName, byte[] data);

    Task<PublicFolderLinkContentScrappedItem> ScrapePublicFolderItems(string folderLink);

  }
}
