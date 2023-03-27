using System;
using System.Windows.Input;
using VCore.Standard.Common;

namespace VCore.WPF.Misc
{
  public class ActionCommand<TArgument> : VDisposableObject, ICommand
  {
    private readonly Action<TArgument> _action;
    private readonly TArgument baseArgument;
    private readonly Func<TArgument, bool> canExecute;
    public ActionCommand(Action<TArgument> action, TArgument baseArgument)
    {
      _action = action;
      this.baseArgument = baseArgument;
    }

    public ActionCommand(Action<TArgument> action, Func<TArgument, bool> canExecute)
    {
      _action = action ?? throw new ArgumentNullException(nameof(action));

      this.canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
    }

    public ActionCommand(Action<TArgument> action)
    {
      _action = action;
    }

    public bool CanExecute(object parameter)
    {
      if (canExecute != null)
      {
        if (parameter is TArgument argument)
        {
          return canExecute(argument);
        }

        return false;
      }

      return true;
    }

    public void RaiseCanExecuteChanged()
    {
      CanExecuteChanged?.Invoke(this, new EventArgs());
    }

    public void Execute(object parameter)
    {
      if (parameter is string stringParameter && bool.TryParse(stringParameter, out var boolValue))
      {
        parameter = boolValue;
      }

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


  public class ActionCommand : ICommand, IDisposable
  {
    private readonly Action _action;
    private readonly Func<bool> canExecute;

    public ActionCommand(Action action)
    {
      _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    public ActionCommand(Action action, Func<bool> canExecute)
    {
      _action = action ?? throw new ArgumentNullException(nameof(action));
      this.canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
    }

    public virtual bool CanExecute(object parameter = null)
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

    public void Dispose()
    {
    }
  }
}
