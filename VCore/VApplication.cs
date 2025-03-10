using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;
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
using Application = System.Windows.Application;
using Control = System.Windows.Controls.Control;
using ButtonBase = System.Windows.Controls.Primitives.ButtonBase;
using VCore.WPF.Misc;
using VCore.WPF.Interfaces;

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
    private bool isConsoleUp = false;
    protected ILogger logger;
    protected IWindowManager windowManager;

    #region BuildVersion

    public static string BuildVersion { get; set; }

    protected virtual bool IsConsoleVisible { get; set; }

    #endregion

    public virtual bool SaveWindowPosition { get; }

    private int numberOfSteps = 18;
    private SaveWindowsPositionFunction saveWindowsPositionFunction;


    #region Methods

    #region OnStartup

    protected override void OnStartup(StartupEventArgs e)
    {
      stopWatch = new Stopwatch();
      stopWatch.Start();

      ButtonBase.FocusableProperty.OverrideMetadata(typeof(ButtonBase), new FrameworkPropertyMetadata(false));

      Control.IsTabStopProperty.OverrideMetadata(typeof(Control), new FrameworkPropertyMetadata(false));
      ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));

      SplashScreenManager.ShowSplashScreen<TSplashScreen>(System.Reflection.Assembly.GetEntryAssembly());

      VSynchronizationContext.UISynchronizationContext = SynchronizationContext.Current;
      VSynchronizationContext.UIDispatcher = Application.Current.Dispatcher;

#if DEBUG
      //IsConsoleVisible = true;
#endif


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
      Kernel.Load<LoggerModule>();
      
      LoadModules();

      OnContainerCreated();

      CultureInfo.CurrentCulture = new CultureInfo("en-US");

      logger = Container.Resolve<ILogger>();
      windowManager = Container.Resolve<IWindowManager>();


      ShowConsole();


      var assembly = System.Reflection.Assembly.GetEntryAssembly();

      BuildVersion = BasicInformationProvider.GetFormattedBuildVersion(assembly);

      //Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

      //DateTime buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);

      //BuildVersion = Container.Resolve<IBasicInformationProvider>().GetFormattedExecutingAssemblyBuildVersion();


      SplashScreenManager.AddProgress(100.0 / numberOfSteps);

    }

    #endregion

    #region ShowConsole

    protected virtual void ShowConsole()
    {
      if (IsConsoleVisible)
      {
        isConsoleUp = WinConsole.CreateConsole();

      }
    }

    #endregion

    #region LoadModules

    protected virtual void LoadModules()
    {

    }

    #endregion

    #region OnContainerCreated

    protected virtual void OnContainerCreated()
    {


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
      
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine($"Console is ready");
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
        AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();

        message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
      }
      catch (Exception ex)
      {
        logger.Log(ex);
      }
      finally
      {
        logger.Log(exception);

        //if (Current?.Dispatcher != null && !(exception is ResourceReferenceKeyNotFoundException))
        //{
        //  await VSynchronizationContext.InvokeOnDispatcherAsync(() =>
        //  {
        //    if (!FullScreenManager.IsFullscreen)
        //      windowManager.ShowErrorPrompt(exception);
        //  });
        //}

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


      if (SaveWindowPosition)
      {
        saveWindowsPositionFunction = new SaveWindowsPositionFunction(shell);
      }


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
      Task.Run(async () =>
      {
        await Task.Delay(TimeSpan.FromSeconds(10));

        Process.GetCurrentProcess().Kill();
      });


      base.OnExit(e);

      logger.Dispose();

      if (isConsoleUp)
        isConsoleUp = !WinConsole.FreeConsole();

      Kernel.Dispose();
    }

    #endregion

    #endregion
  }

  public static class VSynchronizationContext
  {
    public static SynchronizationContext UISynchronizationContext { get; set; }

    public static Dispatcher UIDispatcher { get; set; }

    public static void PostOnUIThread(Action action)
    {
      UISynchronizationContext.Post(x => action(), null);
    }

    public static async Task InvokeOnDispatcherAsync(Action action)
    {
      try
      {
        await UIDispatcher?.InvokeAsync(action);
      }
      catch (TaskCanceledException)
      {
      }
    }

    public static void InvokeOnDispatcher(Action action)
    {
      try
      {
        UIDispatcher?.Invoke(action);
      }
      catch (TaskCanceledException)
      {
      }
    }
  }
}