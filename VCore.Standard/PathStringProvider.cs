using System.IO;
using System.Text.RegularExpressions;

namespace VCore.Standard
{
  public static class PathStringProvider
  {
    #region GetPathValidName

    public static string GetPathValidName(string name)
    {
      if (string.IsNullOrEmpty(name))
      {
        return null;
      }

      string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

      foreach (char c in invalid)
      {
        name = name.Replace(c.ToString(), "-");
      }

      return name.Trim();
    }

    #endregion
  }
}