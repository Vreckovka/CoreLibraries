using System;
using System.Collections.Generic;
using System.Linq;

namespace VCore.Standard.Helpers
{
  public static class CollectionExtentions
  {
    #region DistictBy

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
      return DistinctBy(source, keySelector, EqualityComparer<TKey>.Default);
    }

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
    {
      if (source == null) throw new ArgumentNullException("source");
      if (keySelector == null) throw new ArgumentNullException("keySelector");

      var distinctValues = new HashSet<TKey>(comparer);

      return source.Where(item => distinctValues.Add(keySelector(item)));
    }

    #endregion

    #region SplitList

    public static IEnumerable<List<T>> SplitList<T>(this IEnumerable<T> locations, int nSize = 30)
    {
      var list = locations.ToList();

      for (int i = 0; i < list.Count; i += nSize)
      {
        yield return list.GetRange(i, Math.Min(nSize, list.Count - i));
      }
    } 

    #endregion
  }
}
