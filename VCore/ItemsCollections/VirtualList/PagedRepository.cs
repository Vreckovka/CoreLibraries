using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace VCore.ItemsCollections.VirtualList
{
  public class PagedRepository<TItem> where TItem : class
  {
    #region Fields

    public int NumberOfItems = -1;
    public readonly TItem[] source;
    private readonly int pageSize;
    private TItem[] _cache;

    #endregion Fields

    #region Constructors

    public PagedRepository(TItem[] source, int pageSize)
    {
      if (pageSize <= 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

      this.source = source ?? throw new ArgumentNullException(nameof(source));
      this.pageSize = pageSize;

      NumberOfItems = source.Length;
    }

    #endregion Constructors

    #region Properties

    public int PageCursor { get; set; }

    public int PageSize
    {
      get { return pageSize; }
    }

    private int PageCursorStartIndex
    {
      get { return PageCursor * PageSize; }
    }

    #endregion Properties

    #region Methods

    public void Clear()
    {
      _cache = null;
    }

    public int Count()
    {
      var items = DoLoadPage();
      _cache = new TItem[NumberOfItems];
      UpdateCache(items);
      return NumberOfItems;
    }

    public TItem GetAt(int index)
    {
      if (index < NumberOfItems && _cache[index] == null)
      {
        PageCursor = index / PageSize;
        LoadPage();
      }
      if (index >= NumberOfItems)
        throw new ArgumentException(string.Format("item index [{0}] is exceed the upper boundary [{1}]", index,
          NumberOfItems));
      return _cache[index];
    }

    public int IndexOf(TItem item)
    {
      var result = _cache.Where(x => x != null & x == item).Select((x, index) => new { Position = index, item }).
        FirstOrDefault();
      return result != null ? result.Position : -1;
    }

    protected virtual List<TItem> DoLoadPage()
    {
      var items = new List<TItem>();
      for (int i = PageCursorStartIndex; i < PageCursorStartIndex + PageSize; i++)
      {
        if (source.Length > i)
        {
          //Thread.Sleep(TimeSpan.FromMilliseconds(1));
          items.Add(source[i]);
        }
      }
      return items;
    }

    protected List<TItem> LoadPage()
    {
      var employees = DoLoadPage();
      UpdateCache(employees);
      return employees;
    }

    private void UpdateCache(List<TItem> compounds)
    {
      for (int i = PageCursorStartIndex; i < PageCursorStartIndex + compounds.Count; i++)
      {
        if (i < _cache.Length)
          _cache[i] = compounds[i - PageCursorStartIndex];
      }
    }

    #endregion Methods
  }
}