using System;
using System.Collections.Generic;
using System.IO;
using Logger;
using OpenQA.Selenium.Chrome;
using VPlayer.AudioStorage.Scrappers.CSFD;

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

          chromeOptions.AddArguments(new List<string>() { "headless", "disable-infobars", "--log-level=3" });

          var dir = Directory.GetCurrentDirectory();
          var chromeDriverService = ChromeDriverService.CreateDefaultService(dir, "chromedriver.exe");

          chromeDriverService.HideCommandPromptWindow = true;

          ChromeDriver = new ChromeDriver(chromeDriverService, chromeOptions);

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
  }
}