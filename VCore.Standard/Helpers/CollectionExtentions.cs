﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    #region AddRange

    public static void AddRange<TItem>(this ObservableCollection<TItem> o, IEnumerable<TItem> items)
    {
      foreach (var item in items)
      {
        o.Add(item);
      }
    }

    #endregion

    #region Sort

    public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
    {
      var sortableList = new List<T>(collection);
      sortableList.Sort(comparison);

      for (int i = 0; i < sortableList.Count; i++)
      {
        collection.Move(collection.IndexOf(sortableList[i]), i);
      }
    }

    #endregion

    #region ForEach

    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
     foreach(var item in collection)
     {
       action.Invoke(item);
     }
    }

    #endregion

    #region IndexOf

    public static int? IndexOf<T>(this IList<T> collection, Func<T,bool> action)
    {
      var list = collection.ToList();

      for (int i = 0; i < list.Count; i++)
      {
        if (action.Invoke(list[i]))
        {
          return i;
        }
      }

      return null;
    }

    #endregion
  }
}
