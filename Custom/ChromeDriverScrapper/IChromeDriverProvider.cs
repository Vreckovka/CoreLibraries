using System;
using System.Collections.Generic;
using OpenQA.Selenium.Chrome;

namespace ChromeDriverScrapper
{
  public interface IChromeDriverProvider
  {
    ChromeDriver ChromeDriver { get; set; }
    bool Initialize(string proxyServer = null, List<string> options = null);
    Exception InitializeWithExceptionReturn(string proxyServer = null, List<string> options = null);
    object ExecuteScript(string script, double secondsToWait = 10);
    void ExecuteScriptVoid(string script, double secondsToWait);
    void Dispose();
    public string SafeNavigate(string url, double secondsToWait = 10);
  }
}