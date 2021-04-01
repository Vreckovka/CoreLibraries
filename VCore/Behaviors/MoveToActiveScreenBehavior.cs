using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace VCore.WPF.Behaviors
{
  public class MoveToActiveScreenBehavior : Behavior<Window>
  {
    #region UseAutomatic

    public bool UseAutomatic
    {
      get { return (bool)GetValue(UseAutomaticProperty); }
      set { SetValue(UseAutomaticProperty, value); }
    }

    public static readonly DependencyProperty UseAutomaticProperty =
      DependencyProperty.Register(
        nameof(UseAutomatic),
        typeof(bool),
        typeof(MoveToActiveScreenBehavior),
        new PropertyMetadata(null));


    #endregion

    #region SwitchCommnd

    public ICommand SwitchCommnd
    {
      get { return (ICommand)GetValue(SwitchCommndProperty); }
      set { SetValue(SwitchCommndProperty, value); }
    }

    public static readonly DependencyProperty SwitchCommndProperty =
      DependencyProperty.Register(
        nameof(SwitchCommnd),
        typeof(ICommand),
        typeof(MoveToActiveScreenBehavior),
        new PropertyMetadata(null));


    #endregion

    #region Methods

    #region OnAttached

    protected override void OnAttached()
    {
      AssociatedObject.StateChanged += MainWindow_StateChanged;
      AssociatedObject.Loaded += AssociatedObject_Loaded;
    }

    #endregion

    #region AssociatedObject_Loaded

    private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
    {
      SwitchCommnd = new ActionCommand(SwitchToActiveScreenScreen);
    }

    #endregion

    #region MainWindow_StateChanged

    WindowState lastState;
    private void MainWindow_StateChanged(object sender, EventArgs e)
    {
      if (AssociatedObject.WindowState != WindowState.Minimized && UseAutomatic && lastState == WindowState.Minimized)
      {
        SwitchToActiveScreenScreen();
      }

      lastState = AssociatedObject.WindowState;
    }

    #endregion

    #region SwitchToActiveScreenScreen

    private void SwitchToActiveScreenScreen()
    {
      AssociatedObject.Topmost = true;

      var currentScreen = GetActiveScreen();
      var applicationScreen = GetApplicationScreen();

      if (currentScreen != applicationScreen)
      {
        if (AssociatedObject.WindowState != WindowState.Normal)
        {
          AssociatedObject.WindowState = WindowState.Normal;
        }

        AssociatedObject.Left = currentScreen.Bounds.X + (currentScreen.Bounds.Width / 2) - (AssociatedObject.Width / 2);
        AssociatedObject.Top = currentScreen.Bounds.Y + (currentScreen.Bounds.Height / 2) - (AssociatedObject.Height / 2);
      }

      AssociatedObject.Topmost = false;
    }

    #endregion

    #region GetApplicationScreen

    private Screen GetApplicationScreen()
    {
      return GetScreen(new Point(AssociatedObject.RestoreBounds.X, AssociatedObject.RestoreBounds.Y));
    }

    #endregion

    #region GetActiveScreen

    private Screen GetActiveScreen()
    {
      var mousePostion = GetMousePosition();

      return GetScreen(mousePostion);
    }

    #endregion

    #region GetScreen

    private Screen GetScreen(Point point)
    {
      var screens = System.Windows.Forms.Screen.AllScreens;
      Screen currentScreen = Screen.PrimaryScreen;
      foreach (var screen in screens)
      {
        double topLeftX = screen.Bounds.X;
        double topLeftY = screen.Bounds.Y;
        double bottomRightX = screen.Bounds.X + screen.Bounds.Width;
        double bottomRightY = screen.Bounds.Y + screen.Bounds.Height;


        if (topLeftX <= point.X && point.X <= bottomRightX && topLeftY <= point.Y && point.Y <= bottomRightY)
        {
          return screen;
        }

      }

      return currentScreen;
    }

    #endregion

    #region GetMousePosition

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(ref Win32Point pt);

    [StructLayout(LayoutKind.Sequential)]
    internal struct Win32Point
    {
      public Int32 X;
      public Int32 Y;
    };
    public static Point GetMousePosition()
    {
      var w32Mouse = new Win32Point();
      GetCursorPos(ref w32Mouse);

      return new Point(w32Mouse.X, w32Mouse.Y);
    }

    #endregion

    #region OnDetaching

    protected override void OnDetaching()
    {
      AssociatedObject.StateChanged -= MainWindow_StateChanged;
      AssociatedObject.Loaded -= AssociatedObject_Loaded;
    }

    #endregion

    #endregion
  }

}
