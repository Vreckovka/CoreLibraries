using System.Windows;
using System.Windows.Input;
using VCore.Standard;
using VCore.Standard.Helpers;

namespace VCore.ViewModels
{
  public class BaseWindowViewModel : ViewModel
  {
    #region Properties

    #region WindowChromeVisiblity

    private Visibility windowChromeVisiblity;

    public Visibility WindowChromeVisiblity
    {
      get { return windowChromeVisiblity; }
      set
      {
        if (value != windowChromeVisiblity)
        {
          windowChromeVisiblity = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    public virtual string Title { get; set; } = "BASE WPF APP";

    #region TopMost

    private bool topMost;

    public bool TopMost
    {
      get { return topMost; }
      set
      {
        if (value != topMost)
        {
          topMost = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    public Window Window { get; set; }

    #region WindowState

    private WindowState windowState;

    public WindowState WindowState
    {
      get { return windowState; }
      set
      {
        if (value != windowState)
        {
          windowState = value;
          OnWindowStateChanged(windowState);
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #endregion

    #region Commands

    #region CloseCommand

    private ActionCommand<Window> closeCommand;

    public ICommand CloseCommand
    {
      get
      {
        return closeCommand ??= new ActionCommand<Window>(OnClose).DisposeWith(this);
      }
    }

    protected virtual void OnClose(Window window)
    {
      window.Close();
    }

    #endregion

    #region MaximizeCommand

    private ActionCommand<Window> maximizeCommand;

    public ICommand MaximizeCommand
    {
      get
      {
        return maximizeCommand ??= new ActionCommand<Window>(o =>
        {
          o.ResizeMode = ResizeMode.NoResize;
          o.WindowState = WindowState.Maximized;

        }).DisposeWith(this);
      }
    }

    #endregion

    #region NormalizeCommand

    private ActionCommand<Window> normalizeCommand;

    public ICommand NormalizeCommand
    {
      get
      {

        return normalizeCommand ??= new ActionCommand<Window>((o) =>
        {
          o.ResizeMode = ResizeMode.CanResize;
          o.WindowState = WindowState.Normal;
        }).DisposeWith(this);
      }
    }

    #endregion

    #region MinimizeCommand

    private ActionCommand<Window> minimizeCommand;

    public ICommand MinimizeCommand
    {
      get
      {

        return minimizeCommand ??= new ActionCommand<Window>((o) => o.WindowState = WindowState.Minimized).DisposeWith(this);
      }
    }

    #endregion

    #endregion

    protected virtual void OnWindowStateChanged(WindowState windowState)
    {

    }
  }
}