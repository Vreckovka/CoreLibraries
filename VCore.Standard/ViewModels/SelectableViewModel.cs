namespace VCore.Standard
{
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