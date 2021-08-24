using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using VCore.ViewModels;

namespace VCore.WPF.ViewModels.Windows
{
  public class SplashScreenViewModel : BaseWindowViewModel
  {
    public SplashScreenViewModel()
    {
      ApplicationName = Assembly.GetExecutingAssembly().FullName;
      Message = "Loading...";
      progress = 0;
    }

    #region ApplicationName

    private string applicationName;
    public string ApplicationName
    {
      get
      {
        return applicationName;
      }
      set
      {
        if (applicationName != value)
        {
          applicationName = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region Message

    private string message;
    public string Message
    {
      get
      {
        return message;
      }
      set
      {
        if (message != value)
        {
          message = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region Progress

    private double progress;
    public double Progress
    {
      get
      {
        return progress;
      }
      set
      {
        if (progress != value)
        {
          progress = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion
  }
}
