using System;
using System.Windows.Input;
using VCore.Annotations;

namespace VCore
{
  public class ActionCommand<TArgument> : ICommand
  {
    private readonly Action<TArgument> _action;
    private readonly TArgument baseArgument;

    public ActionCommand(Action<TArgument> action, TArgument baseArgument)
    {
      _action = action;
      this.baseArgument = baseArgument;
    }

    public ActionCommand(Action<TArgument> action) 
    {
      _action = action;
    }

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public void Execute(object parameter)
    {
      if (parameter is TArgument argument)
      {
        _action(argument);
      }
      else
      {
        _action(baseArgument);
      }
    }

    public event EventHandler CanExecuteChanged;
  }


  public class ActionCommand : ICommand
  {
    private readonly Action _action;
    private readonly Func<bool> canExecute;

    public ActionCommand(Action action)
    {
      _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    public ActionCommand(Action action, [NotNull] Func<bool> canExecute)
    {
      _action = action ?? throw new ArgumentNullException(nameof(action));
      this.canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
    }

    public virtual bool CanExecute(object parameter)
    {
      if (canExecute == null)
      {
        return true;
      }

      return canExecute.Invoke();
    }

    public void RaiseCanExecuteChanged()
    {
      CanExecuteChanged?.Invoke(this, new EventArgs());
    }

    public void Execute(object parameter)
    {
      _action();
    }

    public event EventHandler CanExecuteChanged;
  }
}
