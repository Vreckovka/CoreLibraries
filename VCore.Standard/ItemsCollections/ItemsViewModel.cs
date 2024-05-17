using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using VCore.ItemsCollections;
using VCore.Standard;
using VCore.Standard.Helpers;
using VCore.Standard.ViewModels.TreeView;

namespace VCore.WPF.ItemsCollections
{
  public class ItemsViewModel<T> : ViewModel where T : class, INotifyPropertyChanged, ISelectable
  {

    private ReplaySubject<T> actualItemSubject = new ReplaySubject<T>(1);
    private SerialDisposable serialDisposable = new SerialDisposable();

    public ItemsViewModel()
    {
      serialDisposable.Disposable = View.ItemUpdated.Subscribe(OnViewItemChanged);
    }

    public ItemsViewModel(IEnumerable<T> items)
    {
      AddRange(items);
    }

    #region Properties

    #region View

    private RxObservableCollection<T> view = new RxObservableCollection<T>();

    public RxObservableCollection<T> View
    {
      get
      {
        return view;
      }
      set
      {
        if (value != view)
        {
          view = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    public ObservableCollection<T> ViewModels { get; set; } = new ObservableCollection<T>();

    #region OnActualItemChanged

    public IObservable<T> OnActualItemChanged
    {
      get
      {
        return actualItemSubject.AsObservable();
      }
    }

    #endregion

    #region SelectedItem

    private T selectedItem;

    public T SelectedItem
    {
      get { return selectedItem; }
      set
      {
        if (value != selectedItem)
        {
          selectedItem = value;
          actualItemSubject.OnNext(selectedItem);
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #endregion

    #region Methods

    #region Add

    public void Add(T item)
    {
      ViewModels.Add(item);

      if (CanAddToView(item))
      {
        View.Add(item);
      }
    }

    #endregion

    #region Insert

    public void Insert(int index, T item)
    {
      ViewModels.Insert(index, item);

      if (actualFilter != null)
      {
        if (actualFilter(item))
          View.Add(item);
      }
      else
      {
        View.Insert(index, item);
      }
    }

    #endregion

    #region Remove

    public void Remove(T item)
    {
      ViewModels.Remove(item);
      View.Remove(item);
    }

    #endregion

    #region RemoveAt

    public void RemoveAt(int index)
    {
      var itemAtIndex = ViewModels[index];

      ViewModels.RemoveAt(index);
      View.Remove(itemAtIndex);
    }

    #endregion

    #region Clear

    public void Clear()
    {
      View.Clear();
      ViewModels.Clear();
    }

    #endregion

    #region AddRange

    public void AddRange(IEnumerable<T> items)
    {
      foreach (var item in items)
      {
        Add(item);
      }
    }

    #endregion
    
    #region CanAddToView

    private bool CanAddToView(T item)
    {
      if (actualFilter != null)
      {
        return actualFilter(item);
      }

      return true;
    }

    #endregion

    #region OnViewItemChanged

    private void OnViewItemChanged(EventPattern<PropertyChangedEventArgs> eventPattern)
    {
      if (eventPattern.Sender is ISelectable selectable && eventPattern.EventArgs.PropertyName == nameof(ISelectable.IsSelected))
      {
        serialDisposable.Disposable.Dispose();

        var oldItem = SelectedItem;
        var newItem = (T)eventPattern.Sender;

        if (selectable.IsSelected)
        {
          SelectedItem = newItem;

          if (newItem is ISelectable newSelectable)
          {
            newSelectable.IsSelected = true;
          }
        }
        else
        {
          SelectedItem = null;
        }

        if (oldItem is ISelectable oldSelectable && oldItem != null)
        {
          oldSelectable.IsSelected = false;
        }

        serialDisposable.Disposable = View.ItemUpdated.Skip(1).Subscribe(OnViewItemChanged);
      }
    }

    #endregion

    #region Filter

    private Func<T, bool>? actualFilter = null;

    public void Filter(Func<T, bool> canAddToView)
    {
      View.Clear();

      foreach (var item in ViewModels)
      {
        if (canAddToView(item))
        {
          View.Add(item);
        }
      }

      actualFilter = canAddToView;
    }

    #endregion

    #region ResetFilter

    public void ResetFilter()
    {
      View.Clear();

      foreach (var item in ViewModels)
      {
        View.Add(item);
      }

      actualFilter = null;
    }

    #endregion

    #region Dispose

    public override void Dispose()
    {
      base.Dispose();

      actualItemSubject?.Dispose();
      View?.Dispose();
    }

    #endregion

    #endregion
  }
}