using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VCore.Controls;
using VCore.WPF.Managers;

namespace VCore.WPF.Controls
{
  public class NumericUpAndDown : Control
  {
    public double StepSize { get; set; } = 1;

    #region Value

    public double? Value
    {
      get { return (double?)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty =
      DependencyProperty.Register(
        nameof(Value),
        typeof(double?),
        typeof(NumericUpAndDown),
        new PropertyMetadata(null, (x, y) =>
        {
          if (x is NumericUpAndDown numeric)
          {
            numeric?.upCommand?.RaiseCanExecuteChanged();
            numeric?.downCommand?.RaiseCanExecuteChanged();
          }
        }));

    #endregion

    #region UpCommand

    private ActionCommand upCommand;

    public ICommand UpCommand
    {
      get
      {
        if (upCommand == null)
        {
          upCommand = new ActionCommand(OnUpCommand, () => { return Value != null; });
        }

        return upCommand;
      }
    }

    private void OnUpCommand()
    {
      if (Value != null)
      {
        Value += StepSize;
      }
    }

    #endregion

    #region DownCommand

    private ActionCommand downCommand;

    public ICommand DownCommand
    {
      get
      {
        if (downCommand == null)
        {
          downCommand = new ActionCommand(OnDownCommand, () => { return Value != null; });
        }

        return downCommand;
      }
    }

    private void OnDownCommand()
    {
      if (Value != null)
      {
        Value -= StepSize;
      }
    }

    #endregion
  }
}
