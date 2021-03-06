﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
