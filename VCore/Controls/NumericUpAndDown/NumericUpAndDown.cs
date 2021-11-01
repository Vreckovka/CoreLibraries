using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VCore.WPF.Misc;

namespace VCore.WPF.Controls.NumericUpAndDown
{
  public class NumericUpAndDown : Control
  {
    #region StepSize

    public double StepSize
    {
      get { return (double)GetValue(StepSizeProperty); }
      set { SetValue(StepSizeProperty, value); }
    }

    public static readonly DependencyProperty StepSizeProperty =
      DependencyProperty.Register(
        nameof(StepSize),
        typeof(double),
        typeof(NumericUpAndDown),
        new PropertyMetadata(1.0, (x, y) =>
        {
          if (x is NumericUpAndDown numeric)
          {
            numeric?.upCommand?.RaiseCanExecuteChanged();
            numeric?.downCommand?.RaiseCanExecuteChanged();
          }
        }));

    #endregion

    #region MaxValue

    public double? MaxValue
    {
      get { return (double?)GetValue(MaxValueProperty); }
      set { SetValue(MaxValueProperty, value); }
    }

    public static readonly DependencyProperty MaxValueProperty =
      DependencyProperty.Register(
        nameof(MaxValue),
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

    #region MinValue

    public double? MinValue
    {
      get { return (double?)GetValue(MinValueProperty); }
      set { SetValue(MinValueProperty, value); }
    }

    public static readonly DependencyProperty MinValueProperty =
      DependencyProperty.Register(
        nameof(MinValue),
        typeof(double?),
        typeof(NumericUpAndDown),
        new PropertyMetadata(null, (x, y) =>
        {
          if (x is NumericUpAndDown numeric)
          {
            if (numeric.Value == null || numeric.Value < numeric.MinValue)
            {
              numeric.Value = numeric.MinValue;
            }

            numeric?.upCommand?.RaiseCanExecuteChanged();
            numeric?.downCommand?.RaiseCanExecuteChanged();
          }
        }));

    #endregion

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

            if (numeric.Value < numeric.MinValue && numeric.MinValue != null)
            {
              numeric.Value = numeric.MinValue;
            }
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
          upCommand = new ActionCommand(OnUpCommand, () => { return Value != null && (MaxValue >= Value + StepSize || MaxValue == null); });
        }

        return upCommand;
      }
    }

    private void OnUpCommand()
    {
      Value += StepSize;
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
          downCommand = new ActionCommand(OnDownCommand, () => { return Value != null && (MinValue <= Value - StepSize || MinValue == null); });
        }

        return downCommand;
      }
    }

    private void OnDownCommand()
    {
      Value -= StepSize;
    }

    #endregion
  }
}
