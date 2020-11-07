﻿using Ninject;
using System;
using System.ComponentModel;
using VCore.Standard.Common;

namespace VCore.ViewModels
{
  public interface IParametrizedViewModel
  {
  }

  public interface IViewModel : IInitializable, IDisposable, INotifyPropertyChanged
  {
  }


  public abstract class ViewModel : VBindableBase, IViewModel
  {
    #region Methods

    public virtual void Initialize()
    {
    }

    #endregion 
  }

  public abstract class ViewModel<TModel> : ViewModel, IParametrizedViewModel
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