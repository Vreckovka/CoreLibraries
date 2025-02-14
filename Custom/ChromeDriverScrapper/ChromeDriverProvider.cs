using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Logger;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;

namespace ChromeDriverScrapper
{

  public class ChromeDriverProvider : IChromeDriverProvider, IDisposable
  {
    private readonly ILogger logger;
    private bool wasInitilized;
    private string chromeDriverDirectory;
    private string chromeDriverFileName;
    public ChromeDriver ChromeDriver { get; set; }
    private ChromeDriverService chromeDriverService;

    public ChromeDriverProvider(ILogger logger) : this()
    {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ChromeDriverProvider()
    {
      chromeDriverDirectory = Directory.GetCurrentDirectory();
      chromeDriverFileName = "chromedriver.exe";
    }

    #region Initialize

    public bool Initialize(string proxyServer = null, List<string> options = null, string downloadDirectory = null)
    {
      return InitializeWithExceptionReturn(proxyServer, options, downloadDirectory) == null;
    }


    #endregion

    #region InitializeWithExceptionReturn

    public Exception InitializeWithExceptionReturn(string proxyServer = null, List<string> options = null, string downloadDirectory = null)
    {
      if (!File.Exists(Path.Combine(chromeDriverDirectory, chromeDriverFileName)))
      {
        DownloadChromeDriverAsync(new WebClient().DownloadString("https://chromedriver.storage.googleapis.com/LATEST_RELEASE"), chromeDriverDirectory).GetAwaiter().GetResult();
      }

      var result = InitializeChromeDriver(proxyServer, options, downloadDirectory);

      if (result != null)
      {
        var versionData = GetVersionExeception(result);

        if (versionData != null &&
            versionData.Value.Item1 != null
            & versionData.Value.Item2 != null)
        {
          DownloadChromeDriverAsync(versionData.Value.Item2, chromeDriverDirectory).GetAwaiter().GetResult();
        }

        return InitializeChromeDriver(proxyServer, options);
      }

      return result;
    }

    #endregion

    #region InitializeChromeDriver

    private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

    public Exception InitializeChromeDriver(string proxyServer = null, List<string> options = null, string downloadDirectory = null)
    {
      try
      {
        semaphoreSlim.WaitAsync().Wait();

        if (!wasInitilized)
        {
          chromeDriverService?.Dispose();
          chromeDriverService = null;

          var chromeOptions = new ChromeOptions();

          if (options != null)
          {
            chromeOptions.AddArguments(options);
          }
          else
          {
            chromeOptions.AddArguments(new List<string>()
            {
              "--disable-gpu",      
              //Prevents DevToolsActivePort file doesn't exist Error
              "--no-sandbox",
              "--disable-dev-shm-usage",

              "--headless",
              "--start-maximized",
              "--disable-infobars",
              "--disable-extensions",
              "--log-level=3",
              "--disable-cookie-encryption=false",
              "--block-new-web-contents",
              "--enable-precise-memory-info",
              "--ignore-certificate-errors",
              "--window-size=1920,1080",

            });


            chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");
            chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
          }

          if (downloadDirectory == null)
          {
            downloadDirectory = Directory.GetCurrentDirectory();
          }

          chromeOptions.AddUserProfilePreference("download.default_directory", downloadDirectory);



          if (proxyServer != null)
          {
            chromeOptions.AddArgument($"--proxy-server={proxyServer}");
          }

          chromeDriverService = ChromeDriverService.CreateDefaultService(chromeDriverDirectory, chromeDriverFileName);

          chromeDriverService.HideCommandPromptWindow = true;

          ChromeDriver = new ChromeDriver(chromeDriverService, chromeOptions);
          wasInitilized = true;
        }

        return null;
      }
      catch (Exception ex)
      {
        if (chromeDriverService != null)
        {
          chromeDriverService.Dispose();
        }

        if (ChromeDriver != null)
        {
          ChromeDriver.Dispose();
        }


        logger?.Log(ex);

        return ex;
      }
      finally
      {
        semaphoreSlim.Release();
      }
    }

    #endregion

    #region DownloadChromeDriver

    private Task DownloadChromeDriverAsync(string version, string chromeDriverLocation)
    {
      ChromeDriver?.Dispose();
      chromeDriverService?.Dispose();

      return Task.Run(async () =>
      {
        var fileName = "chromedriver_win32.zip";

        using (var wc = new WebClient())
        {
          try
          {
            await semaphoreSlim.WaitAsync();

            var downloadPage = GetChromeDriverDownloadString(version, wc);

            wc.DownloadFile(new Uri(downloadPage), fileName);

            File.Delete("chromedriver.exe");

            ZipFile.ExtractToDirectory(fileName, chromeDriverLocation, true);

            //Extracted zip contains inner folder
            if (!File.Exists(Path.Combine(chromeDriverDirectory, chromeDriverFileName)))
            {
              var filePath = Path.Combine(chromeDriverDirectory, "chromedriver-win32", chromeDriverFileName);

              File.Move(filePath, Path.Combine(chromeDriverDirectory, chromeDriverFileName));
            }

            File.Delete(fileName);
          }
          catch (Exception ex)
          {
            var exc = new Exception("Chrome driver was not initialized properly!", ex);

            throw exc;
          }
          finally
          {
            semaphoreSlim.Release();
          }
        }
      });
    }


    private string GetChromeDriverDownloadString(string desiredVersion, WebClient webClient)
    {
      string result = null;

      var json = webClient.DownloadString($"https://googlechromelabs.github.io/chrome-for-testing/known-good-versions-with-downloads.json");
      var latestStableVersion = System.Text.Json.JsonSerializer.Deserialize<ChromeForTestingJsonResponse>(json);

      var split = desiredVersion.Split(".");
      var onlyFix = $"{split[0]}.{split[1]}.{split[2]}";

      result = latestStableVersion.versions.FirstOrDefault(x => x.version.Contains(onlyFix))?.downloads?.chromedriver?.SingleOrDefault(x => x.platform == "win32")?.url;

      if (string.IsNullOrEmpty(result))
      {
        var majorVersion = desiredVersion.Split(".")[0];
        var latestMajorVersion = webClient.DownloadString($"https://chromedriver.storage.googleapis.com/LATEST_RELEASE_{majorVersion}");


        result = $"https://chromedriver.storage.googleapis.com/{latestMajorVersion}/chromedriver_win32.zip";
      }

      return result;
    }

    #endregion

    #region GetVersionExeception

    public (string, string)? GetVersionExeception(Exception ex)
    {
      var message = ex.Message;
      if (message.Contains("This version of ChromeDriver only supports Chrome version"))
      {
        var actualVersionRegex = new Regex(@"supports Chrome version.?(\d+)");
        var requiredVersionRegex = new Regex(@"Current browser version is ((\d+|\.)+)");

        var actualVersionMatch = actualVersionRegex.Match(message);
        var requiredVersionMatch = requiredVersionRegex.Match(message);

        var actualVersionString = actualVersionMatch?.Groups[1].Value;
        var requiredVersionString = requiredVersionMatch?.Groups[1].Value;

        return (actualVersionString, requiredVersionString);
      }

      return null;
    }

    #endregion

    #region SafeNavigate

    private WebDriverWait wait;
    private object lockWait = new object();

    public string SafeNavigate(string url, out string redirectedUrl, double secondsToWait = 10, int extraMiliseconds = 0, bool useProxy = false)
    {
      lock (lockWait)
      {
        if (!Initialize())
        {
          redirectedUrl = null;
          return null;
        }

        if (useProxy)
        {
          return UseProxySite(url, out redirectedUrl);
        }

        return SafeNavigate((x) => { }, url, out redirectedUrl, secondsToWait, extraMiliseconds);
      }
    }

    public string Navigate(string url)
    {
      lock (lockWait)
      {
        if (!Initialize())
        {
          return null;
        }

        var navigation = ChromeDriver.Navigate();
        navigation.GoToUrl(url);

        return ChromeDriver.PageSource;
      }
    }

    private string SafeNavigate(Action<string> action, string url, out string redirectedUrl, double secondsToWait = 10, int extraMiliseconds = 0)
    {
      wait = new WebDriverWait(ChromeDriver, TimeSpan.FromSeconds(secondsToWait));
     
      if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var validUrl))
      {
        var result = wait.Until((x) =>
        {
          try
          {
            var navigation = x.Navigate();
            navigation.GoToUrl(validUrl);

            Thread.Sleep(extraMiliseconds);

            action.Invoke(x.PageSource);

            return x;
          }
          catch (WebDriverException wb)
          {
            if (wb.Message.Contains("disconnected: Unable to receive message from renderer")
            || wb.Message.Contains("No connection could be made because the target machine actively refused it"))
            {
              wasInitilized = false;
            }

            return null;
          }
          catch (Exception ex)
          {
            return null;
          }
        });

        redirectedUrl = result.Url;
        return result.PageSource;
      }

      redirectedUrl = null;
      return null;
    }

    #endregion

    #region ExecuteScript

    public object ExecuteScript(string script, double secondsToWait = 10)
    {
      WebDriverWait wait = new WebDriverWait(ChromeDriver, TimeSpan.FromSeconds(secondsToWait));
      var resultGuid = Guid.Empty;

      var waitResult = wait.Until(x =>
      {
        try
        {
          var result = ChromeDriver.ExecuteScript(script);

          if (result == null)
          {
            resultGuid = Guid.NewGuid();
            return resultGuid;
          }

          return result;
        }
        catch (Exception ex)
        {
          return null;
        }
      });


      return waitResult;
    }

    #endregion

    #region ExecuteScriptVoid

    public void ExecuteScriptVoid(string script, double secondsToWait)
    {
      WebDriverWait wait = new WebDriverWait(ChromeDriver, TimeSpan.FromSeconds(secondsToWait));
      ChromeDriver.ExecuteScript(script);

      wait.Until(x => x);
    }

    #endregion

    public string UseProxySite(string url, out string redirectedUrl)
    {
      var proxySite = "https://www.proxysite.com/";
      string result = null;

      SafeNavigate((x) =>
      {
        ExecuteScript($"document.getElementsByClassName('url-form')[0].getElementsByTagName('input')[0].value = '{url}'");
        ExecuteScript("document.getElementsByClassName('url-form')[0]?.getElementsByTagName('button')[0].click()");

        result = ChromeDriver.PageSource;

      }, proxySite, out redirectedUrl);

      return result;
    }



    #region Dispose

    public void Dispose()
    {
      ChromeDriver?.Dispose();
      chromeDriverService?.Dispose();
      wasInitilized = false;
      ChromeDriver = null;
    }

    #endregion


  }

  public class ChromeForTestingJsonResponse
  {
    public DateTime timestamp { get; set; }
    public List<Version> versions { get; set; }

    public class Chrome
    {
      public string platform { get; set; }
      public string url { get; set; }
    }

    public class Chromedriver
    {
      public string platform { get; set; }
      public string url { get; set; }
    }

    public class Downloads
    {
      public List<Chrome> chrome { get; set; }
      public List<Chromedriver> chromedriver { get; set; }
    }





    public class Version
    {
      public string version { get; set; }
      public string revision { get; set; }
      public Downloads downloads { get; set; }
    }
  }
}
