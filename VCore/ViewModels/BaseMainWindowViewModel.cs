using System.Windows;
using System.Windows.Input;
using Ninject;
using VCore.Standard;
using VCore.Standard.Helpers;

namespace VCore.ViewModels
{
  public class BaseMainWindowViewModel : ViewModel
  {
    public BaseMainWindowViewModel()
    {
    }

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

    #endregion

    #region Commands

    #region CloseCommand

    private ActionCommand<Window> closeCommand;

    public ICommand CloseCommand
    {
      get
      {
        return closeCommand ??= new ActionCommand<Window>((o) => o.Close()).DisposeWith(this);
      }
    }

    #endregion

    #region MaximizeCommand

    private ActionCommand<Window> maximizeCommand;

    public ICommand MaximizeCommand
    {
      get
      {
        return maximizeCommand ??= new ActionCommand<Window>(o => o.WindowState = WindowState.Maximized).DisposeWith(this);
      }
    }

    #endregion

    #region NormalizeCommand

    private ActionCommand<Window> normalizeCommand;

    public ICommand NormalizeCommand
    {
      get
      {

        return normalizeCommand ??= new ActionCommand<Window>((o) => o.WindowState = WindowState.Normal).DisposeWith(this);
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

  }
}
