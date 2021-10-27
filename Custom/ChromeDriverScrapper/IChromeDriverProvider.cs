using System;
using OpenQA.Selenium.Chrome;

namespace ChromeDriverScrapper
{
  public interface IChromeDriverProvider
  {
    ChromeDriver ChromeDriver { get; set; }
    bool Initialize();
    Exception InitializeWithExceptionReturn();
    bool IsVersionExeception(Exception ex);
    object ExecuteScript(string script, double secondsToWait = 10);
    void ExecuteScriptVoid(string script, double secondsToWait);
    void Dispose();
    public string SafeNavigate(string url, double secondsToWait = 10);
  }
}