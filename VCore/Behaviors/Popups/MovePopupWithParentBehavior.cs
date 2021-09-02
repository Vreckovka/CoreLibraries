﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using Microsoft.Xaml.Behaviors;

namespace VCore.WPF.Behaviors.Popups
{
  public class MovePopupWithParentBehavior : Behavior<Popup>
  {
    private Window window;
    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.LayoutUpdated += AssociatedObject_LayoutUpdated; 
     
    }

    private void AssociatedObject_LayoutUpdated(object sender, EventArgs e)
    {
      if (window == null)
      {
        if (AssociatedObject.PlacementTarget != null)
        {
          window = Window.GetWindow(AssociatedObject.PlacementTarget);

        }
        else
        {
          window = Window.GetWindow(AssociatedObject);
        }

        if (window != null)
        {
          window.LocationChanged += Window_LocationChanged;
        }
      }
    }

    private void Window_LocationChanged(object sender, EventArgs e)
    {
      var offset = AssociatedObject.HorizontalOffset;
      AssociatedObject.HorizontalOffset = offset + 1;
      AssociatedObject.HorizontalOffset = offset;
    }


    protected override void OnDetaching()
    {
      AssociatedObject.LayoutUpdated -= AssociatedObject_LayoutUpdated;
      window.LocationChanged -= Window_LocationChanged;
    }
  }
}
