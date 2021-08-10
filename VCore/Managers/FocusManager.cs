using System;
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
    public static HashSet<Control> FocusedItems { get; set; } = new HashSet<Control>();


    public static bool IsAnyFocused()
    {
       return FocusedItems.Any();
    }

    public static void AddToFocusItems(Control control)
    {
      FocusedItems.Add(control);
    }

    public static void RemoveFromFocusItems(Control control)
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
