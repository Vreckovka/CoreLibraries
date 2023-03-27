using System;
using System.Threading;
using System.Threading.Tasks;
using Logger;
using PCloudClient.Api;

namespace PCloudClient
{
  public class PCloudContext : IDisposable
  {
    private readonly ILogger logger;

    public PCloudContext(bool isSsl, string host, ILogger logger)
    {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
      IsSsl = isSsl;
      Host = host;
    }

    public bool IsLoggedIn { get; private set; }
    public bool IsSsl { get; }
    public string Host { get; }
    public Connection Connection { get; private set; }

    #region LoginAsync


    private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
    public async Task<bool> LoginAsync(LoginInfo credentials)
    {
      await semaphoreSlim.WaitAsync();
      try
      {
        if (credentials != null && Connection == null)
        {
          await OpenConnection();

          await Connection.login(credentials.Email, credentials.Password);

          IsLoggedIn = true;
        }
      }
      catch (Exception ex)
      {
        logger.Log(ex);

        Connection = null;
        IsLoggedIn = false;
      }
      finally
      {
        semaphoreSlim.Release();
      }

      return IsLoggedIn;
    }

    #endregion

    public async Task OpenConnection()
    {
      Connection = await Connection.open(IsSsl, Host);
    }

    #region Logout

    public async Task<bool> Logout()
    {
      if (Connection != null)
      {
        if (Connection.isDesynced)
          return false;

        await Connection.logout();
        Connection.Dispose();
      }

      Connection = null;
      IsLoggedIn = false;

      return true;
    }

    #endregion

    public async void Dispose()
    {
      if (IsLoggedIn)
        await Logout();
    }
  }
}