﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VCore.WPF.Managers;

namespace VCore.Controls
{
  public class Finder : Control
  {
    #region KeyDownCommand

    public ICommand KeyDownCommand
    {
      get { return (ICommand)GetValue(KeyDownCommandProperty); }
      set { SetValue(KeyDownCommandProperty, value); }
    }

    public static readonly DependencyProperty KeyDownCommandProperty =
        DependencyProperty.Register(
            nameof(KeyDownCommand),
            typeof(ICommand),
            typeof(Finder),
            new PropertyMetadata(null));


    #endregion

    #region FocusChanged

    private ActionCommand<string> focusChanged;

    public ICommand FocusChanged
    {
      get
      {
        if (focusChanged == null)
        {
          focusChanged = new ActionCommand<string>(OnFocusChanged);
        }

        return focusChanged;
      }
    }

    private void OnFocusChanged(string isFocused)
    {
      var isFocusBool = bool.Parse(isFocused);

      if (isFocusBool)
      {
        VFocusManager.AddToFocusItems(this);

        CaptureMouse();

       
      }
      else
      {
        VFocusManager.RemoveFromFocusItems(this);

        ReleaseMouseCapture();
      }
    }

    #endregion



    #region Text

    public string Text
    {
      get { return (string)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
      DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(Finder),
        new PropertyMetadata(null));


    #endregion
  }
}
