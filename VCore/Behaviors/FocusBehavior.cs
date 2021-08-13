using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using VCore.WPF.Managers;

namespace VCore.WPF.Behaviors
{
  public class FocusBehavior : Behavior<FrameworkElement>
  {
    #region OnFocusCommand

    public ICommand OnFocusCommand
    {
      get { return (ICommand)GetValue(OnFocusCommandProperty); }
      set { SetValue(OnFocusCommandProperty, value); }
    }

    public static readonly DependencyProperty OnFocusCommandProperty =
      DependencyProperty.Register(
        nameof(OnFocusCommand),
        typeof(ICommand),
        typeof(FocusBehavior),
        new PropertyMetadata(null));


    #endregion

    #region OnLostFocusCommand

    public ICommand OnLostFocusCommand
    {
      get { return (ICommand)GetValue(OnLostFocusCommandProperty); }
      set { SetValue(OnLostFocusCommandProperty, value); }
    }

    public static readonly DependencyProperty OnLostFocusCommandProperty =
      DependencyProperty.Register(
        nameof(OnLostFocusCommand),
        typeof(ICommand),
        typeof(FocusBehavior),
        new PropertyMetadata(null));


    #endregion

    public bool SetFocusManager { get; set; }

    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.GotFocus += AssociatedObject_GotFocus;
      AssociatedObject.LostFocus += AssociatedObject_LostFocus; ;
    }

    private void AssociatedObject_LostFocus(object sender, RoutedEventArgs e)
    {
      if (SetFocusManager)
      {
        VFocusManager.RemoveFromFocusItems(AssociatedObject);
      }

      OnLostFocusCommand?.Execute(null);
    }

    private void AssociatedObject_GotFocus(object sender, RoutedEventArgs e)
    {
      if (SetFocusManager)
      {
        VFocusManager.AddToFocusItems(AssociatedObject);
      }

      OnFocusCommand?.Execute(null);
    }
  }
}