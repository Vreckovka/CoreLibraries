﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VCore.WPF.Managers
{
  public static class VFocusManager
  {
    public static HashSet<FrameworkElement> FocusedItems { get; set; } = new HashSet<FrameworkElement>();


    public static bool IsAnyFocused()
    {
       return FocusedItems.Any();
    }

    public static void AddToFocusItems(FrameworkElement control)
    {
      FocusedItems.Add(control);
    }

    public static void RemoveFromFocusItems(FrameworkElement control)
    {
      FocusedItems.Remove(control);
    }

    public static void SetFocus(DependencyObject dependencyObject)
    {
      IInputElement focusedControl = Keyboard.FocusedElement;

      if (focusedControl != null)
      {
        FocusManager.SetFocusedElement(dependencyObject, (IInputElement)dependencyObject);

        focusedControl.ReleaseMouseCapture();

        Keyboard.ClearFocus();
      }
    }
  }
}
