using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Logger;
using Ninject;
using Prism.Ioc;
using Prism.Ninject;
using Prism.Regions;
using VCore.Modularity.NinjectModules;
using VCore.Other;
using VCore.Standard;
using VCore.Standard.Modularity.Interfaces;
using VCore.Standard.Modularity.NinjectModules;
using VCore.ViewModels;
using VCore.WPF.Managers;

namespace VCore.WPF
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  ///

  public abstract class VApplication<TMainWindow, TMainWindowViewModel, TSplashScreen> : PrismApplication
    where TMainWindow : Window
    where TMainWindowViewModel : BaseMainWindowViewModel
    where TSplashScreen : IView, new()
  {
    protected Stopwatch stopWatch;
    protected IKernel Kernel;
    protected bool isConsoleUp = false;
    protected ILogger logger;
    protected IWindowManager windowManager;

    #region BuildVersion

    public static string BuildVersion { get; set; }

    #endregion

    #region RegisterTypes

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {

      SplashScreenManager.SetText("Loading modules");

      Kernel = Container.GetContainer();

      VIoc.Kernel = Kernel;

      Kernel.Load<CommonNinjectModule>();
      Kernel.Load<WPFNinjectModule>();

      LoadModules();

      CultureInfo.CurrentCulture = new CultureInfo("en-US");

      logger = Container.Resolve<ILogger>();
      windowManager = Container.Resolve<IWindowManager>();

#if DEBUG

      isConsoleUp = WinConsole.CreateConsole();

      Console.ForegroundColor = ConsoleColor.Green;

      Console.WriteLine("CONSOLE IS READY");
#endif

      Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

      DateTime buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);

      BuildVersion = $"{version} ({buildDate.ToString("dd.MM.yyyy")})";

      SplashScreenManager.AddProgress(0.33);

    }

    #endregion

    #region LoadModules

    protected virtual void LoadModules()
    {

    }

    #endregion

    #region OnStartup

    protected override void OnStartup(StartupEventArgs e)
    {
      stopWatch = new Stopwatch();
      stopWatch.Start();

      SplashScreenManager.ShowSplashScreen<TSplashScreen>();

      Control.IsTabStopProperty.OverrideMetadata(typeof(Control), new FrameworkPropertyMetadata(false));

      base.OnStartup(e);
    }

    #endregion

    #region CreateShell

    protected override Window CreateShell()
    {
      SplashScreenManager.SetText("Creating shell");

      var vm = CreateMainWindow();

      vm.Loaded += MainWindow_Loaded;

      SplashScreenManager.AddProgress(0.33);

      return vm;
    }

    #endregion

    #region OnInitialized

    protected override void OnInitialized()
    {
      SplashScreenManager.SetText("Initilizing");

      base.OnInitialized();

      SplashScreenManager.AddProgress(0.33);

      SetupExceptionHandling();

      stopWatch.Stop();

      Console.WriteLine(stopWatch.Elapsed);
    }

    #endregion

    #region SetupExceptionHandling

    private void SetupExceptionHandling()
    {
      AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

      DispatcherUnhandledException += (s, e) =>
      {
        LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
        e.Handled = true;
      };

      TaskScheduler.UnobservedTaskException += (s, e) =>
      {
        LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
        e.SetObserved();
      };
    }

    #endregion

    #region LogUnhandledException

    private async void LogUnhandledException(Exception exception, string source)
    {
      string message = $"Unhandled exception ({source})";

      try
      {
        System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();

        message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
      }
      catch (Exception ex)
      {
        logger.Log(ex);
      }
      finally
      {
        logger.Log(MessageType.Error, message);
        logger.Log(exception);

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
          windowManager.ShowErrorPrompt(exception);
        });
      }
    }

    #endregion

    #region CreateMainWindow

    private TMainWindow CreateMainWindow()
    {
      var shell = Container.Resolve<TMainWindow>();

      RegionManager.SetRegionManager(shell, Kernel.Get<IRegionManager>());

      RegionManager.UpdateRegions();

      var dataContext = Kernel.Get<TMainWindowViewModel>();

      shell.DataContext = dataContext;

      return shell;
    }

    #endregion

    #region MainWindow_Loaded

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      SplashScreenManager.CloseActualSplashScreen();
    }

    #endregion

    #region OnExit

    protected override void OnExit(ExitEventArgs e)
    {
      if (isConsoleUp)
        isConsoleUp = !WinConsole.FreeConsole();

      base.OnExit(e);
    }

    #endregion

  }
}