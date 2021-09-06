using System.Reflection;

namespace VCore.Standard.Providers
{
  public interface IBasicInformationProvider
  {
    string GetBuildVersion(string stringFormat = "dd.MM.yyyy");
    string GetFormattedExecutingAssemblyBuildVersion();
    string GetFormattedBuildVersion(Assembly assembly);
  }
}