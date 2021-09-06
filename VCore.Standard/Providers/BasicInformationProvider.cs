using System;
using System.Reflection;

namespace VCore.Standard.Providers
{
  public class VPlayerBasicInformationProvider : IBasicInformationProvider
  {
    public string GetBuildVersion(string stringFormat = "dd.MM.yyyy")
    {
      Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

      DateTime buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);

      return $"{version} ({buildDate.ToString(stringFormat)})";
    }

    public string GetFormattedExecutingAssemblyBuildVersion()
    {
      var assembly = Assembly.GetExecutingAssembly();

      return GetFormattedBuildVersion(assembly);
    }

    public string GetFormattedBuildVersion(Assembly assembly)
    {
      var assemblyName = assembly.GetName();

      Version version = assemblyName.Version;

      DateTime buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);

      if ((DateTime.Now - buildDate).TotalDays <= 2)
      {
        return $"{version} ({buildDate.ToString("dd.MM.yyyy")} {buildDate.ToShortTimeString()})";
      }
      else
      {
        return $"{version} ({buildDate.ToString("dd.MM.yyyy")})";
      }
    }
  }
}
