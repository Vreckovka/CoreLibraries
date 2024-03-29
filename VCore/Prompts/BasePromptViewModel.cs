﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using VCore.Standard.Modularity.Interfaces;
using VCore.WPF.ViewModels.Prompt;

namespace VCore.WPF.Prompts
{
  public class BasePromptViewModel<TModel> : BasePromptViewModel
  {
    #region Constructors

    public BasePromptViewModel(TModel model)
    {
      Model = model;
    }

    #endregion Constructors

    #region Properties

    public TModel Model { get; set; }

    #endregion Properties
  }


  public class BasePromptViewModel : PromptViewModel
  {
    public BasePromptViewModel()
    {
      CanExecuteOkCommand = () => { return true; };
      CancelVisibility = Visibility.Visible;
    }

    public string Text { get; set; }

    public string ItemName { get; set; }

    public string AfterText { get; set; }
  }
}
