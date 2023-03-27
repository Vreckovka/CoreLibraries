using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using VCore.Standard.Helpers;

namespace VCore.ItemsCollections
{
  public class RxObservableCollection<TItem> : ObservableCollection<TItem>, IDisposable where TItem : class, INotifyPropertyChanged
  {
    #region Fields

    private List<IDisposable> itemsDisposables = new List<IDisposable>();

    #endregion Fields

    #region Constructors

    public RxObservableCollection()
    {
      EnableNotification();
    }

    public RxObservableCollection(IEnumerable<TItem> items)
    {
      foreach (var item in items)
      {
        Add(item);

        itemsDisposables.Add(Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          x => item.PropertyChanged += x,
          x => item.PropertyChanged -= x).Subscribe(ItemPropertyChanged));
      }
      OrderedCollection = this.OrderBy(KeySelector);
      View.AddRange(items);

      if (SortType != null)
        View.Sort(SortType);

      CollectionChanged += ObservableItems_CollectionChanged;
    }

    #endregion Constructors

    #region Properties

    #region ItemAdded

    private ReplaySubject<EventPattern<TItem>> itemAddedSubject = new ReplaySubject<EventPattern<TItem>>(1);

    public IObservable<EventPattern<TItem>> ItemAdded
    {
      get
      {
        return itemAddedSubject.AsObservable();
      }
    }

    #endregion

    #region ItemRemoved

    private ReplaySubject<EventPattern<TItem>> itemRemovedSubject = new ReplaySubject<EventPattern<TItem>>(1);

    public IObservable<EventPattern<TItem>> ItemRemoved
    {
      get
      {
        return itemRemovedSubject.AsObservable();
      }
    }

    #endregion

    #region ItemUpdated

    private ReplaySubject<EventPattern<PropertyChangedEventArgs>> itemUpdatedSubject = new ReplaySubject<EventPattern<PropertyChangedEventArgs>>(1);

    public IObservable<EventPattern<PropertyChangedEventArgs>> ItemUpdated
    {
      get
      {
        return itemUpdatedSubject.AsObservable();
      }
    }

    #endregion

    #region Cleared

    private Subject<NotifyCollectionChangedEventArgs> clearedSubject = new Subject<NotifyCollectionChangedEventArgs>();

    public IObservable<NotifyCollectionChangedEventArgs> Cleared
    {
      get
      {
        return clearedSubject.AsObservable();
      }
    }

    #endregion

    public NotifyCollectionChangedAction? LastAction { get; private set; }

    #region SortType

    private Comparison<TItem> sortType;

    public Comparison<TItem> SortType
    {
      get { return sortType; }
      set
      {
        if (value != sortType)
        {
          sortType = value;

          if (sortType != null)
            View.Sort(sortType);
        }
      }
    }

    #endregion

    #region OrderedCollection

    private IEnumerable<TItem> orderedCollection;

    public IEnumerable<TItem> OrderedCollection
    {
      get { return orderedCollection; }
      set
      {
        if (value != orderedCollection)
        {
          orderedCollection = value;
          OnPropertyChanged(new PropertyChangedEventArgs(nameof(OrderedCollection)));
        }
      }
    }

    #endregion

    #region View

    public ObservableCollection<TItem> View { get; } = new ObservableCollection<TItem>();

    #endregion


    #endregion Properties

    #region Methods

    #region Dispose

    public void Dispose()
    {
      itemAddedSubject?.Dispose();
      itemRemovedSubject?.Dispose();
      itemUpdatedSubject?.Dispose();

      foreach (var disposable in itemsDisposables)
      {
        disposable.Dispose();
      }
    }

    #endregion

    #region ForEach

    public void ForEach(Action<TItem> action)
    {
      foreach (var item in this)
      {
        action.Invoke(item);
      }
    }

    #endregion

    #region ItemPropertyChanged

    private void ItemPropertyChanged(EventPattern<PropertyChangedEventArgs> eventPattern)
    {
      itemUpdatedSubject?.OnNext(eventPattern);

      if (SortType != null)
        View.Sort(SortType);
    }

    #endregion

    #region ObservableItems_CollectionChanged

    private void ObservableItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems != null)
      {
        foreach (var newItem in e.NewItems.OfType<TItem>())
        {
          itemsDisposables.Add(Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
            x => newItem.PropertyChanged += x,
            x => newItem.PropertyChanged -= x).Subscribe(ItemPropertyChanged));

          itemAddedSubject.OnNext(new EventPattern<TItem>(this, newItem));

          View.Add(newItem);
        }
      }

      if (e.OldItems != null)
      {
        foreach (var oldItem in e.OldItems.OfType<TItem>())
        {
          itemRemovedSubject.OnNext(new EventPattern<TItem>(this, oldItem));
          View.Remove(oldItem);
        }
      }

      OrderedCollection = this.OrderBy(KeySelector);

      if (SortType != null)
        View.Sort(SortType);

      LastAction = e.Action;

      if (e.Action == NotifyCollectionChangedAction.Reset)
      {
        clearedSubject.OnNext(e);
      }
    }

    #endregion

    public void DisableNotification()
    {
      CollectionChanged -= ObservableItems_CollectionChanged;
    }

    public void EnableNotification()
    {
      CollectionChanged += ObservableItems_CollectionChanged;
    }

    public void AddRange(IEnumerable<TItem> items)
    {
      foreach (var item in items)
      {
        Add(item);
      }
    }

    public void RemoveRange(IEnumerable<TItem> items)
    {
      foreach (var item in items)
      {
        Remove(item);
      }
    }

    protected virtual string KeySelector(TItem other)
    {
      return default;
    }

    #endregion 
  }
}