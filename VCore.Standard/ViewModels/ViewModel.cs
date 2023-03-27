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
      this.model = model;
    }

    #endregion Constructors

    #region Properties

    private TModel model;

    public virtual TModel Model
    {
      get { return model; }
      set
      {
        model = value;
        RaisePropertyChanged();
      }
    }
  }

  #endregion Properties
}
