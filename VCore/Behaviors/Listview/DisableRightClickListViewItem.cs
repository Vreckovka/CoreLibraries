using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace VCore.WPF.Behaviors.Listview
{
  public class DisableRightClickSelection : Behavior<ListView>
  {
    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.PreviewMouseRightButtonDown += AssociatedObject_PreviewMouseRightButtonDown;
    }

    private void AssociatedObject_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      e.Handled = true;
    }

    protected override void OnDetaching()
    {
      AssociatedObject.PreviewMouseRightButtonDown -= AssociatedObject_PreviewMouseRightButtonDown;
    }
  }
}
