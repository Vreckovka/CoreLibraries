using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace VCore.WPF.Behaviors.Buttons
{
  public class ClickCountBehavior : Behavior<FrameworkElement>
  {
    #region Command

    public static readonly DependencyProperty CommandProperty =
      DependencyProperty.Register(
        nameof(Command),
        typeof(ICommand),
        typeof(ClickCountBehavior),
        new PropertyMetadata(null));

    public ICommand Command
    {
      get { return (ICommand)GetValue(CommandProperty); }
      set { SetValue(CommandProperty, value); }
    }

    #endregion

    public int ClickCount { get; set; } = 2;

    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
    }

    private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount >= ClickCount)
      {
        Command?.Execute(null);
      }
    }

    protected override void OnDetaching()
    {
      AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;
    }
  }
}