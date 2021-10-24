using System.IO;
using System.Text.RegularExpressions;

namespace VCore.Standard
{
  public static class PathStringProvider
  {
    #region GetNormalizedName

    public static string GetNormalizedName(string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return input;
      }

      Regex rgx = new Regex("[^a-zA-Z0-9]");

      return rgx.Replace(input.ToLower(), "");
    }

    #endregion

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