using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using VCore.Helpers;
using VCore.WPF.Managers;

namespace VCore.WPF.Behaviors.Buttons
{
    public class CloseContextMenuOnClickBehavior : Behavior<ButtonBase>
    {
    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.Click += AssociatedObject_Click;
    }

    private void AssociatedObject_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      var item = AssociatedObject.GetFirstParentOfType<ContextMenu>();

      if (item != null)
      {
        item.IsOpen = false;
      }
    }
  }
}
