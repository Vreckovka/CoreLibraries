using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace VCore.WPF.Behaviors.Sliders
{
  public class WheelSliderBehavior : Behavior<FrameworkElement>
  {
    public double Step { get; set; } = 2;

    #region Slider

    public Slider Slider
    {
      get { return (Slider)GetValue(SliderProperty); }
      set { SetValue(SliderProperty, value); }
    }

    public static readonly DependencyProperty SliderProperty =
      DependencyProperty.Register(
        nameof(Slider),
        typeof(Slider),
        typeof(WheelSliderBehavior),
        new PropertyMetadata(null));


    #endregion

    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.MouseWheel += AssociatedObject_MouseWheel;
    }

    private void AssociatedObject_MouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (Slider == null)
      {
        return;
      }

      var delta = e.Delta;

      if (delta > 0)
      {
        if (Slider.Value + Step <= Slider.Maximum)
          Slider.Value += Step;
        else if (Slider.Value <= Slider.Maximum)
        {
          Slider.Value = Slider.Maximum;
        }
      }
      else
      {
        if (Slider.Value - Step >= 0)
          Slider.Value -= Step;
      }

      e.Handled = true;
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();

      AssociatedObject.MouseWheel -= AssociatedObject_MouseWheel;
    }
  }
}