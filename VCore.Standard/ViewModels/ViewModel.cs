using System;
using System.ComponentModel;
using Ninject;
using VCore.Standard.Common;

namespace VCore.Standard
{
  public interface IParametrizedViewModel
  {
  }

  public interface IViewModel : IInitializable,IDisposable, INotifyPropertyChanged
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
   

    #region Methods

    public virtual void Initialize()
    {
      
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

    public TModel Model { get; set; }

    #endregion Properties
  }
}