using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VCore.Standard.Modularity.Interfaces;

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

    #region AddRangeBatched

    public static void AddRangeBatched<TItem>(this ObservableCollection<TItem> o, IEnumerable<TItem> items, int batchSize = 20)
    {
      var lists = items.SplitList(batchSize);

      foreach (var list in lists)
      {
        foreach (var item in list)
        {
          o.Add(item);
        }
      }
    }

    #endregion

    public static TimeSpan Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, TimeSpan> func)
    {
      return new TimeSpan(source.Sum(item => func(item).Ticks));
    }

    #region Sort

    /// <summary>
    ///  Example: Orders.Sort((x, y) => x.Price.CompareTo(y.Price));
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="comparison"></param>
    public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
    {
      var sortableList = new List<T>(collection);
      sortableList.Sort(comparison);

      for (int i = 0; i < sortableList.Count; i++)
      {
        collection.Move(collection.IndexOf(sortableList[i]), i);
      }
    }

    public static void Sort<T>(this ObservableCollection<T> collection, Comparer<T> comparison)
    {
      var sortableList = new List<T>(collection);
      sortableList.Sort(comparison);

      for (int i = 0; i < sortableList.Count; i++)
      {
        collection.Move(collection.IndexOf(sortableList[i]), i);
      }
    }

    #endregion

    #region LinqSort

    /// <summary>
    /// Sorting observable by Linq. !!! Clearing collection
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="observableCollection"></param>
    /// <param name="keySelector"></param>
    public static void LinqSort<TSource, TKey>(this ObservableCollection<TSource> observableCollection, Func<TSource, TKey> keySelector)
    {
      var a = observableCollection.OrderBy(keySelector).ToList();
      observableCollection.Clear();
      foreach (var b in a)
      {
        observableCollection.Add(b);
      }
    }

    #endregion

    #region LinqSortDescending

    /// <summary>
    /// Sorting observable by Linq. !!! Clearing collection
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="observableCollection"></param>
    /// <param name="keySelector"></param>
    public static void LinqSortDescending<TSource, TKey>(this ObservableCollection<TSource> observableCollection, Func<TSource, TKey> keySelector)
    {
      var a = observableCollection.OrderByDescending(keySelector).ToList();
      observableCollection.Clear();
      foreach (var b in a)
      {
        observableCollection.Add(b);
      }
    }

    #endregion

    #region ForEach

    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
      foreach (var item in collection)
      {
        action.Invoke(item);
      }
    }

    #endregion

    #region IndexOf

    public static int? IndexOf<T>(this IList<T> collection, Func<T, bool> action)
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

    #region GetEnummerable

    public static IEnumerable<T> GetEnummerable<T>(this T item)
    {
      return new List<T>() { item }.AsEnumerable();
    }

    #endregion

    #region SelectManyRecursive

    public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
    {
      var result = source.SelectMany(selector);
      if (!result.Any())
      {
        return result;
      }
      return result.Concat(result.SelectManyRecursive(selector));
    }

    #endregion

    #region RefreshCollection

    /// <summary>
    /// Example: Orders.RefreshCollection(item.Orders,(x,y) => x.Model.order_id == y.Model.order_id);
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="collection"></param>
    /// <param name="newItems"></param>
    /// <param name="comparision"></param>
    public static void RefreshCollection<TItem>(
      this ObservableCollection<TItem> collection,
      IEnumerable<TItem> newItems,
      Func<TItem, TItem, bool> comparision) where TItem : IUpdateable<TItem>
    {
      var list = newItems.ToList();
      var notInCollection = collection.Where(x => list.All(y => !comparision(x, y))).ToList();

      foreach (var item in list)
      {
        var found = collection.SingleOrDefault(x => comparision(x, item));

        if (found == null)
        {
          if (item != null)
            collection.Add(item);
        }
        else
        {
          found.Update(item);
        }
      }

      foreach (var removedItem in notInCollection)
      {
        collection.Remove(collection.SingleOrDefault(x => comparision(x, removedItem)));
      }
    } 

    #endregion
  }
}
