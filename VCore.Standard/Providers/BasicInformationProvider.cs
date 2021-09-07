using System;
using System.Reflection;

namespace VCore.Standard.Providers
{
  public static class BasicInformationProvider
  {
   public static string GetFormattedBuildVersion(Assembly assembly)
    {
      var assemblyName = assembly.GetName();

      Version version = assemblyName.Version;

      DateTime buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);

      if ((DateTime.Now - buildDate).TotalDays <= 2)
      {
        return $"{version} ({buildDate.ToString("dd.MM.yyyy")} {buildDate.ToString("HH:mm")})";
      }
      else
      {
        return $"{version} ({buildDate.ToString("dd.MM.yyyy")})";
      }
    }

  }
}
