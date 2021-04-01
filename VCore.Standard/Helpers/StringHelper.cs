using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

    #region ChunkSimilarity

    public static double ChunkSimilarity(this string original, string predicate)
    {
      if (original.Length > predicate.Length)
      {
        var splits = original.SplitToChunks(predicate.Length);

        return splits.Max(x => x.Similarity(predicate));
      }
      else
      {
        return original.Similarity(predicate);
      }

    }

    #endregion

    public static IEnumerable<string> SplitToChunks(this string str, int chunkSize)
    {
      var list = new List<string>();

      for (int i = 0; i < str.Length; i += chunkSize)
      {
        if (i + chunkSize < str.Length)
          list.Add(str.Substring(i, chunkSize));
        else
        {
          var diff = -1 * ( str.Length - i - chunkSize);
          list.Add(str.Substring(i - diff, chunkSize));
        }
      }

      return list;
    }
  }
}
