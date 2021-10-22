using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VCore.WPF.Controls
{
  public class VPasswordBox : Decorator
  {
    private Grid grid;
    private System.Windows.Controls.PasswordBox passwordBox;
    private TextBox textBox;

    public VPasswordBox()
    {
      grid = new Grid();
      passwordBox = new System.Windows.Controls.PasswordBox();
      textBox = new TextBox();
      passwordBox.TabIndex = 1;
      passwordBox.IsTabStop = true;

      passwordBox.PasswordChanged += PasswordBox_PasswordChanged;

      textBox.Visibility = Visibility.Hidden;
      textBox.IsTabStop = true;
      passwordBox.IsTabStop = true;


//      button.PreviewMouseLeftButtonDown += OnShowPassword;
//      button.PreviewMouseLeftButtonUp += HidePassword;

      grid.Children.Add(passwordBox);
      grid.Children.Add(textBox);
      //grid.Children.Add(button);
    }

    public override void BeginInit()
    {
      Child = grid;

      base.BeginInit();
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
      Password = ((System.Windows.Controls.PasswordBox)sender).Password;
    }

    private void HidePassword(object sender, MouseButtonEventArgs e)
    {
      textBox.Visibility = Visibility.Hidden;
      passwordBox.Visibility = Visibility.Visible;
    }

    private void OnShowPassword(object sender, RoutedEventArgs e)
    {
      textBox.Visibility = Visibility.Visible;
      passwordBox.Visibility = Visibility.Hidden;
    }

    #region Password

    public static readonly DependencyProperty PasswordProperty =
      DependencyProperty.Register(nameof(Password), typeof(string), typeof(VPasswordBox), new PropertyMetadata(null, OnPasswordChanged));

    private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      string password = e.NewValue as string;

      var passwordBox = ((VPasswordBox)d).passwordBox;
      var textBox = ((VPasswordBox)d).textBox;

      textBox.Text = password;

      if (password != passwordBox.Password)
      {
        passwordBox.Password = password;
      }
    }

    public string Password
    {
      get => (string)GetValue(PasswordProperty);
      set => SetValue(PasswordProperty, value);
    }

    #endregion
  }
}
