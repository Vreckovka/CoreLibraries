using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Listener;
using Logger;
using Ninject;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Ninject;
using Prism.Regions;
using VCore.Standard;
using VCore.Standard.Modularity.Interfaces;
using VCore.Standard.Modularity.NinjectModules;
using VCore.Standard.Providers;
using VCore.WPF.Interfaces.Managers;
using VCore.WPF.Managers;
using VCore.WPF.Modularity.NinjectModules;
using VCore.WPF.Other;
using VCore.WPF.ViewModels;

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

    protected virtual bool IsConsoleVisible { get; set; }

    #endregion

    private int numberOfSteps = 18;

    #region Methods

    #region OnStartup

    protected override void OnStartup(StartupEventArgs e)
    {
#if DEBUG
      IsConsoleVisible = true;
#endif

      stopWatch = new Stopwatch();
      stopWatch.Start();

      SplashScreenManager.ShowSplashScreen<TSplashScreen>(System.Reflection.Assembly.GetEntryAssembly());

      Control.IsTabStopProperty.OverrideMetadata(typeof(Control), new FrameworkPropertyMetadata(false));
      ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));

      base.OnStartup(e);
    }

    #endregion

    #region RegisterTypes

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      SplashScreenManager.SetText("Registering types");

      Kernel = Container.GetContainer();
      VIoc.Kernel = Kernel;

      Kernel.Load<CommonNinjectModule>();
      Kernel.Load<WPFNinjectModule>();

      OnContainerCreated();

      LoadModules();

      CultureInfo.CurrentCulture = new CultureInfo("en-US");

      logger = Container.Resolve<ILogger>();
      windowManager = Container.Resolve<IWindowManager>();


      ShowConsole();


      var version = System.Reflection.Assembly.GetExecutingAssembly().GetName();

      //Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

      //DateTime buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);

      //BuildVersion = Container.Resolve<IBasicInformationProvider>().GetFormattedExecutingAssemblyBuildVersion();


      SplashScreenManager.AddProgress(100.0 / numberOfSteps);

    }

    #endregion

    protected virtual void ShowConsole()
    {
      if (IsConsoleVisible)
      {
        isConsoleUp = WinConsole.CreateConsole();

        Console.ForegroundColor = ConsoleColor.Green;

        Console.WriteLine("CONSOLE IS READY");
      }
    }

    #region LoadModules

    protected virtual void LoadModules()
    {

    }

    #endregion

    #region OnContainerCreated

    protected virtual void OnContainerCreated()
    {
      Kernel.Load<LoggerModule>();

      SplashScreenManager.SetText("Loading settings");
    }

    #endregion

    #region Configure methods

    #region ConfigureModuleCatalog

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
      SplashScreenManager.SetText("Loading modules");

      base.ConfigureModuleCatalog(moduleCatalog);

      SplashScreenManager.AddProgress(100.0 / numberOfSteps);
    }

    #endregion

    #region ConfigureViewModelLocator

    protected override void ConfigureViewModelLocator()
    {
      SplashScreenManager.SetText("Configuring locator");

      base.ConfigureViewModelLocator();

      SplashScreenManager.AddProgress(100.0 / numberOfSteps);
    }

    #endregion

    #region ConfigureDefaultRegionBehaviors

    protected override void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors)
    {
      SplashScreenManager.SetText("Configuring region behaviors");

      base.ConfigureDefaultRegionBehaviors(regionBehaviors);

      SplashScreenManager.AddProgress(100.0 / numberOfSteps);
    }

    #endregion

    #region ConfigureRegionAdapterMappings

    protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
    {
      SplashScreenManager.SetText("Configuring adapter mappings");

      base.ConfigureRegionAdapterMappings(regionAdapterMappings);

      SplashScreenManager.AddProgress(100.0 / numberOfSteps);
    }

    #endregion

    #region ConfigureServiceLocator

    protected override void ConfigureServiceLocator()
    {
      SplashScreenManager.SetText("Configuring service locator");

      base.ConfigureServiceLocator();

      SplashScreenManager.AddProgress(100.0 / numberOfSteps);
    }

    #endregion

    #region CreateModuleCatalog

    protected override IModuleCatalog CreateModuleCatalog()
    {
      SplashScreenManager.SetText("Creating module catalog");

      var catalog = base.CreateModuleCatalog();

      SplashScreenManager.AddProgress(100.0 / numberOfSteps);

      return catalog;
    }

    #endregion

    #endregion

    #region CreateShell

    protected override Window CreateShell()
    {
      SplashScreenManager.SetText("Creating shell");

      var vm = CreateMainWindow();

      vm.Loaded += MainWindow_Loaded;

      SplashScreenManager.AddProgress(25);

      return vm;
    }

    #endregion

    #region OnInitialized

    protected override void OnInitialized()
    {
      SetupExceptionHandling();

      SplashScreenManager.SetText("Initilizing");

      base.OnInitialized();

      SplashScreenManager.AddProgress(25);

      stopWatch.Stop();

      Console.WriteLine(stopWatch.Elapsed);
    }

    #endregion

    #region SetupExceptionHandling

    private void SetupExceptionHandling()
    {

      //#if !DEBUG
      AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

      DispatcherUnhandledException += (s, e) =>
      {
        LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
        e.Handled = true;
      };
      //#endif
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
        logger.Log(exception);

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
          windowManager.ShowErrorPrompt(exception);
        });

        OnUnhandledExceptionCaught(exception);
      }
    }

    #endregion

    #region OnUnhandledExceptionCaught

    protected virtual void OnUnhandledExceptionCaught(Exception exception)
    {

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

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      SplashScreenManager.CloseActualSplashScreen();

      Application.Current.MainWindow.Topmost = true;

      await Task.Delay(250);

      Application.Current.MainWindow.Topmost = false;
    }

    #endregion

    #region OnExit

    protected override void OnExit(ExitEventArgs e)
    {
      base.OnExit(e);

      if (isConsoleUp)
        isConsoleUp = !WinConsole.FreeConsole();

      Kernel.Dispose();

     // Process.GetCurrentProcess().Kill();
    }

    #endregion

    #endregion
  }
}