using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

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
      if (s == null || t == null)
      {
        return 0;
      }

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

      var sim = 1.0f - dis / maxLen;

      return sim;
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

    #region SimilarityByTags

    public static bool SimilarityByTags(this string original, string predicate)
    {
      var originalSplits = original.ToLower().Split(" ").ToList();
      var predicateSplits = predicate.ToLower().Split(" ").Select(x => x.ToLower()).ToList();

      var orderered = predicateSplits.OrderBy(x => x.Length).ToList();

      var min = orderered.First().Length;
      var max = orderered.Last().Length;

      originalSplits.AddRange(original.SplitToChunks(min < 1 ? 2 : min));
      originalSplits.AddRange(original.SplitToChunks(max < 1 ? 2 : max));

      originalSplits = originalSplits.Select(x => x.ToLower()).Distinct().ToList();

      bool result = false;

      int resultCount = 0;


      foreach (var originalSplit in originalSplits.Where(x => !string.IsNullOrEmpty(x)))
      {
        foreach (var tag in predicateSplits)
        {
          if (tag.Length == originalSplit.Length)
            result = originalSplit.Contains(tag);

          if (result)
          {
            break;
          }
        }

        if (result)
        {
          resultCount++;
          result = false;
        }

        if (resultCount >= predicateSplits.Count)
        {
          return true;
        }
      }

      if (!result)
      {
        return original.ToLower().Contains(orderered.Last());
      }

      return result;
    }

    #endregion

    #region SplitToChunks

    public static IEnumerable<string> SplitToChunks(this string str, int chunkSize)
    {
      var list = new List<string>();

      for (int i = 0; i < str.Length; i += chunkSize)
      {
        if (i + chunkSize < str.Length)
          list.Add(str.Substring(i, chunkSize));
        else
        {
          var diff = -1 * (str.Length - i - chunkSize);
          list.Add(str.Substring(i - diff, chunkSize));
        }
      }

      return list;
    }

    #endregion

    public static string RemoveDiacritics(this string text)
    {
      if (string.IsNullOrEmpty(text))
      {
        return text;
      }

      var normalizedString = text.Normalize(NormalizationForm.FormD);
      var stringBuilder = new StringBuilder();

      foreach (var c in normalizedString)
      {
        var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
        if (unicodeCategory != UnicodeCategory.NonSpacingMark)
        {
          stringBuilder.Append(c);
        }
      }

      return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
  }
}
