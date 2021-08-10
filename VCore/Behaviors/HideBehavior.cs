using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Xaml.Behaviors;
using VCore.Helpers;

namespace VCore.WPF.Behaviors
{
  public interface IHideable
  {
    event EventHandler<bool> Hide;
  }

  public enum ResizeParameter
  {
    Width,
    Height
  }
  public class HideBehavior : Behavior<FrameworkElement>
  {
    #region Fields

    private GridSplitter gridSplitter;
    private Grid parentGrid;
    private double valueBeforeAnimation;
    private double? lastValue;
    private DoubleAnimation animation;
    private EventHandler showEventHandler;
    private EventHandler hideEventHandler;
    private bool wasInitlized;
    private ButtonBase executeButton;
    private Window parentWindow;

    #endregion

    #region Properties

    public double MinValue { get; set; }
    public string ExecuteButtonName { get; set; }
    public string GridSplitterName { get; set; }
    public Duration Duration { get; set; } = new Duration(TimeSpan.FromSeconds(1));
    public ResizeParameter ResizeParameter { get; set; }
    public bool ChangeVisibility { get; set; }

    #region IsHidden

    public static readonly DependencyProperty IsHiddenProperty =
      DependencyProperty.Register(
        nameof(IsHidden),
        typeof(bool),
        typeof(HideBehavior),
        new PropertyMetadata(null));

    public bool IsHidden
    {
      get { return (bool)GetValue(IsHiddenProperty); }
      set { SetValue(IsHiddenProperty, value); }
    }

    #endregion

    #region ValueToExpand

    public static readonly DependencyProperty ValueToExpandProperty =
      DependencyProperty.Register(
        nameof(ValueToExpand),
        typeof(double?),
        typeof(HideBehavior),
        new PropertyMetadata(null));

    public double? ValueToExpand
    {
      get { return (double?)GetValue(ValueToExpandProperty); }
      set { SetValue(ValueToExpandProperty, value); }
    }

    #endregion

    #region CanHide

    public static readonly DependencyProperty CanHideProperty =
      DependencyProperty.Register(
        nameof(CanHide),
        typeof(bool),
        typeof(HideBehavior),
        new PropertyMetadata(true));

    public bool CanHide
    {
      get { return (bool)GetValue(CanHideProperty); }
      set { SetValue(CanHideProperty, value); }
    }

    #endregion

    #region HideOnLostCapture

    public static readonly DependencyProperty HideOnLostCaptureProperty =
      DependencyProperty.Register(
        nameof(HideOnLostCapture),
        typeof(bool),
        typeof(HideBehavior),
        new PropertyMetadata(false));

    public bool HideOnLostCapture
    {
      get { return (bool)GetValue(HideOnLostCaptureProperty); }
      set { SetValue(HideOnLostCaptureProperty, value); }
    }

    #endregion

    #endregion

    #region Methods

    #region OnAttached

    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.LayoutUpdated += AssociatedObject_LayoutUpdated;
    }


    #endregion

    #region AddWindowHandler

    private void AddWindowHandler()
    {
      parentWindow?.AddHandler
      (
        UIElement.MouseUpEvent,
        (MouseButtonEventHandler)HandleClickOutsideOfControl,
        true
      );

      parentWindow.Deactivated += ParentWindow_Deactivated;
    }

    #endregion

    #region ParentWindow_Deactivated

    private void ParentWindow_Deactivated(object sender, EventArgs e)
    {
      ReleaseObject();
    }

    #endregion

    #region HandleClickOutsideOfControl

    private void HandleClickOutsideOfControl(object sender, MouseButtonEventArgs e)
    {
      ReleaseObject();
    }

    #endregion

    #region ReleaseObject

    private void ReleaseObject()
    {
      AssociatedObject.ReleaseMouseCapture();

      if (CanHide && HideOnLostCapture && !AssociatedObject.IsMouseOver && animation == null)
      {
        if (executeButton is ToggleButton toggleButton)
        {
          toggleButton.IsChecked = false;
        }

        Button_Click(null, null);
      }
    }

    #endregion

    #region AssociatedObject_LayoutUpdated

    private void AssociatedObject_LayoutUpdated(object sender, EventArgs e)
    {
      if (!wasInitlized)
      {
        var grid = (FrameworkElement)AssociatedObject;
        parentGrid = (Grid)VisualTreeHelper.GetParent(grid);
        executeButton = (ButtonBase)parentGrid.FindChildByName(ExecuteButtonName);
        gridSplitter = parentGrid.FindChildByName<GridSplitter>(GridSplitterName);
        parentWindow = parentGrid.GetFirstParentOfType<Window>();

        AddWindowHandler();

        if (ResizeParameter == ResizeParameter.Height)
        {
          if (AssociatedObject.ActualHeight == 0 || AssociatedObject.Width == 0 || AssociatedObject.Height == MinValue)
          {
            IsHidden = true;
            CanHide = false;
          }

          valueBeforeAnimation = AssociatedObject.Height;
        }
        else
        {
          if (AssociatedObject.ActualWidth == 0 || AssociatedObject.Width == 0 || AssociatedObject.Width == MinValue)
          {
            IsHidden = true;
            CanHide = false;
          }

          valueBeforeAnimation = AssociatedObject.Width;
        }

        if (AssociatedObject.DataContext is IHideable hideable)
        {
          hideable.Hide += (s, e) => Button_Click(null, null);
        }
        else if (executeButton != null)
        {
          executeButton.Click += Button_Click;
        }

        wasInitlized = true;
      }
    }

    #endregion

    #region Button_Click

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      if (ResizeParameter == ResizeParameter.Height)
      {
        StartResize(AssociatedObject.ActualHeight, FrameworkElement.HeightProperty, ResizeParameter);
      }
      else
      {
        StartResize(AssociatedObject.ActualWidth, FrameworkElement.WidthProperty, ResizeParameter);

        if (gridSplitter != null)
        {
          parentGrid.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Auto);
        }
      }
    }

    #endregion

    #region StartResize

    private async void StartResize(double actualValue, DependencyProperty dependencyProperty, ResizeParameter resizeParameter)
    {
      if (hideEventHandler == null)
      {
        hideEventHandler = new EventHandler((x, y) => { DoubleAnim_Completed(MinValue, dependencyProperty, resizeParameter); });
      }

      if (animation == null)
      {
        IsHidden = !IsHidden;
        CanHide = !IsHidden;

        if (IsHidden)
        {
          lastValue = actualValue;

          animation = new DoubleAnimation(actualValue, MinValue, Duration);
          animation.AccelerationRatio = 0.5;


          animation.FillBehavior = FillBehavior.Stop;

          animation.Completed += hideEventHandler;

          AssociatedObject.BeginAnimation(dependencyProperty, animation);
        }
        else
        {
          if (ChangeVisibility)
            if (AssociatedObject.Visibility != Visibility.Visible)
            {
              AssociatedObject.Visibility = Visibility.Visible;
              await Task.Delay(10);
            }

          double valueToExpand = 0;

          if (lastValue != null)
          {
            valueToExpand = lastValue.Value;
          }

          if (ValueToExpand != null)
          {
            valueToExpand = ValueToExpand.Value;
          }

          if (showEventHandler == null)
          {
            showEventHandler = new EventHandler((x, y) => { DoubleAnim_Completed(valueToExpand, dependencyProperty, resizeParameter); });
          }

          animation = new DoubleAnimation(MinValue, valueToExpand, Duration);
          animation.AccelerationRatio = 0.5;

          animation.FillBehavior = FillBehavior.Stop;

          animation.Completed += showEventHandler;

          AssociatedObject.BeginAnimation(dependencyProperty, animation);
        }
      }
    }

    #endregion

    #region DoubleAnim_Completed

    private void DoubleAnim_Completed(double value, DependencyProperty dependencyProperty, ResizeParameter resizeParameter)
    {
      AssociatedObject.BeginAnimation(dependencyProperty, null);

      if (value == MinValue)
      {
        if (resizeParameter == ResizeParameter.Height)
        {
          AssociatedObject.Height = value;
        }
        else
        {
          AssociatedObject.Width = value;
        }
      }
      else
      {
        var setValue = valueBeforeAnimation;

        if (ValueToExpand != null)
        {
          setValue = ValueToExpand.Value;
        }

        if (resizeParameter == ResizeParameter.Height)
        {
          if (gridSplitter != null)
            parentGrid.RowDefinitions[2].Height = new GridLength(value);

          AssociatedObject.Height = setValue;
        }
        else
        {
          if (gridSplitter != null)
            parentGrid.ColumnDefinitions[2].Width = new GridLength(value);


          AssociatedObject.Width = setValue;
        }


      }

      if (showEventHandler != null)
        animation.Completed -= showEventHandler;

      if (showEventHandler != null)
        animation.Completed -= hideEventHandler;

      animation = null;
    }

    #endregion

    #region OnDetaching

    protected override void OnDetaching()
    {
      AssociatedObject.LayoutUpdated -= AssociatedObject_LayoutUpdated;

      if (executeButton != null)
        executeButton.Click += Button_Click;

      parentWindow?.RemoveHandler
      (
        UIElement.MouseDownEvent,
        (MouseButtonEventHandler)HandleClickOutsideOfControl
      );
    }



    #endregion 

    #endregion
  }
}
