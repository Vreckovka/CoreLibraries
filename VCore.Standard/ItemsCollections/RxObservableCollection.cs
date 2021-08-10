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

namespace VCore.ItemsCollections
{
  public class RxObservableCollection<TItem> : ObservableCollection<TItem>, IDisposable where TItem : INotifyPropertyChanged
  {
    #region Fields

    private List<IDisposable> itemsDisposables = new List<IDisposable>();

    #endregion Fields

    #region Constructors

    public RxObservableCollection()
    {
      CollectionChanged += ObservableItems_CollectionChanged;
    }

    public RxObservableCollection(IEnumerable<TItem> items)
    {
      foreach (var item in items)
      {
        Add(item);
      }
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

    public NotifyCollectionChangedAction? LastAction { get; private set; }

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
    }

    #endregion

    #region ObservableItems_CollectionChanged

    private void ObservableItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems != null)
      {
        foreach (var newItem in e.NewItems.OfType<TItem>())
        {
          itemsDisposables.Add(Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
            x => newItem.PropertyChanged += x,
            x => newItem.PropertyChanged -= x).Subscribe(ItemPropertyChanged));

          itemAddedSubject.OnNext(new EventPattern<TItem>(this, newItem));
        }
      }

      if (e.OldItems != null)
      {
        foreach (var oldItem in e.OldItems.OfType<TItem>())
        {
          itemRemovedSubject.OnNext(new EventPattern<TItem>(this, oldItem));
        }
      }

      LastAction = e.Action;
    }

    #endregion

    #endregion 
  }
}