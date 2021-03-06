using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace VCore.Controls
{
  //public class AnimatePath : Control
  //{
  //  #region IconHoverColor

  //  public Color IconHoverColor
  //  {
  //    get { return (Color)GetValue(IconHoverColorProperty); }
  //    set { SetValue(IconHoverColorProperty, value); }
  //  }

  //  public static readonly DependencyProperty IconHoverColorProperty =
  //    DependencyProperty.Register(
  //      nameof(IconHoverColor),
  //      typeof(Color),
  //      typeof(ButtonWithIcon),
  //      new PropertyMetadata((Color)ColorConverter.ConvertFromString("#f0f8ff")));


  //  #endregion

  //  #region IconDefaultColor

  //  public Color IconDefaultColor
  //  {
  //    get { return (Color)GetValue(IconDefaultColorProperty); }
  //    set { SetValue(IconDefaultColorProperty, value); }
  //  }

  //  public static readonly DependencyProperty IconDefaultColorProperty =
  //    DependencyProperty.Register(
  //      nameof(IconDefaultColor),
  //      typeof(Color),
  //      typeof(ButtonWithIcon),
  //      new PropertyMetadata(Colors.Transparent, (x, y) =>
  //      {
  //        if (x is AnimatePath buttonWithIcon)
  //        {
  //          if (y.NewValue is Color newColor && buttonWithIcon.IconBrush is SolidColorBrush solidColorBrush)
  //          {
  //            if (solidColorBrush.Color != newColor)
  //            {
  //              buttonWithIcon.AnimateImageDefaultColor(0);
  //            }
  //          }
  //        }
  //      }));

  //  #endregion

  //  #region IconDefaultBrush

  //  public Brush IconBrush
  //  {
  //    get { return (Brush)GetValue(IconBrushProperty); }
  //    set { SetValue(IconBrushProperty, value); }
  //  }

  //  public static readonly DependencyProperty IconBrushProperty =
  //    DependencyProperty.Register(
  //      nameof(IconBrush),
  //      typeof(Brush),
  //      typeof(ButtonWithIcon),
  //      new PropertyMetadata((SolidColorBrush)(new BrushConverter().ConvertFrom("#30ffffff"))));

  //  #endregion

  //  ColorAnimation hoverUp;
  //  ColorAnimation hoverDown;
  //  private double animationInSeconds = .15;

  //  #region AnimateHoverColor

  //  private ActionCommand animateHoverColor;

  //  public ICommand AnimateHoverColor
  //  {
  //    get
  //    {
  //      if (animateHoverColor == null)
  //      {
  //        animateHoverColor = new ActionCommand(OnAnimateHoverColor);
  //      }

  //      return animateHoverColor;
  //    }
  //  }



  //  public void OnAnimateHoverColor()
  //  {
  //    Brush rootElementBrush;

  //    if (IconBrush.IsFrozen)
  //    {
  //      rootElementBrush = IconBrush.Clone();
  //    }
  //    else
  //    {
  //      rootElementBrush = IconBrush;
  //    }

  //    hoverUp = new ColorAnimation();

  //    hoverUp.From = IconDefaultColor;
  //    hoverUp.To = IconHoverColor;

  //    hoverUp.Duration = new Duration(TimeSpan.FromSeconds(animationInSeconds));

  //    rootElementBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
  //    rootElementBrush.BeginAnimation(SolidColorBrush.ColorProperty, hoverUp);

  //    IconBrush = rootElementBrush;
  //  }

  //  #endregion

  //  #region AnimateDefaultColor

  //  private ActionCommand animateDefaultColor;

  //  public ICommand AnimateDefaultColor
  //  {
  //    get
  //    {
  //      if (animateDefaultColor == null)
  //      {
  //        animateDefaultColor = new ActionCommand(OnAnimateDefaultColor);
  //      }

  //      return animateDefaultColor;
  //    }
  //  }

  //  private void AnimateImageDefaultColor(double? durationSeconds = null)
  //  {
  //    Brush rootElementBrush;

  //    hoverDown = new ColorAnimation();

  //    hoverDown.From = IconHoverColor;
  //    hoverDown.To = IconDefaultColor;

  //    if (IconBrush.IsFrozen)
  //    {
  //      rootElementBrush = IconBrush.Clone();
  //    }
  //    else
  //    {
  //      rootElementBrush = IconBrush;
  //    }

  //    if (durationSeconds != null)
  //    {
  //      hoverDown.Duration = new Duration(TimeSpan.FromSeconds(durationSeconds.Value));
  //    }
  //    else
  //    {
  //      hoverDown.Duration = new Duration(TimeSpan.FromSeconds(animationInSeconds));
  //    }


  //    rootElementBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
  //    rootElementBrush.BeginAnimation(SolidColorBrush.ColorProperty, hoverDown);

  //    IconBrush = rootElementBrush;
  //  }

  //  public void OnAnimateDefaultColor()
  //  {
  //    AnimateImageDefaultColor();
  //  }

  //  #endregion
  //}


  public class PathButton : ToggleButton
  {
    #region PathStyle

    public Style PathStyle
    {
      get { return (Style)GetValue(PathStyleProperty); }
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
      get { return (Brush)GetValue(PathCheckedBrushProperty); }
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
      get { return (Color)GetValue(IconHoverColorProperty); }
      set { SetValue(IconHoverColorProperty, value); }
    }

    public static readonly DependencyProperty IconHoverColorProperty =
      DependencyProperty.Register(
        nameof(IconHoverColor),
        typeof(Color),
        typeof(PathButton),
        new PropertyMetadata((Color)ColorConverter.ConvertFromString("#f0f8ff")));


    #endregion

    #region IconDefaultColor

    public Color IconDefaultColor
    {
      get { return (Color)GetValue(IconDefaultColorProperty); }
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
            if (y.NewValue is Color newColor && buttonWithIcon.Foreground is SolidColorBrush solidColorBrush)
            {
              if (solidColorBrush.Color != newColor)
              {
                buttonWithIcon.AnimateImageDefaultColor(0);
              }
            }
          }
        }));

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

      if (Foreground.IsFrozen)
      {
        rootElementBrush = Foreground.Clone();
      }
      else
      {
        rootElementBrush = Foreground;
      }

      hoverUp = new ColorAnimation();

      hoverUp.From = IconDefaultColor;
      hoverUp.To = IconHoverColor;

      hoverUp.Duration = new Duration(TimeSpan.FromSeconds(animationInSeconds));

      rootElementBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
      rootElementBrush.BeginAnimation(SolidColorBrush.ColorProperty, hoverUp);

      Foreground = rootElementBrush;
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

      if (Foreground.IsFrozen)
      {
        rootElementBrush = Foreground.Clone();
      }
      else
      {
        rootElementBrush = Foreground;
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

      Foreground = rootElementBrush;
    }

    public void OnAnimateDefaultColor()
    {
      AnimateImageDefaultColor();
    }

    #endregion
  }

  public class ButtonWithIcon : Button
  {
    #region IconPathStyle

    public Style IconPathStyle
    {
      get { return (Style)GetValue(IconPathStyleProperty); }
      set { SetValue(IconPathStyleProperty, value); }
    }

    public static readonly DependencyProperty IconPathStyleProperty =
      DependencyProperty.Register(
        nameof(IconPathStyle),
        typeof(Style),
        typeof(ButtonWithIcon),
        new PropertyMetadata((x,y) =>
        {

        }));


    #endregion

    #region IconHeight

    public double IconHeight
    {
      get { return (double)GetValue(IconHeightProperty); }
      set { SetValue(IconHeightProperty, value); }
    }

    public static readonly DependencyProperty IconHeightProperty =
      DependencyProperty.Register(
        nameof(IconHeight),
        typeof(double),
        typeof(ButtonWithIcon),
        new PropertyMetadata(25.0));

    #endregion

    #region IconWidth

    public double IconWidth
    {
      get { return (double)GetValue(IconWidthProperty); }
      set { SetValue(IconWidthProperty, value); }
    }

    public static readonly DependencyProperty IconWidthProperty =
      DependencyProperty.Register(
        nameof(IconWidth),
        typeof(double),
        typeof(ButtonWithIcon),
        new PropertyMetadata(25.0));

    #endregion

    #region IconMargin

    public Thickness IconMargin
    {
      get { return (Thickness)GetValue(IconMarginProperty); }
      set { SetValue(IconMarginProperty, value); }
    }

    public static readonly DependencyProperty IconMarginProperty =
      DependencyProperty.Register(
        nameof(IconMargin),
        typeof(Thickness),
        typeof(ButtonWithIcon),
        new PropertyMetadata(new Thickness(0)));

    #endregion

    #region IconHoverColor

    public Color IconHoverColor
    {
      get { return (Color)GetValue(IconHoverColorProperty); }
      set { SetValue(IconHoverColorProperty, value); }
    }

    public static readonly DependencyProperty IconHoverColorProperty =
      DependencyProperty.Register(
        nameof(IconHoverColor),
        typeof(Color),
        typeof(ButtonWithIcon),
        new PropertyMetadata((Color)ColorConverter.ConvertFromString("#f0f8ff")));


    #endregion

    #region IconDefaultColor

    public Color IconDefaultColor
    {
      get { return (Color)GetValue(IconDefaultColorProperty); }
      set { SetValue(IconDefaultColorProperty, value); }
    }

    public static readonly DependencyProperty IconDefaultColorProperty =
      DependencyProperty.Register(
        nameof(IconDefaultColor),
        typeof(Color),
        typeof(ButtonWithIcon),
        new PropertyMetadata(Colors.Transparent, (x, y) =>
        {
          if (x is ButtonWithIcon buttonWithIcon)
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

    #region IconDefaultBrush

    public Brush IconBrush
    {
      get { return (Brush)GetValue(IconBrushProperty); }
      set { SetValue(IconBrushProperty, value); }
    }

    public static readonly DependencyProperty IconBrushProperty =
      DependencyProperty.Register(
        nameof(IconBrush),
        typeof(Brush),
        typeof(ButtonWithIcon),
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
