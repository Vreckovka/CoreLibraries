﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using VCore.WPF.Interfaces.Managers;

namespace VCore.WPF.Managers
{
  public static class FullScreenManager
  {
    private static Timer cursorTimer;
    private static ElapsedEventHandler hideCursorDelegate;
    private static ReplaySubject<Unit> resetMouseSubject = new ReplaySubject<Unit>(1);
    private static ReplaySubject<Unit> hideMouseSubject = new ReplaySubject<Unit>(1);
    private static ReplaySubject<bool> fullsScreenSubject = new ReplaySubject<bool>(1);
    private static object batton = new object();

    static FullScreenManager()
    {
      cursorTimer = new Timer(1500);
      cursorTimer.AutoReset = false;

      hideCursorDelegate = (s, e) =>
      {
        if (!IsMouseBlocked && isFullscreen)
        {
          hideMouseSubject.OnNext(Unit.Default);
          SafeOverrideCursor(Cursors.None);
          IsMouseHidden = true;
        }
        else
        {
          ResetMouse();
        }
      };

      cursorTimer.Elapsed += hideCursorDelegate;

    }

    #region OnResetMouse

    public static IObservable<Unit> OnResetMouse
    {
      get
      {
        return resetMouseSubject.AsObservable();
      }
    }

    #endregion

    #region OnHideMouse

    public static IObservable<Unit> OnHideMouse
    {
      get
      {
        return hideMouseSubject.AsObservable();
      }
    }

    #endregion

    #region OnFullScreen

    public static IObservable<bool> OnFullScreen
    {
      get
      {
        return fullsScreenSubject.AsObservable();
      }
    }

    #endregion

    #region IsFullscreen

    private static bool isFullscreen;

    public static bool IsFullscreen
    {
      get { return isFullscreen; }
      set
      {
        if (value != isFullscreen)
        {
          isFullscreen = value;

          if (isFullscreen)
          {
            cursorTimer.Stop();
            cursorTimer.Start();
          }

          ResetMouse();

          fullsScreenSubject.OnNext(value);
        }
      }
    }

    #endregion

    public static bool IsMouseHidden { get; set; }

    public static bool IsMouseBlocked { get; set; }

    #region ResetMouse

    public static void ResetMouse()
    {
      if (IsMouseHidden)
      {
        cursorTimer.Stop();

        Mouse.OverrideCursor = null; //Show cursor
        resetMouseSubject.OnNext(Unit.Default);
        IsMouseHidden = false;

        cursorTimer.Start();
      }
      else 
      {
        cursorTimer.Stop();

        if (IsFullscreen && !IsMouseBlocked)
          cursorTimer.Start();
      }

    }

    #endregion

    #region SafeOverrideCursor

    private static void SafeOverrideCursor(Cursor cursor)
    {
      VSynchronizationContext.InvokeOnDispatcher(new Action(() =>
      {
        Mouse.OverrideCursor = cursor;
      }));
    }

    #endregion

  }
}