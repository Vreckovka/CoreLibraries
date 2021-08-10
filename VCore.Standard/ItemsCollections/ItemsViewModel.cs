using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using VCore.ItemsCollections;
using VCore.Standard;
using VCore.Standard.ViewModels.TreeView;

namespace VCore.WPF.ItemsCollections
{
  public class ItemsViewModel<T> : ViewModel where T : class, INotifyPropertyChanged
  {

    private ReplaySubject<T> actualItemSubject = new ReplaySubject<T>(1);


    public ItemsViewModel()
    {
      View.ItemUpdated.Subscribe(OnViewItemChanged);
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

          if (selectedItem is ISelectable selectable)
          {
            selectable.IsSelected = true;
          }

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

      if (CanAddToView())
      {
        View.Add(item);
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

    #region Clear

    public void Clear()
    {
      View.Clear();
      ViewModels.Clear();
    }

    #endregion

    public void AddRange(IEnumerable<T> items)
    {
      foreach (var item in items)
      {
        Add(item);
      }
    }

    #region CanAddToView

    private bool CanAddToView()
    {
      return true;
    }

    #endregion

    #region OnViewItemChanged

    private void OnViewItemChanged(EventPattern<PropertyChangedEventArgs> eventPattern)
    {
      if (eventPattern.Sender is ISelectable selectable && eventPattern.EventArgs.PropertyName == nameof(ISelectable.IsSelected))
      {
        if (selectable.IsSelected)
        {
          SelectedItem = (T)eventPattern.Sender;
        }
        else
        {
          SelectedItem = null;
        }
      }
    }

    #endregion

    #region Filter

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