using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VCore.Standard.ViewModels.TreeView
{

  public class TreeViewItemViewModel<TModel> : TreeViewItemViewModel where TModel : class
  {
    public TreeViewItemViewModel(TModel model)
    {
      Model = model ?? throw new ArgumentNullException(nameof(model));
    }

    #region Model

    private TModel model;

    public TModel Model
    {
      get { return model; }
      set
      {
        if (value != model)
        {
          model = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

  }

  public class TreeViewItemViewModel : ViewModel
  {
   
    #region IsExpanded

    private bool isExpanded;

    public bool IsExpanded
    {
      get { return isExpanded; }
      set
      {
        if (value != isExpanded)
        {
          isExpanded = value;

          if (isExpanded)
            OnExpanded(isExpanded);

          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region CanExpand

    private bool canExpand;

    public bool CanExpand
    {
      get { return canExpand; }
      set
      {
        if (value != canExpand)
        {
          canExpand = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region SubItems

    private List<TreeViewItemViewModel> subItems = new List<TreeViewItemViewModel>();

    public List<TreeViewItemViewModel> SubItems
    {
      get { return subItems; }
      set
      {
        if (value != subItems)
        {
          subItems = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region Name

    private string name;

    public string Name
    {
      get { return name; }
      set
      {
        if (value != name)
        {
          name = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion


    #region HighlitedText

    private string highlitedText;

    public string HighlitedText
    {
      get { return highlitedText; }
      set
      {
        if (value != highlitedText)
        {
          highlitedText = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    protected virtual void OnExpanded(bool isExpandend)
    {
    }
  }
}