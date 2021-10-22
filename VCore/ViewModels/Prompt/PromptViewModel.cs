using System;
using System.Windows;
using System.Windows.Input;
using VCore.WPF.Misc;

namespace VCore.WPF.ViewModels.Prompt
{
  public class PromptViewModel : BaseWindowViewModel
  {
    protected Func<bool> CanExecuteOkCommand { get; set; } = () => { return false; };

    public PromptResult PromptResult { get; set; }

    #region OkCommand

    protected ActionCommand okCommand;

    public ICommand OkCommand
    {
      get
      {
        return okCommand ??= new ActionCommand(OnOkCommand, CanExecuteOkCommand);
      }
    }

    protected virtual void OnOkCommand()
    {
      PromptResult = PromptResult.Ok;
      Window?.Close();
    }

    #endregion

    #region CancelCommand

    private ActionCommand cancelCommand;

    public ICommand CancelCommand
    {
      get
      {
        return cancelCommand ??= new ActionCommand(OnCancelCommand);
      }
    }

    protected virtual void OnCancelCommand()
    {
      PromptResult = PromptResult.Cancel;
      Window?.Close();
    }

    #endregion

    public Visibility CancelVisibility { get; set; } = Visibility.Collapsed;
    public Visibility OkVisibility { get; set; } = Visibility.Visible;
  }
}