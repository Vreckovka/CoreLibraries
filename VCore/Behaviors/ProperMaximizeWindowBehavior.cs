using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Xaml.Behaviors;

namespace VCore.WPF.Behaviors
{
  public class ProperMaximizeWindowBehavior : Behavior<Window>
  {
    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.SourceInitialized += new EventHandler(win_SourceInitialized);
    }

    void win_SourceInitialized(object sender, System.EventArgs e)
    {
      var handle = (new WindowInteropHelper(AssociatedObject)).Handle;
      var handleSource = HwndSource.FromHwnd(handle);
      if (handleSource == null)
        return;
      handleSource.AddHook(WindowProc);
    }

    private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      switch (msg)
      {
        case 0x0024:/* WM_GETMINMAXINFO */
          WmGetMinMaxInfo(hwnd, lParam);
          //handled = true;
          break;
      }

      return (IntPtr)0;
    }

    private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
    {
      var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

      // Adjust the maximized size and position to fit the work area of the correct monitor
      var currentScreen = System.Windows.Forms.Screen.FromHandle(hwnd);
      var workArea = currentScreen.WorkingArea;
      var monitorArea = currentScreen.Bounds;
      mmi.ptMaxPosition.x = Math.Abs(workArea.Left - monitorArea.Left);
      mmi.ptMaxPosition.y = Math.Abs(workArea.Top - monitorArea.Top);
      mmi.ptMaxSize.x = Math.Abs(workArea.Right - workArea.Left);
      mmi.ptMaxSize.y = Math.Abs(workArea.Bottom - workArea.Top);

      Marshal.StructureToPtr(mmi, lParam, true);
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();

      AssociatedObject.SourceInitialized -= win_SourceInitialized;
    }

  }


  [StructLayout(LayoutKind.Sequential)]
  public struct MINMAXINFO
  {
    public POINT ptReserved;
    public POINT ptMaxSize;
    public POINT ptMaxPosition;
    public POINT ptMinTrackSize;
    public POINT ptMaxTrackSize;
  };

  [StructLayout(LayoutKind.Sequential)]
  public struct POINT
  {
    /// <summary>
    /// x coordinate of point.
    /// </summary>
    public int x;
    /// <summary>
    /// y coordinate of point.
    /// </summary>
    public int y;

    /// <summary>
    /// Construct a point of coordinates (x,y).
    /// </summary>
    public POINT(int x, int y)
    {
      this.x = x;
      this.y = y;
    }
  }
}
