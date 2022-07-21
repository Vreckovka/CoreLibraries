using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Logger;
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

    public ChromeDriverProvider(ILogger logger)
    {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

      chromeDriverDirectory = Directory.GetCurrentDirectory();
      chromeDriverFileName = "chromedriver.exe";
    }

    #region Initialize

    public bool Initialize(string proxyServer = null, List<string> options = null)
    {
      return InitializeWithExceptionReturn(proxyServer, options) == null;
    }


    #endregion

    #region InitializeWithExceptionReturn

    public Exception InitializeWithExceptionReturn(string proxyServer = null, List<string> options = null)
    {
      if (!File.Exists(Path.Combine(chromeDriverDirectory, chromeDriverFileName)))
      {
        DownloadChromeDriverAsync(new WebClient().DownloadString("https://chromedriver.storage.googleapis.com/LATEST_RELEASE"), chromeDriverDirectory).GetAwaiter().GetResult();
      }

      var result = InitializeChromeDriver(proxyServer, options);

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

    public Exception InitializeChromeDriver(string proxyServer = null, List<string> options = null)
    {
      ChromeDriverService chromeDriverService = null;

      try
      {
        if (!wasInitilized)
        {
          var chromeOptions = new ChromeOptions();

          if (options != null)
          {
            chromeOptions.AddArguments(options);
          }
          else
          {
            chromeOptions.AddArguments(new List<string>() {
              "--headless",
              "--disable-gpu",
              "--no-sandbox",
              "--start-maximized",
              "--disable-infobars",
              "--disable-extensions",
              "--log-level=3",
              "--disable-cookie-encryption=false",
              "--block-new-web-contents",
              "--enable-precise-memory-info",
              "--ignore-certificate-errors",
            });
          }


          //chromeOptions.AddArgument("--user-agent=Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.50 Safari/537.36");

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


        logger.Log(ex);

        return ex;
      }
    }

    #endregion

    #region DownloadChromeDriver

    private Task DownloadChromeDriverAsync(string version, string chromeDriverLocation)
    {
      var downloadPage = $"https://chromedriver.storage.googleapis.com/{version}/chromedriver_win32.zip";
      var fileName = "chromedriver_win32.zip";

      TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

      using (var wc = new WebClient())
      {
        wc.DownloadFileCompleted += (x, y) =>
        {
          File.Delete("chromedriver.exe");

          ZipFile.ExtractToDirectory(fileName, chromeDriverLocation);

          File.Delete(fileName);

          tcs.SetResult(false);
        };

        wc.DownloadFileAsync(new Uri(downloadPage), fileName);
      }

      return tcs.Task;
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
    public string SafeNavigate(string url, double secondsToWait)
    {
      lock (lockWait)
      {
        if (!Initialize())
        {
          return null;
        }

        wait = new WebDriverWait(ChromeDriver, TimeSpan.FromSeconds(secondsToWait));

        if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var validUrl))
        {
          var result = wait.Until((x) =>
          {
            var navigation = x.Navigate();

            navigation.GoToUrl(validUrl);

            try
            {
              return x.PageSource;
            }
            catch (Exception ex)
            {
              return null;
            }
           
          });

          return result;
        }

        return null;
      }
    }

    #endregion

    #region ExecuteScript

    public object ExecuteScript(string script, double secondsToWait)
    {
      WebDriverWait wait = new WebDriverWait(ChromeDriver, TimeSpan.FromSeconds(secondsToWait));

      return wait.Until(x =>
      {
        try
        {
          return ChromeDriver.ExecuteScript(script);
        }
        catch (Exception ex)
        {

          throw;
        }
      });
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

    #region Dispose

    public void Dispose()
    {
      ChromeDriver?.Dispose();
      wasInitilized = false;
      ChromeDriver = null;
    }

    #endregion
  }
}