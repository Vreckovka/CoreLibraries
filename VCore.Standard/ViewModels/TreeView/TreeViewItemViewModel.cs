using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VCore.WPF.ItemsCollections;

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

  public class TreeViewItemViewModel : ViewModel, ISelectable
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

    private ItemsViewModel<TreeViewItemViewModel> subItems = new ItemsViewModel<TreeViewItemViewModel>();

    public ItemsViewModel<TreeViewItemViewModel> SubItems
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

    #region IsSelected

    private bool isSelected;

    public bool IsSelected
    {
      get { return isSelected; }
      set
      {
        if (value != isSelected)
        {
          isSelected = value;
          OnSelected(isSelected);
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    public void AddItem(TreeViewItemViewModel treeViewItemViewModel)
    {
      SubItems.Add(treeViewItemViewModel);
    }

    protected virtual void OnExpanded(bool isExpandend)
    {
    }

    protected virtual void OnSelected(bool isSelected)
    {
    }
  }
}