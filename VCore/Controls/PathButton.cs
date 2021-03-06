﻿using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace VCore.Controls
{

  public class PathButton : ToggleButton
  {
    #region PathStyle

    public Style PathStyle
    {
      get { return (Style) GetValue(PathStyleProperty); }
      set { SetValue(PathStyleProperty, value); }
    }

    public static readonly DependencyProperty PathStyleProperty =
      DependencyProperty.Register(
        nameof(PathStyle),
        typeof(Style),
        typeof(PathButton),
        new PropertyMetadata(null));


    #endregion

    #region PathCheckedBrush

    public Brush PathCheckedBrush
    {
      get { return (Brush) GetValue(PathCheckedBrushProperty); }
      set { SetValue(PathCheckedBrushProperty, value); }
    }

    public static readonly DependencyProperty PathCheckedBrushProperty =
      DependencyProperty.Register(
        nameof(PathCheckedBrush),
        typeof(Brush),
        typeof(PathButton),
        new PropertyMetadata(Brushes.Black));


    #endregion

    #region IconHoverColor

    public Color IconHoverColor
    {
      get { return (Color) GetValue(IconHoverColorProperty); }
      set { SetValue(IconHoverColorProperty, value); }
    }

    public static readonly DependencyProperty IconHoverColorProperty =
      DependencyProperty.Register(
        nameof(IconHoverColor),
        typeof(Color),
        typeof(PathButton),
        new PropertyMetadata((Color) ColorConverter.ConvertFromString("#f0f8ff")));


    #endregion

    #region IconDefaultColor

    public Color IconDefaultColor
    {
      get { return (Color) GetValue(IconDefaultColorProperty); }
      set { SetValue(IconDefaultColorProperty, value); }
    }

    public static readonly DependencyProperty IconDefaultColorProperty =
      DependencyProperty.Register(
        nameof(IconDefaultColor),
        typeof(Color),
        typeof(PathButton),
        new PropertyMetadata(Colors.Transparent, (x, y) =>
        {
          if (x is PathButton buttonWithIcon)
          {
            if (y.NewValue is Color newColor && buttonWithIcon.IconBrush is SolidColorBrush solidColorBrush)
            {
              if (solidColorBrush.Color != newColor)
              {
                buttonWithIcon.AnimateImageDefaultColor(0);
              }
            }
          }
        }));

    #endregion

    #region IconBrush

    public Brush IconBrush
    {
      get { return (Brush)GetValue(IconBrushProperty); }
      set { SetValue(IconBrushProperty, value); }
    }

    public static readonly DependencyProperty IconBrushProperty =
      DependencyProperty.Register(
        nameof(IconBrush),
        typeof(Brush),
        typeof(PathButton),
        new PropertyMetadata((SolidColorBrush)(new BrushConverter().ConvertFrom("#30ffffff"))));

    #endregion

    ColorAnimation hoverUp;
    ColorAnimation hoverDown;
    private double animationInSeconds = .15;

    #region AnimateHoverColor

    private ActionCommand animateHoverColor;

    public ICommand AnimateHoverColor
    {
      get
      {
        if (animateHoverColor == null)
        {
          animateHoverColor = new ActionCommand(OnAnimateHoverColor);
        }

        return animateHoverColor;
      }
    }



    public void OnAnimateHoverColor()
    {
      Brush rootElementBrush;

      if (IconBrush.IsFrozen)
      {
        rootElementBrush = IconBrush.Clone();
      }
      else
      {
        rootElementBrush = IconBrush;
      }

      hoverUp = new ColorAnimation();

      hoverUp.From = IconDefaultColor;
      hoverUp.To = IconHoverColor;

      hoverUp.Duration = new Duration(TimeSpan.FromSeconds(animationInSeconds));

      rootElementBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
      rootElementBrush.BeginAnimation(SolidColorBrush.ColorProperty, hoverUp);

      IconBrush = rootElementBrush;
    }

    #endregion

    #region AnimateDefaultColor

    private ActionCommand animateDefaultColor;

    public ICommand AnimateDefaultColor
    {
      get
      {
        if (animateDefaultColor == null)
        {
          animateDefaultColor = new ActionCommand(OnAnimateDefaultColor);
        }

        return animateDefaultColor;
      }
    }

    private void AnimateImageDefaultColor(double? durationSeconds = null)
    {
      Brush rootElementBrush;

      hoverDown = new ColorAnimation();

      hoverDown.From = IconHoverColor;
      hoverDown.To = IconDefaultColor;

      if (IconBrush.IsFrozen)
      {
        rootElementBrush = IconBrush.Clone();
      }
      else
      {
        rootElementBrush = IconBrush;
      }

      if (durationSeconds != null)
      {
        hoverDown.Duration = new Duration(TimeSpan.FromSeconds(durationSeconds.Value));
      }
      else
      {
        hoverDown.Duration = new Duration(TimeSpan.FromSeconds(animationInSeconds));
      }


      rootElementBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
      rootElementBrush.BeginAnimation(SolidColorBrush.ColorProperty, hoverDown);

      IconBrush = rootElementBrush;
    }

    public void OnAnimateDefaultColor()
    {
      AnimateImageDefaultColor();
    }

    #endregion
  }
}
