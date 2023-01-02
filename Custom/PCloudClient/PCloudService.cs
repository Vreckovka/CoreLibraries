using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Logger;
using PCloudClient.Api;
using PCloudClient.Domain;
using PCloudClient.JsonResponses;
using VCore;
using VCore.Standard;
using FileInfo = PCloudClient.Domain.FileInfo;

namespace PCloudClient
{
  public class PCloudService : IPCloudService
  {
    #region Fields

    private readonly string filePath;
    private readonly ILogger logger;
    private bool ssl;
    private string host;

    private LoginInfo credentials;

    #endregion

    #region Constructors

    public PCloudService(string filePath, ILogger logger)
    {
      this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
      host = "eapi.pcloud.com";
      ssl = true;
    }

    #endregion

    #region Initilize

    public void Initilize()
    {
      credentials = GetLoginInfo();
    }

    #endregion

    #region GetLoginInfo

    public LoginInfo GetLoginInfo()
    {
      if (File.Exists(filePath))
      {
        var json = File.ReadAllText(filePath);

        return JsonSerializer.Deserialize<LoginInfo>(json);
      }

      return null;
    }

    #endregion

    #region ExecuteAction

    private async Task<TResult> ExecuteAction<TResult>(Func<Connection, TResult> action, bool permisionless = false)
    {
      using (var context = new PCouldContext(ssl, host, logger))
      {
        if (!permisionless)
        {
          await context.LoginAsync(credentials);
        }
        else
        {
          await context.OpenConnection();
        }

        var conn = context.Connection;

        if (context.IsLoggedIn || permisionless)
        {
          try
          {

            var result = action.Invoke(conn);

            if (result is Task task)
            {
              await task;
            }

            return result;
          }
          catch (Exception ex)
          {
            logger.Log(ex);
          }
        }
      }

      return default(TResult);
    }

    #endregion

    #region GetAudioLink

    public async Task<PublicLink> GetAudioLink(long id)
    {
      var task = await ExecuteAction(async (conn) =>
      {
        return await conn.GetAudioLink(id);
      });

      return task == null ? null : await task;
    }

    #endregion

    #region GetFileStats

    public async Task<PCloudResponse<Stats>> GetFileStats(long id)
    {
      var task = await ExecuteAction(async (conn) =>
      {
        return await conn.GetFileStats(id);
      });

      return task == null ? null : await task;
    }

    #endregion

    #region GetFileLink

    public async Task<PublicLink> GetFileLink(long id)
    {
      var task = await ExecuteAction(async (conn) =>
      {
        return await conn.GetFileLink(id);
      });

      return task == null ? null : await task;
    }

    #endregion

    #region GetPublicLinks

    public AsyncProcess<List<KeyValuePair<long, PublicLink>>> GetPublicLinks(IEnumerable<long> ids, string actionName, CancellationToken cancellationToken = default)
    {
      var process = new AsyncProcess<List<KeyValuePair<long, PublicLink>>>();
      var idsList = ids.ToList();
      process.InternalProcessesCount = idsList.Count;

      process.Process = Task.Run(async () =>
      {
        var task = await ExecuteAction(async (conn) =>
        {
          var links = new List<KeyValuePair<long, PublicLink>>();

          try
          {
            foreach (var id in idsList)
            {
              cancellationToken.ThrowIfCancellationRequested();
              var link = await conn.GetPublicLink(actionName, id);

              process.ProcessedCount++;

              links.Add(new KeyValuePair<long, PublicLink>(id, link));
            }
          }
          catch (OperationCanceledException ex)
          {
          }

          return links;
        });

        return task == null ? null : await task;
      });

      return process;
    }

    #endregion

    #region GetAudioLinks

    public AsyncProcess<List<KeyValuePair<long, PublicLink>>> GetAudioLinks(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
      return GetPublicLinks(ids, "getaudiolink");
    }

    #endregion

    #region GetFileinks

    public AsyncProcess<List<KeyValuePair<long, PublicLink>>> GetFileLinks(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
      return GetPublicLinks(ids, "getfilelink");
    }

    #endregion

    #region IsUserLoggedIn

    public bool IsUserLoggedIn()
    {
      return File.Exists(filePath);
    }

    #endregion

    #region GetFilesAsync

    public async Task<IEnumerable<FileInfo>> GetFilesAsync(long folderId)
    {
      var task = await ExecuteAction(async (conn) =>
      {
        return await conn.getFiles(folderId);
      });

      return task == null ? null : await task;
    }

    #endregion

    #region GetFoldersAsync

    public async Task<IEnumerable<FolderInfo>> GetFoldersAsync(long folderId)
    {
      var task = await ExecuteAction(async (conn) =>
      {
        return await conn.getFolders(folderId);
      });

      return task == null ? null : await task;
    }

    #endregion

    #region SaveLoginInfo

    public void SaveLoginInfo(string email, string password)
    {
      var loginInfo = new LoginInfo()
      {
        Email = email,
        Password = password
      };

      var json = JsonSerializer.Serialize(loginInfo);


      StringHelper.EnsureDirectoryExists(filePath);

      File.WriteAllText(filePath, json);

      credentials = loginInfo;
    }

    #endregion

    #region GetFoldersAsync

    public async Task<FolderInfo> GetFolderInfo(long id)
    {
      var task = await ExecuteAction(async (conn) =>
      {
        return await conn.listFolder(id);
      });

      return task == null ? null : await task;
    }

    #endregion

    #region ExistsFolderAsync

    public async Task<bool> ExistsFolderAsync(long id)
    {
      var task = await ExecuteAction(async (conn) =>
      {
        return (await conn.listFolder(id)) != null;
      });

      return task == null ? false : await task;
    }

    #endregion

    #region CreateFile

    public async Task<long?> CreateFile(string name, long parenId)
    {
      var task = await ExecuteAction(async (conn) =>
      {
        var file = await conn.createFile(parenId, name, FileMode.Create, FileAccess.Write);
        long? result = null;

        if (file.fileId > 0)
        {
          result = file.fileId;
        }

        return result;
      });

      return task == null ? null : await task;
    }

    #endregion

    #region WriteToFile

    public async Task<bool> WriteToFile(byte[] data, long id)
    {
      var task = await ExecuteAction(async (conn) =>
      {
        MemoryStream ms = new MemoryStream(data, false);

        var fd = await conn.createFile(id, FileMode.Open, FileAccess.Write);

        await conn.writeFile(fd, ms, ms.Length);
        await conn.closeFile(fd);

        return true;
      });

      return task == null ? false : await task;

    }

    #endregion

    #region CreateFileAndWrite

    public async Task<bool> CreateFileAndWrite(string name, byte[] data, long folderId)
    {
      var task = await ExecuteAction(async (conn) =>
      {
        MemoryStream ms = new MemoryStream(data, false);

        var fd = await conn.createFile(folderId, name, FileMode.Create, FileAccess.Write);

        await conn.writeFile(fd, ms, ms.Length);
        await conn.closeFile(fd);

        return true;
      });

      return task == null ? false : await task;
    }

    #endregion

    #region CreateUploadLink

    public async Task<bool> CreateUploadLink(long folderId, string comment)
    {
      var task = await ExecuteAction(async (conn) =>
      {
        await conn.CreateUploadLink(folderId, comment);

        return true;
      });

      return task == null ? false : await task;
    }

    #endregion

    #region Uploadtolink

    public async Task<bool> Uploadtolink(string code, string fileName, byte[] data, bool permissionless = false)
    {
      var task = await ExecuteAction(async (conn) =>
      {
        var memoryStream = new MemoryStream(data);

        await conn.uploadToLink(fileName, memoryStream, code, Authentication.GetDeviceInfo());

        return true;
      }, permissionless);

      return task == null ? false : await task;
    }

    #endregion

    #region UploadToLinkHttp
    public async Task UploadToLinkHttp(string code, string fileName, byte[] data)
    {
      if (string.IsNullOrWhiteSpace(fileName))
        throw new ArgumentException("File name can't be empty", "fileName");

      var methodName = "uploadtolink";
      var request = $"https://{host}/{methodName}?names=NamesParameter&code={code}&file={fileName}";

      HttpClient client = new HttpClient();
      HttpContent httpContent = new StreamContent(new MemoryStream(data));
      HttpResponseMessage responseMessage = await client.PostAsync(request, httpContent);

      string responseBody = await responseMessage.Content.ReadAsStringAsync();
    }


    #endregion

    #region ScrapePublicFolderItems

    public Task<PublicFolderLinkContentScrappedItem> ScrapePublicFolderItems(string folderLink)
    {
      return Task.Run(() =>
      {
        WebClient client = new WebClient();

        string downloadString = client.DownloadString(folderLink);
        var stringEnd = "      if (obLength(";

        var asd = downloadString.Split("publinkData")[1].Substring(3).Replace("if (obLength(", null).Trim()
          .Replace("};", "}");

        return JsonSerializer.Deserialize<PublicFolderLinkContentScrappedItem>(asd);
      });
    }

    #endregion

    #region CreateFolder

    public async Task<FolderInfo> CreateFolder(string name, long? parentId)
    {
      var task = await ExecuteAction(async (conn) =>
      {
        return await conn.createFolder(name, parentId);
      });

      return task == null ? null : await task;
    }

    #endregion

    #region ReadFile

    public async Task<MemoryStream> ReadFile(long id)
    {
      var task = await ExecuteAction(async (conn) =>
      {
        var fd = await conn.createFile(id, FileMode.Open, FileAccess.Read);

        var fileSize = await conn.getFileSize(fd);

        MemoryStream msRead = new MemoryStream();

        await conn.readFile(fd, msRead, fileSize.length);

        return msRead;
      });

      return task == null ? null : await task;
    }

    #endregion

    #region DeleteFile

    public async Task DeleteFile(long id)
    {
      var task = await ExecuteAction(async (conn) =>
      {
        await conn.deleteFile(id);
      });

      await task;
    }

    #endregion
  }
}
