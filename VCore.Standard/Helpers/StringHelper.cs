using System;
using System.IO;

namespace VCore
{
  public static class StringHelper
  {
    public static void EnsureDirectoryExists(this string filePath)
    {
      FileInfo fi = new FileInfo(filePath);

      if (fi.Directory != null && !fi.Directory.Exists)
      {
        System.IO.Directory.CreateDirectory(fi.Directory.FullName);
      }
    }

    #region LevenshteinDistance

    public static int LevenshteinDistance(this string s, string t)
    {
      int n = s.Length;
      int m = t.Length;
      int[,] d = new int[n + 1, m + 1]; // matrix
      int cost = 0;

      if (n == 0) return m;
      if (m == 0) return n;

      // Initialize.
      for (int i = 0; i <= n; d[i, 0] = i++) ;
      for (int j = 0; j <= m; d[0, j] = j++) ;

      // Find min distance.
      for (int i = 1; i <= n; i++)
      {
        for (int j = 1; j <= m; j++)
        {
          cost = (t.Substring(j - 1, 1) == s.Substring(i - 1, 1) ? 0 : 1);
          d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
        }
      }

      return d[n, m];
    }

    #endregion

    #region Similarity

    /// <summary>
    /// Similarity use LevenshteinDistance retuns value between 0 and 1
    /// </summary>
    /// <param name="s"></param>
    /// <param name="t"></param>
    /// <param name="useAbsoluteString"></param>
    /// <param name="ignorCase"></param>
    /// <returns></returns>
    public static float Similarity(this string s, string t, bool useAbsoluteString = false, bool ignorCase = true)
    {
      if (useAbsoluteString)
      {
        s = s.ToLower().Replace(" ", string.Empty);
        t = t.ToLower().Replace(" ", string.Empty);
      }

      int maxLen = Math.Max(s.Length, t.Length);

      if (maxLen == 0)
      {
        return 1.0f;
      }

      if (ignorCase)
      {
        s = s.ToLowerInvariant();
        t = t.ToLowerInvariant();
      }

      float dis = LevenshteinDistance(s, t);

      return 1.0f - dis / maxLen;
    }

    #endregion
  }
}
