using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace VCore.Other
{
  public static class WinConsole
  {
    [DllImport("kernel32.dll")]
    public static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    public static extern bool FreeConsole();


    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetStdHandle(int nStdHandle, IntPtr hHandle);

    public const int STD_OUTPUT_HANDLE = -11;
    public const int STD_INPUT_HANDLE = -10;
    public const int STD_ERROR_HANDLE = -12;

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPTStr)] string filename,
      [MarshalAs(UnmanagedType.U4)] uint access,
      [MarshalAs(UnmanagedType.U4)] FileShare share,
      IntPtr securityAttributes,
      [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
      [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
      IntPtr templateFile);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr CreateFileW(
      [MarshalAs(UnmanagedType.LPWStr)] string filename,
      [MarshalAs(UnmanagedType.U4)] uint access,
      [MarshalAs(UnmanagedType.U4)] uint share,
      IntPtr securityAttributes,
      [MarshalAs(UnmanagedType.U4)] uint creationDisposition,
      [MarshalAs(UnmanagedType.U4)] uint flagsAndAttributes,
      IntPtr templateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    public const uint GENERIC_WRITE = 0x40000000;
    public const uint GENERIC_READ = 0x80000000;
    private const int MY_CODE_PAGE = 437;
    private const uint FILE_SHARE_WRITE = 0x2;
    private const uint OPEN_EXISTING = 0x3;
    private const uint ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200;

    public static void OverrideRedirection()
    {
      AllocConsole();

      var hOut = GetStdHandle(STD_OUTPUT_HANDLE);
      var hRealOut = CreateFile("CONOUT$", GENERIC_READ | GENERIC_WRITE, FileShare.Write, IntPtr.Zero, FileMode.OpenOrCreate, 0, IntPtr.Zero);
      if (hRealOut != hOut)
      {
        SetStdHandle(STD_OUTPUT_HANDLE, hRealOut);
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding) { AutoFlush = true });
      }

      if (GetConsoleMode(hRealOut, out var cMode))
        SetConsoleMode(hRealOut, cMode | ENABLE_VIRTUAL_TERMINAL_INPUT);
    }
    

    private enum StdHandle : int
    {
      Input = -10,
      Output = -11,
      Error = -12
    }

    public static bool CreateConsole()
    {
      if (AllocConsole())
      {
        //https://developercommunity.visualstudio.com/content/problem/12166/console-output-is-gone-in-vs2017-works-fine-when-d.html
        // Console.OpenStandardOutput eventually calls into GetStdHandle. As per MSDN documentation of GetStdHandle: http://msdn.microsoft.com/en-us/library/windows/desktop/ms683231(v=vs.85).aspx will return the redirected handle and not the allocated console:
        // "The standard handles of a process may be redirected by a call to  SetStdHandle, in which case  GetStdHandle returns the redirected handle. If the standard handles have been redirected, you can specify the CONIN$ value in a call to the CreateFile function to get a handle to a console's input buffer. Similarly, you can specify the CONOUT$ value to get a handle to a console's active screen buffer."
        // Get the handle to CONOUT$.    
        var stdOutHandle = CreateFile("CONOUT$", GENERIC_READ | GENERIC_WRITE, FileShare.ReadWrite, IntPtr.Zero, FileMode.CreateNew, FileAttributes.Normal, IntPtr.Zero);

        if (stdOutHandle == new IntPtr(-1))
        {
          throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        if (!SetStdHandle((int)StdHandle.Output, stdOutHandle))
        {
          throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);

        return true;
      }

      return false;
    }
  }
}