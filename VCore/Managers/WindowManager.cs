using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using VCore.Standard;
using VCore.Standard.Modularity.Interfaces;
using VCore.ViewModels;
using VCore.WPF.Behaviors;
using VCore.WPF.Prompts;
using VCore.WPF.ViewModels.Prompt;

namespace VCore.WPF.Managers
{
  public class WindowManager : IWindowManager
  {
    private Window overlayWindow;

    #region ShowYesNoPrompt

    public PromptResult ShowYesNoPrompt(string text, string header = "")
    {
      var vm = new GenericPromptViewModel()
      {
        Title = header,
        Text = text
      };

      ShowPrompt<GenericPromptView>(vm);


      return vm.PromptResult;
    }

    #endregion

    #region ShowDeletePrompt

    public PromptResult ShowDeletePrompt(string itemName)
    {
      return ShowDeletePrompt(itemName, "Do you really want to delete item ", " ?", "Delete");
    }

    public PromptResult ShowDeletePrompt(
      string itemName,
      string header,
      string beforeText,
      string afterText)
    {
      var vm = new DeleteItemPromptViewModel()
      {
        Text = beforeText,
        Title = header,
        ItemName = itemName,
        AfterText = afterText
      };

      ShowPrompt<DeletePromptView>(vm);

      return vm.PromptResult;
    }

    #endregion


    #region ShowPrompt

    public MessageBoxResult ShowPrompt(string text, string header = "")
    {
      return MessageBox.Show(text, header, MessageBoxButton.OK);
    }

    #endregion

    #region ShowPrompt

    public void ShowPrompt<TView>(ViewModel viewModel) where TView : IView, new()
    {
      var window = new Window();

      window.Content = new TView();

      window.DataContext = viewModel;

      if (viewModel is BaseWindowViewModel baseWindowViewModel)
      {
        baseWindowViewModel.Window = window;
      }

      window.Owner = Application.Current.MainWindow;

      window.SizeToContent = SizeToContent.WidthAndHeight;
      window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      window.WindowStyle = WindowStyle.None;
      window.AllowsTransparency = true;
      window.ResizeMode = ResizeMode.NoResize;
      window.ShowInTaskbar = false;

      window.Loaded += Window_Loaded;
      window.Closed += Window_Closed;


      ShowOverlayWindow();

      window.Owner = overlayWindow;

      window.ShowDialog();
    }



    #endregion

    #region ShowErrorPrompt

    public void ShowErrorPrompt(Exception ex)
    {
      var vm = new GenericPromptViewModel()
      {
        Title = "Error occured",
        Text = ex.ToString()
      };

      ShowPrompt<ErrorPromptView>(vm);
    }

    #endregion

    #region Window_Closed

    private void Window_Closed(object sender, EventArgs e)
    {
      if (sender is Window window)
      {
        window.Loaded -= Window_Loaded;
        window.Closed -= Window_Closed;

        Application.Current?.MainWindow?.Focus();

        overlayWindow?.Close();
      }
    }

    #endregion

    #region Window_Loaded

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      if (sender is Window window)
      {
        if (IsModal(window) && overlayWindow == null)
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

        overlayWindow.Width = mainWn.ActualWidth;
        overlayWindow.Height = mainWn.ActualHeight;
        overlayWindow.Top = mainWn.Top;
        overlayWindow.Left = mainWn.Left;
        overlayWindow.WindowState = mainWn.WindowState;

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


  }
}
