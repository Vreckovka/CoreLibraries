namespace VCore.Standard.ViewModels
{
  public class ListViewItemViewModel : ViewModel
  {
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
  }

  public class ListViewItemViewModel<TModel> : ViewModel<TModel>
  {
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
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region IsEnabled

    private bool isEnabled = true;

    public bool IsEnabled
    {
      get { return isEnabled; }
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

    public ListViewItemViewModel(TModel model) : base(model)
    {
    }
  }
}