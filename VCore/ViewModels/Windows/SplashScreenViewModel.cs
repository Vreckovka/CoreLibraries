﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using VCore.Standard.Providers;

namespace VCore.WPF.ViewModels.Windows
{
  public class SplashScreenViewModel : BaseWindowViewModel
  {
    public SplashScreenViewModel(Assembly assembly)
    {
      var assemblyName = assembly.GetName();

      ApplicationName = assemblyName.Name;

      ApplicationVersion = BasicInformationProvider.GetFormattedBuildVersion(assembly);

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

    #region ApplicationVersion

    private string applicationVersion;
    public string ApplicationVersion
    {
      get
      {
        return applicationVersion;
      }
      set
      {
        if (applicationVersion != value)
        {
          applicationVersion = value;
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
