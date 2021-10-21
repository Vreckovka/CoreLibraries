using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Ninject;
using VCore.Standard.Common;

namespace VCore.Standard
{

  public interface IParametrizedViewModel
  {
  }

  public interface IViewModel : IInitializable, IDisposable, INotifyPropertyChanged
  {
  }

  public interface IViewModel<TModel> : IViewModel
  {
    #region Properties

    TModel Model { get; set; }

    #endregion Properties
  }


  public abstract class ViewModel : VBindableBase, IViewModel
  {

    [JsonIgnore]
    public bool WasInitilized { get; set; }

    #region Methods

    public virtual void Initialize()
    {
      //if (WasInitilized)
      //{
      //  return;
      //}

      WasInitilized = true;
    }

    #endregion
  }

  public abstract class ViewModel<TModel> : ViewModel, IParametrizedViewModel, IViewModel<TModel>
  {
    #region Constructors

    public ViewModel(TModel model)
    {
      Model = model;
    }

    #endregion Constructors

    #region Properties

    public virtual TModel Model { get; set; }

    #endregion Properties
  }

  public abstract class SelectableViewModel<TModel> : ViewModel<TModel>
  {
    protected SelectableViewModel(TModel model) : base(model)
    {
    }

    private bool isSelected;

    public bool IsSelected
    {
      get
      {
        return isSelected;
      }
      set
      {
        if (value != isSelected)
        {
          isSelected = value;
          OnSelectionChanged(value);
          RaisePropertyChanged();
        }
      }
    }

    protected virtual void OnSelectionChanged(bool newValue)
    {

    }
  }
}