using System.Windows;

namespace VCore.WPF.Prompts
{
  public class LoginPromptViewModel : GenericPromptViewModel
  {
    public LoginPromptViewModel()
    {
      CanExecuteOkCommand = () => { return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Password); };

      CancelVisibility = Visibility.Visible;
    }


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
          okCommand.RaiseCanExecuteChanged();
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region Password

    private string password;

    public string Password
    {
      get { return password; }
      set
      {
        if (value != password)
        {
          password = value;
          okCommand.RaiseCanExecuteChanged();
          RaisePropertyChanged();
        }
      }
    }

    #endregion

  }
}