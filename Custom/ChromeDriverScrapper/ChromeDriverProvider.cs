using System;
using System.Collections.Generic;
using System.IO;
using Logger;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;

namespace ChromeDriverScrapper
{

  public class ChromeDriverProvider : IChromeDriverProvider
  {
    private readonly ILogger logger;
    private bool wasInitilized;
    public ChromeDriver ChromeDriver { get; set; }

    public ChromeDriverProvider(ILogger logger)
    {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Initialize

    public bool Initialize()
    {
      try
      {
        if (!wasInitilized)
        {
          var chromeOptions = new ChromeOptions();
          
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
            "--test-type",
            "--test-type=browser",
            "--ignore-certificate-errors",
            "--ignore-certificate-errors",
          });

          chromeOptions.AddArgument("--user-agent=Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.50 Safari/537.36");
          chromeOptions.Proxy = null;

          var dir = Directory.GetCurrentDirectory();
          var chromeDriverService = ChromeDriverService.CreateDefaultService(dir, "chromedriver.exe");

          chromeDriverService.HideCommandPromptWindow = true;

          ChromeDriver = new ChromeDriver(chromeDriverService, chromeOptions);

          //var userAgent = ChromeDriver.ExecuteScript("return navigator.userAgent;");

          wasInitilized = true;
        }

        return wasInitilized;
      }
      catch (Exception ex)
      {
        logger.Log(ex);

        return false;
      }
    }

    #endregion

    #region SafeNavigate

    public string SafeNavigate(string url, double secondsToWait)
    {
      if (!Initialize())
      {
        return null;
      }

      WebDriverWait wait = new WebDriverWait(ChromeDriver, TimeSpan.FromSeconds(secondsToWait));

      return wait.Until((x) =>
      {
        ChromeDriver.Navigate().GoToUrl(url);

        return ChromeDriver.PageSource;
      });
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