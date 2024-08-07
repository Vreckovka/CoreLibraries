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
using VCore.WPF.Behaviors;
using VCore.WPF.Interfaces.Managers;
using VCore.WPF.Prompts;
using VCore.WPF.ViewModels;
using VCore.WPF.ViewModels.Prompt;

namespace VCore.WPF.Managers
{
  public class WindowManager : IWindowManager
  {
    private Window overlayWindow;
    private object windowLock = new object();
    private List<Window> windows = new List<Window>();

    #region ShowPrompt

    public void ShowPrompt<TView>(ViewModel viewModel, double width = 0, double height = 0, bool showOverlay = true) where TView : IView, new()
    {
      lock (windowLock)
      {
        var window = GetWindowView<TView>(viewModel, showOverlay);

        window.Loaded += (x,y) => Window_Loaded(x,y, showOverlay);
        window.Closed += Window_Closed;

        if (width > 0 || height > 0)
        {
          window.Width = width;
          window.Height = height;
          window.SizeToContent = SizeToContent.Manual;
        }

        if (overlayWindow == null && showOverlay)
        {
          ShowOverlayWindow();

          window.Owner = overlayWindow;
        }

        window.ShowInTaskbar = true;
        window.Title = "Custom prompt";
        viewModel?.Initialize();

        if (showOverlay)
          window.ShowDialog();
        else
          window.Show();
      }
    }

    #endregion

    #region ShowQuestionPrompt

    public PromptResult ShowQuestionPrompt<TView, TViewModel>(TViewModel viewModel)
      where TView : IView, new()
      where TViewModel : BasePromptViewModel
    {
      ShowPrompt<TView>(viewModel);

      return viewModel.PromptResult;
    }

    #endregion

    #region OkCancel

    public PromptResult OkCancel(string text, string header = "", Visibility cancelVisibility = Visibility.Collapsed)
    {
      var vm = new BasePromptViewModel()
      {
        Title = header,
        Text = text,
        CancelVisibility = cancelVisibility,
      };

      ShowPrompt<GenericPromptView>(vm);


      return vm.PromptResult;
    }

    #endregion

    #region ShowDeletePrompt

    public PromptResult ShowDeletePrompt(
      string itemName,
      string header = "Delete",
      string beforeText = "Do you really want to delete item ",
      string afterText = " ?")
    {
      return ShowQuestionPrompt<DeletePromptView>(itemName, header, beforeText, afterText);
    }

    #endregion

    public PromptResult ShowQuestionPrompt<TView>(
      string itemName = "",
      string header = "",
      string beforeText = "",
      string afterText = "?") where TView : IView, new()
    {
      var vm = new BasePromptViewModel()
      {
        Text = beforeText,
        Title = header,
        ItemName = itemName,
        AfterText = afterText
      };

      ShowPrompt<TView>(vm);

      return vm.PromptResult;
    }

    public PromptResult ShowQuestionPrompt(
      string beforeText,
      string header = "",
      string itemName = "",
      string afterText = "?")
    {
      var vm = new BasePromptViewModel()
      {
        Text = beforeText,
        Title = header,
        ItemName = itemName,
        AfterText = afterText
      };

      ShowPrompt<GenericPromptView>(vm);

      return vm.PromptResult;
    }

    #region ShowErrorPrompt

    public void ShowErrorPrompt(Exception ex)
    {
      var vm = new BasePromptViewModel()
      {
        Title = "Error occured",
        Text = ex.ToString()
      };

      ShowPrompt<ErrorPromptView>(vm);
    }

    public void ShowErrorPrompt(string message)
    {
      var vm = new BasePromptViewModel()
      {
        Title = "Error occured",
        Text = message
      };

      ShowPrompt<ErrorPromptView>(vm);
    }

    #endregion

    #region Window_Closed

    private void Window_Closed(object sender, EventArgs e)
    {
      lock (windowLock)
      {
        if (sender is Window window)
        {
          //window.Loaded -= Window_Loaded;
          window.Closed -= Window_Closed;

          windows.Remove(window);
        }

        if (windows.Count == 0)
        {
          Application.Current?.MainWindow?.Focus();
          overlayWindow?.Close();
          overlayWindow = null;
          FullScreenManager.IsMouseBlocked = false;
        }
      }
    }

    #endregion

    #region Window_Loaded

    private void Window_Loaded(object sender, RoutedEventArgs e, bool showOverlay)
    {
      lock (windowLock)
      {
        if (sender is Window window && showOverlay)
        {
          windows.Add(window);
          FullScreenManager.IsMouseBlocked = true;
        }
      }
    }

    #endregion

    #region ShowOverlayWindow

    protected virtual void ShowOverlayWindow()
    {
      lock (windowLock)
      {
        if (overlayWindow != null)
        {
          return;
        }

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
    }




    #endregion

    #region OverlayWindow_Closed

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

      myWindow.Width = copyFrom.ActualWidth;
      myWindow.Height = copyFrom.ActualHeight;
      myWindow.Top = copyFrom.Top;
      myWindow.Left = copyFrom.Left;
      myWindow.WindowState = copyFrom.WindowState;
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
