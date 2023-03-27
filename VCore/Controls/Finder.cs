using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VCore.WPF.Managers;
using VCore.WPF.Misc;
using VCore.WPF.ViewModels.Prompt;

namespace VCore.WPF.Controls
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

    #region ClearSearchCommand

    protected ActionCommand clearSearchCommand;

    public ICommand ClearSearchCommand
    {
      get
      {
        return clearSearchCommand ??= new ActionCommand(OnClearSearchCommand);
      }
    }

    protected virtual void OnClearSearchCommand()
    {
      Text = "";
    }

    #endregion

    #region FocusChanged

    private ActionCommand<object> focusChanged;

    public ICommand FocusChanged
    {
      get
      {
        if (focusChanged == null)
        {
          focusChanged = new ActionCommand<object>(OnFocusChanged);
        }

        return focusChanged;
      }
    }

    private void OnFocusChanged(object isFocused)
    {
      if (bool.TryParse(isFocused.ToString(), out var isFocusBool))
      {
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
