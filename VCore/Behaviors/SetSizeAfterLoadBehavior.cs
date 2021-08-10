using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace VCore.WPF.Behaviors
{
  public class SetSizeAfterLoadBehavior : Behavior<FrameworkElement>
  {
    public double ExtraWidth { get; set; }
    public double ExtraHeight { get; set; }
    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.Loaded += AssociatedObject_Loaded;
    }

    private void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
      AssociatedObject.Width = AssociatedObject.ActualWidth + ExtraWidth;
      AssociatedObject.Height = AssociatedObject.ActualHeight + ExtraHeight;
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();

      AssociatedObject.Loaded -= AssociatedObject_Loaded;
    }
  }
}
