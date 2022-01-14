using Logger;

namespace VCore.Standard.Modularity.NinjectModules
{
  public class LoggerModule : BaseNinjectModule
  {
    private string logFilePath = "Loggs\\logs.txt";

    public override void RegisterProviders()
    {
      base.RegisterProviders();

      Kernel.Bind<FileLoggerContainer>().ToSelf().WithConstructorArgument("logFilePath", logFilePath);
      Kernel.Bind<ILogger>().To<Logger.Logger>().InSingletonScope();
      Kernel.Bind<ILoggerContainer>().To<Logger.ConsoleLogger>();
    }

  }
}