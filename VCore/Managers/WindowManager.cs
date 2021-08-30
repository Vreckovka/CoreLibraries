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

    #region ShowPrompt

    public void ShowPrompt<TView>(ViewModel viewModel) where TView : IView, new()
    {
      var window = GetWindowView<TView>(viewModel);

      window.Loaded += Window_Loaded;
      window.Closed += Window_Closed;

      ShowOverlayWindow();

      window.Owner = overlayWindow;

      window.ShowDialog();
    }

    #endregion

    #region ShowQuestionPrompt

    public PromptResult ShowQuestionPrompt<TView, TViewModel>(TViewModel viewModel)
      where TView : IView, new()
      where TViewModel : GenericPromptViewModel
    {
      ShowPrompt<TView>(viewModel);

      return viewModel.PromptResult;
    }

    #endregion

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
      }

      Application.Current?.MainWindow?.Focus();
      overlayWindow?.Close();
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
      overlayWindow?.Close();

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

        CopyWindowSizeAndPosition(overlayWindow, mainWn);

        mainWn.LocationChanged += MainWn_Changed;
        mainWn.SizeChanged += MainWn_Changed;
        mainWn.StateChanged += MainWn_Changed;
        overlayWindow.Closed += OverlayWindow_Closed;

        overlayWindow.Show();
      }
    }

    private void OverlayWindow_Closed(object sender, EventArgs e)
    {
      var mainWn = Application.Current.MainWindow;

      if (mainWn != null)
      {
        mainWn.LocationChanged -= MainWn_Changed;
        mainWn.SizeChanged -= MainWn_Changed;
        mainWn.StateChanged -= MainWn_Changed;

        if (overlayWindow != null)
          overlayWindow.Closed -= OverlayWindow_Closed;
      }
    }

    #endregion

    #region MainWn_Changed

    private void MainWn_Changed(object sender, EventArgs e)
    {
      CopyWindowSizeAndPosition(overlayWindow, Application.Current.MainWindow);
    }

    #endregion

    #region CopyWindowSizeAndPosition

    private void CopyWindowSizeAndPosition(Window myWindow, Window copyFrom)
    {
      if (myWindow == null || copyFrom == null)
      {
        return;
      }

      overlayWindow.Width = copyFrom.ActualWidth;
      overlayWindow.Height = copyFrom.ActualHeight;
      overlayWindow.Top = copyFrom.Top;
      overlayWindow.Left = copyFrom.Left;
      overlayWindow.WindowState = copyFrom.WindowState;
    }

    #endregion

    #region GetWindowView

    public Window GetWindowView<TView>(object dataContext, bool setOwner = true) where TView : IView, new()
    {
      var window = new Window();

      window.Content = new TView();

      window.DataContext = dataContext;

      if (dataContext is BaseWindowViewModel baseWindowViewModel)
      {
        baseWindowViewModel.Window = window;
      }

      if (setOwner)
      {
        window.Owner = Application.Current.MainWindow;
      }


      window.SizeToContent = SizeToContent.WidthAndHeight;
      window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      window.WindowStyle = WindowStyle.None;
      window.AllowsTransparency = true;
      window.ResizeMode = ResizeMode.NoResize;
      window.ShowInTaskbar = false;

      return window;
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
