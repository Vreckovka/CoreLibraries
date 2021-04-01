using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using VCore.Standard;
using VCore.Standard.Modularity.Interfaces;
using VCore.ViewModels;
using VCore.WPF.Behaviors;

namespace VCore.WPF.Managers
{
  public class WindowManager : IWindowManager
  {
    private Window overlayWindow;

    #region ShowYesNoPrompt

    public MessageBoxResult ShowYesNoPrompt(string text, string header = "")
    {
      return MessageBox.Show(header, text, MessageBoxButton.YesNo, MessageBoxImage.Warning);
    }

    #endregion

    #region ShowPrompt

    public MessageBoxResult ShowPrompt(string text, string header = "")
    {
      return MessageBox.Show(text,header, MessageBoxButton.OK);
    }

    #endregion

    #region ShowPrompt

    public void ShowPrompt<TView>(ViewModel viewModel) where TView : IView, new()
    {
      var window = new Window();

      //BehaviorCollection Behaviors = Interaction.GetBehaviors(window);

      //Behaviors.Add(new ProperMaximizeWindowBehavior());

      window.Content = new TView();

      window.DataContext = viewModel;

      if(viewModel is BaseWindowViewModel baseWindowViewModel)
      {
        baseWindowViewModel.Window = window;
      }

      window.Owner = Application.Current.MainWindow;

      window.SizeToContent = SizeToContent.WidthAndHeight;
      window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      window.WindowStyle = WindowStyle.None;
      window.AllowsTransparency = true;
      window.Topmost = true;
      window.ResizeMode = ResizeMode.NoResize;
      window.ShowInTaskbar = false;

      window.Loaded += Window_Loaded;
      window.Closed += Window_Closed;

      
      window.ShowDialog();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      if (sender is Window window)
      {
        window.Loaded -= Window_Loaded;
        window.Closed -= Window_Closed;

        overlayWindow?.Close();
      }
    }

    #region Window_Loaded

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      if (sender is Window window)
      {
        if (IsModal(window))
        {
          ShowOverlayWindow();
        }
      }
    }

    #endregion

    #region ShowOverlayWindow

    private void ShowOverlayWindow()
    {
      overlayWindow = new Window();

      var mainWn = Application.Current.MainWindow;

      if (mainWn != null)
      {
        overlayWindow.Owner = mainWn;

        overlayWindow.Background = Brushes.Black;
        overlayWindow.Opacity = 0.90;
        overlayWindow.Style = null;
        overlayWindow.WindowStyle = WindowStyle.None;
        overlayWindow.ResizeMode = ResizeMode.NoResize;
        overlayWindow.AllowsTransparency = true;
        overlayWindow.ShowInTaskbar = false;

        overlayWindow.Width = mainWn.Width;
        overlayWindow.Height = mainWn.Height;
        overlayWindow.Top = mainWn.Top;
        overlayWindow.Left = mainWn.Left;

        overlayWindow.Show();
      }
    }

    #endregion

    #region IsModal

    public bool IsModal(Window window)
    {
      return (bool)typeof(Window).GetField("_showingAsDialog", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(window);
    }

    #endregion

    #endregion
  }
}
