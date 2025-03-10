using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using VCore.WPF.Interfaces;

namespace VCore.WPF.Misc
{
  public class SaveWindowsPositionFunction
  {
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
      public int Left;
      public int Top;
      public int Right;
      public int Bottom;
    }

    public static int SWP_NOSIZE = 1;

    [DllImport("kernel32")]
    static extern IntPtr GetConsoleWindow();


    [DllImport("user32")]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
      int x, int y, int cx, int cy, int flags);



    private string positionPath = "position.json";
    private Window mainWindow;


    public SaveWindowsPositionFunction(Window mainWindow)
    {
      this.mainWindow = mainWindow;
      mainWindow.Loaded += MainWindow_Loaded;
      mainWindow.Closing += MainWindow_Closing;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      if (File.Exists(positionPath))
      {
        var positions = JsonSerializer.Deserialize<SaveWindowsPositionFunction.WindowPosition[]>(File.ReadAllText(positionPath));

        var pos = positions[0];

        mainWindow.Left = pos.Left;
        mainWindow.Top = pos.Top;
        mainWindow.Width = pos.Width;
        mainWindow.Height = pos.Height;

        if (positions.Length > 1)
        {
          var con = positions[1];

          var handle = GetConsoleWindow();

          if (IntPtr.Zero != handle)
          {
            SetWindowPos(handle, IntPtr.Zero,
              (int)con.Left,
              (int)con.Top,
              (int)con.Width,
              (int)con.Height, SWP_NOSIZE);

            Console.WindowHeight = (int)con.Height;
            Console.WindowWidth = (int)con.Width;
            Console.BufferWidth = (int)con.Width;
          }
        }
      }
    }

    private void MainWindow_Closing(object sender, EventArgs e)
    {
      var position = new SaveWindowsPositionFunction.WindowPosition();

      if (mainWindow.WindowState == WindowState.Normal)
      {
        position.Left = mainWindow.Left;
        position.Top = mainWindow.Top;
        position.Height = mainWindow.ActualHeight;
        position.Width = mainWindow.ActualWidth;
      }
      else
      {
        // Use RestoreBounds when the window is minimized or maximized
        position.Left = mainWindow.RestoreBounds.Left;
        position.Top = mainWindow.RestoreBounds.Top;
        position.Height = mainWindow.RestoreBounds.Height;
        position.Width = mainWindow.RestoreBounds.Width;
      }

      var handle = GetConsoleWindow();

      if (IntPtr.Zero != handle)
      {
        var console = new SaveWindowsPositionFunction.WindowPosition();

        RECT rct = new RECT();
        GetWindowRect(handle, ref rct);

        console.Left = rct.Left;
        console.Top = rct.Top;
        console.Height = Console.WindowHeight;
        console.Width = Console.WindowWidth;

        File.WriteAllText(positionPath, JsonSerializer.Serialize(new SaveWindowsPositionFunction.WindowPosition[] { position, console }));
      }
      else
      {
        File.WriteAllText(positionPath, JsonSerializer.Serialize(new SaveWindowsPositionFunction.WindowPosition[] { position }));
      }
    }



    public class WindowPosition
    {
      public double Height { get; set; }
      public double Width { get; set; }
      public double Left { get; set; }
      public double Top { get; set; }
    }
  }
}
