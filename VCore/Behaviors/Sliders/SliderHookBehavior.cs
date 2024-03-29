﻿
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Listener;
using Microsoft.Xaml.Behaviors;
using Ninject;
using VCore.Standard;
using VCore.WPF.Managers;

namespace VCore.WPF.Behaviors.Sliders
{
  public class SliderHookBehavior : Behavior<Slider>
  {
    public double Step { get; set; } = 2;
    public bool HookWhenFullscreen { get; set; }
    public bool WheelOnly { get; set; }
    private KeyListener keyListener;
    private bool isHooked;
    private bool isFocusFromBehavior;
    private bool isHookedOnFullScreen;

    #region OnAttached

    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.PreviewMouseWheel += Slider_PreviewMouseWheel;

      if (!WheelOnly)
      {
        AssociatedObject.PreviewMouseLeftButtonUp += AssociatedObject_PreviewMouseLeftButtonUp;

        AssociatedObject.Focusable = true;
        AssociatedObject.GotFocus += AssociatedObject_GotFocus;
        AssociatedObject.LostFocus += AssociatedObject_LostFocus;
      }
    

      Application.Current.MainWindow.Deactivated += MainWindow_Deactivated;

      keyListener = VIoc.Kernel.Get<KeyListener>();


      if (HookWhenFullscreen)
      {
        if (FullScreenManager.IsFullscreen)
        {
          isHookedOnFullScreen = true;
          HookSlider();
        }

        FullScreenManager.OnFullScreen.Subscribe((x) =>
        {
          if (isHookedOnFullScreen)
          {
            UnHookSlider();
          }
          if (x && !isHooked)
          {
            isHookedOnFullScreen = true;
            HookSlider();
          }
        });
      }
    }

    private void MainWindow_Deactivated(object sender, EventArgs e)
    {
      UnHookSlider();
    }


    #endregion

    private void AssociatedObject_LostFocus(object sender, RoutedEventArgs e)
    {
      UnHookSlider();
    }

    private void AssociatedObject_GotFocus(object sender, RoutedEventArgs e)
    {
      if (!isFocusFromBehavior)
      {
        HookSlider();
      }
    }

    private void AssociatedObject_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      HookSlider();

      isFocusFromBehavior = true;
    }

    #region Slider_PreviewMouseWheel

    private void Slider_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
      SetSliderValue(e.Delta);

      e.Handled = true;
    }

    #endregion

    #region HookSlider

    private void HookSlider()
    {
      if (!isHooked)
      {
        isHooked = true;
        isFocusFromBehavior = true;

        VFocusManager.SetFocus(AssociatedObject);
        AssociatedObject.Focus();
        //AssociatedObject.CaptureMouse();

        Task.Run(() =>
        {
          keyListener.HookMouse();
        });

        keyListener.OnMouseEvent += KeyListener_OnMouseEvent;
      }
    }

    #endregion

    #region UnHookSlider

    private void UnHookSlider()
    {
      isHooked = false;
      isFocusFromBehavior = false;
      //AssociatedObject.ReleaseMouseCapture();

      Task.Run(() =>
      {
        keyListener.UnHookMouse();
      });

      keyListener.OnMouseEvent -= KeyListener_OnMouseEvent;

      if (isHookedOnFullScreen)
      {
        isHookedOnFullScreen = false;
      }
    }

    #endregion

    #region SetSliderValue

    private void SetSliderValue(int delta)
    {
      VSynchronizationContext.PostOnUIThread(() =>
      {
        if (delta > 0)
        {
          if (AssociatedObject.Value + Step <= AssociatedObject.Maximum)
            AssociatedObject.Value += Step;
        }
        else
        {
          if (AssociatedObject.Value - Step >= 0)
            AssociatedObject.Value -= Step;
        }
      });

    }

    #endregion

    #region KeyListener_OnMouseEvent

    private void KeyListener_OnMouseEvent(object sender, Listener.MouseEventArgs e)
    {
      if (e.Event == MouseMessages.WM_MOUSEWHEEL)
      {
        var value = (int)e.EventData;

        SetSliderValue(value);
      }
    }

    #endregion

    #region OnDetaching

    protected override void OnDetaching()
    {
      base.OnDetaching();

      AssociatedObject.PreviewMouseWheel -= Slider_PreviewMouseWheel;
      UnHookSlider();

      if (!WheelOnly)
      {
        AssociatedObject.PreviewMouseLeftButtonUp += AssociatedObject_PreviewMouseLeftButtonUp;
        AssociatedObject.GotFocus -= AssociatedObject_GotFocus;
        AssociatedObject.LostFocus -= AssociatedObject_LostFocus;
      }
    }

    #endregion
  }
}