using VCore.Standard.ViewModels.TreeView;

namespace VCore.Standard
{
  public abstract class SelectableViewModel<TModel> : ViewModel<TModel>, ISelectable
  {
    protected SelectableViewModel(TModel model) : base(model)
    {
    }

    #region IsSelected

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

    #endregion

    #region IsEnabled

    private bool isEnabled = true;

    public bool IsEnabled
    {
      get
      {
        return isEnabled;
      }
      set
      {
        if (value != isEnabled)
        {
          isEnabled = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    protected virtual void OnSelectionChanged(bool newValue)
    {

    }
  }
}