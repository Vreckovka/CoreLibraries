using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using VCore.WPF.Managers;

namespace VCore.WPF.Behaviors.Sliders
{
  public class SliderHookBehavior : Behavior<Slider>
  {
    public double Step { get; set; } = 5;

    #region OnAttached

    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.PreviewMouseLeftButtonUp += AssociatedObject_PreviewMouseLeftButtonUp;
      AssociatedObject.PreviewMouseWheel += Slider_PreviewMouseWheel;
      AssociatedObject.GotFocus += AssociatedObject_GotFocus;
      AssociatedObject.LostFocus += AssociatedObject_LostFocus; ;
    }

    #endregion

    private void AssociatedObject_LostFocus(object sender, RoutedEventArgs e)
    {
      isFocusFromBehavior = false;
    }

    private void AssociatedObject_GotFocus(object sender, RoutedEventArgs e)
    {
      if (!isFocusFromBehavior)
      {
        HookToSlider();
      }
    }

    private void AssociatedObject_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      HookToSlider();
    }


    #region Slider_PreviewMouseWheel

    private void Slider_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
      SetSliderValue(sender, e);
    }

    #endregion

    #region SetSliderValue

    private void SetSliderValue(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
      if (sender is Slider slider)
      {
       

        if (e.Delta > 0)
        {
          slider.Value += Step;
        }
        else
        {
          slider.Value -= Step;
        }

        e.Handled = true;
      }
    }

    #endregion

    #region Slider_PreviewMouseUp

    private void Slider_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      if (sender is Slider slider)
      {
        slider.IsMoveToPointEnabled = true;

        if (!IsMouseOver(slider))
        {
          slider.ReleaseMouseCapture();
          VFocusManager.SetFocus(Application.Current.MainWindow);
          e.Handled = true;
          slider.PreviewMouseUp -= Slider_PreviewMouseDown;
        }
      }
    }

    #endregion

    #region HookToSlider

    private bool isFocusFromBehavior;
    private void HookToSlider()
    {
      if (Mouse.Captured != AssociatedObject)
      {
        AssociatedObject.IsMoveToPointEnabled = false;

        VFocusManager.SetFocus(AssociatedObject);
        AssociatedObject.Focus();
        AssociatedObject.CaptureMouse();

        AssociatedObject.PreviewMouseDown += Slider_PreviewMouseDown;
        AssociatedObject.MouseWheel += Slider_PreviewMouseWheel;

        isFocusFromBehavior = true;
      }
    }

    #endregion

    #region IsMouseOver

    private bool IsMouseOver(FrameworkElement element)
    {
      bool IsMouseOverEx = false;

      VisualTreeHelper.HitTest(element, d =>
        {
          if (d == element)
          {
            IsMouseOverEx = true;
            return HitTestFilterBehavior.Stop;
          }
          else
            return HitTestFilterBehavior.Continue;
        },
        ht => HitTestResultBehavior.Stop,
        new PointHitTestParameters(Mouse.GetPosition(element)));

      return IsMouseOverEx;
    }

    #endregion

    protected override void OnDetaching()
    {
      base.OnDetaching();

      AssociatedObject.PreviewMouseLeftButtonUp -= AssociatedObject_PreviewMouseLeftButtonUp;
      AssociatedObject.PreviewMouseWheel -= Slider_PreviewMouseWheel;
    }
  }
}