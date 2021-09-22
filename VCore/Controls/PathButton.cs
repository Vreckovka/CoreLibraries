using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace VCore.Controls
{
  public class FontAwesomePathButton : PathButton
  {
    #region IconName

    public string IconName
    {
      get { return (string)GetValue(IconNameProperty); }
      set { SetValue(IconNameProperty, value); }
    }

    public static readonly DependencyProperty IconNameProperty =
      DependencyProperty.Register(
        nameof(IconName),
        typeof(string),
        typeof(FontAwesomePathButton),
        new PropertyMetadata(null));


    #endregion

    #region IconType

    public string IconType
    {
      get { return (string)GetValue(IconTypeProperty); }
      set { SetValue(IconTypeProperty, value); }
    }

    public static readonly DependencyProperty IconTypeProperty =
      DependencyProperty.Register(
        nameof(IconType),
        typeof(string),
        typeof(FontAwesomePathButton),
        new PropertyMetadata("regular"));


    #endregion
  }

  public class PathButton : ToggleButton
  {
    private double animationInSeconds = .15;

    static PathButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PathButton), new FrameworkPropertyMetadata(typeof(PathButton)));
    }

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

    #region HorizontalIconAlignment

    public HorizontalAlignment HorizontalIconAlignment
    {
      get { return (HorizontalAlignment)GetValue(HorizontalIconAlignmentProperty); }
      set { SetValue(HorizontalIconAlignmentProperty, value); }
    }

    public static readonly DependencyProperty HorizontalIconAlignmentProperty =
      DependencyProperty.Register(
        nameof(HorizontalIconAlignment),
        typeof(HorizontalAlignment),
        typeof(PathButton),
        new PropertyMetadata(HorizontalAlignment.Center));


    #endregion

    #region VerticalIconAlignment

    public VerticalAlignment VerticalIconAlignment
    {
      get { return (VerticalAlignment)GetValue(VerticalIconAlignmentProperty); }
      set { SetValue(VerticalIconAlignmentProperty, value); }
    }

    public static readonly DependencyProperty VerticalIconAlignmentProperty =
      DependencyProperty.Register(
        nameof(VerticalIconAlignment),
        typeof(VerticalAlignment),
        typeof(PathButton),
        new PropertyMetadata(VerticalAlignment.Center));


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
        typeof(PathButton),
        new PropertyMetadata(new Thickness(0)));


    #endregion

    #region CornerRadius

    public CornerRadius CornerRadius
    {
      get { return (CornerRadius)GetValue(CornerRadiusProperty); }
      set { SetValue(CornerRadiusProperty, value); }
    }

    public static readonly DependencyProperty CornerRadiusProperty =
      DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(PathButton),
        new PropertyMetadata(new CornerRadius(0)));


    #endregion

    #region IconStretch

    public Stretch IconStretch
    {
      get { return (Stretch)GetValue(IconStretchProperty); }
      set { SetValue(IconStretchProperty, value); }
    }

    public static readonly DependencyProperty IconStretchProperty =
      DependencyProperty.Register(
        nameof(IconStretch),
        typeof(Stretch),
        typeof(PathButton),
        new PropertyMetadata(Stretch.Fill));


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
        typeof(PathButton),
        new PropertyMetadata(20.0));


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
        typeof(PathButton),
        new PropertyMetadata(20.0));


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

    #region ForegroundCheckedColor

    public Color ForegroundCheckedColor
    {
      get { return (Color)GetValue(ForegroundCheckedColorProperty); }
      set { SetValue(ForegroundCheckedColorProperty, value); }
    }

    public static readonly DependencyProperty ForegroundCheckedColorProperty =
      DependencyProperty.Register(
        nameof(ForegroundCheckedColor),
        typeof(Color),
        typeof(PathButton),
        new FrameworkPropertyMetadata(Colors.Red, FrameworkPropertyMetadataOptions.AffectsRender,(x, y) =>
        {
          if(x is PathButton pathButton && pathButton.IsChecked == true)
          {
            var clone = pathButton.Foreground.Clone();

            clone = pathButton.GetAnimation(pathButton.Foreground, pathButton.ForegroundDefaultColor, (Color)y.NewValue);

            pathButton.Foreground = clone;
          }
        }));


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
        new PropertyMetadata(Colors.Green, (x, y) =>
        {
          if (x is PathButton buttonWithIcon)
          {
            if (y.NewValue is Color newColor && buttonWithIcon.IconBrush is SolidColorBrush solidColorBrush)
            {
              if (solidColorBrush.Color != newColor)
              {
                buttonWithIcon.IconBrush = new SolidColorBrush(newColor);
              }
            }
          }
        }));

    #endregion

    #region ForegroundHoverColor

    public Color ForegroundHoverColor
    {
      get { return (Color)GetValue(ForegroundHoverColorProperty); }
      set { SetValue(ForegroundHoverColorProperty, value); }
    }

    public static readonly DependencyProperty ForegroundHoverColorProperty =
      DependencyProperty.Register(
        nameof(ForegroundHoverColor),
        typeof(Color),
        typeof(PathButton),
        new PropertyMetadata((Color)ColorConverter.ConvertFromString("#ffffff")));


    #endregion

    #region ForegroundDefaultColor

    public Color ForegroundDefaultColor
    {
      get { return (Color)GetValue(ForegroundDefaultColorProperty); }
      set { SetValue(ForegroundDefaultColorProperty, value); }
    }

    public static readonly DependencyProperty ForegroundDefaultColorProperty =
      DependencyProperty.Register(
        nameof(ForegroundDefaultColor),
        typeof(Color),
        typeof(PathButton),
        new PropertyMetadata(Colors.White, (x, y) =>
         {
           if (x is PathButton buttonWithIcon)
           {
             if (y.NewValue is Color newColor && buttonWithIcon.IconBrush is SolidColorBrush solidColorBrush)
             {
               if (solidColorBrush.Color != newColor && 
                   buttonWithIcon.IsChecked == false && 
                   buttonWithIcon.IsEnabled)
               {
                 buttonWithIcon.Foreground = new SolidColorBrush(newColor);
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

    #region IsReadOnly

    public bool IsReadOnly
    {
      get { return (bool)GetValue(IsReadOnlyProperty); }
      set { SetValue(IsReadOnlyProperty, value); }
    }

    public static readonly DependencyProperty IsReadOnlyProperty =
      DependencyProperty.Register(nameof(IsReadOnly), typeof(bool),
        typeof(PathButton), new UIPropertyMetadata(false));

    #endregion

    #region Commands

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
      Brush iconBrushColone = IconBrush.Clone();

      iconBrushColone = GetAnimation(iconBrushColone, IconDefaultColor, IconHoverColor);

      Brush foregroundBrushColone = Foreground.Clone();

      foregroundBrushColone = GetAnimation(foregroundBrushColone, ForegroundDefaultColor, ForegroundHoverColor);

      SetBrushes(iconBrushColone, foregroundBrushColone, foregroundBrushColone);
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
      Brush iconBrushColone = IconBrush.Clone();

      iconBrushColone = GetAnimation(iconBrushColone, IconHoverColor, IconDefaultColor);

      Brush foregroundBrushColone = Foreground.Clone();

      foregroundBrushColone = GetAnimation(foregroundBrushColone, ForegroundHoverColor, ForegroundDefaultColor);

      SetBrushes(iconBrushColone, foregroundBrushColone, foregroundBrushColone);
    }

    #endregion

    #endregion

    #region Methods

    protected override void OnToggle()
    {
      if (!IsReadOnly)
      {
        base.OnToggle();
      }
    }

    #region OnChecked

    protected override void OnChecked(RoutedEventArgs e)
    {
      base.OnChecked(e);

      Foreground = Foreground.Clone();

      Foreground.BeginAnimation(SolidColorBrush.ColorProperty, null);

      Foreground = GetAnimation(Foreground, ForegroundDefaultColor, ForegroundCheckedColor);
    }

    #endregion

    #region OnUnchecked

    protected override void OnUnchecked(RoutedEventArgs e)
    {
      base.OnUnchecked(e);

      Foreground = Foreground.Clone();

      Foreground.BeginAnimation(SolidColorBrush.ColorProperty, null);

      Foreground = GetAnimation(Foreground, ForegroundCheckedColor, ForegroundDefaultColor);

    }

    #endregion

    #region GetAnimation

    private Brush GetAnimation(Brush rootElementBrush, Color from, Color to)
    {
      rootElementBrush = new SolidColorBrush(to);

      var hoverUp = new ColorAnimation();

      hoverUp.From = from;
      hoverUp.To = to;

      hoverUp.Duration = new Duration(TimeSpan.FromSeconds(animationInSeconds));

      rootElementBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
      rootElementBrush.BeginAnimation(SolidColorBrush.ColorProperty, hoverUp);

      return rootElementBrush;
    }

    #endregion

    #region SetBrushes

    private void SetBrushes(Brush iconBrush, Brush foregroundBrush, Brush borderBrush)
    {
      IconBrush = iconBrush;

      if (IsChecked.HasValue && !IsChecked.Value)
        Foreground = foregroundBrush;
      //BorderBrush = borderBrush;
    }

    #endregion

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);

      AnimateImageDefaultColor();

    }

    #region OnAnimateDefaultColor

    public void OnAnimateDefaultColor()
    {
      AnimateImageDefaultColor();
    }

    #endregion 

    #endregion

  }
}
