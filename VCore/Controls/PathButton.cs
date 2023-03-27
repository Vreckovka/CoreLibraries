using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using VCore.WPF.Misc;

namespace VCore.WPF.Controls
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

    public PathButton()
    {

    }

    #region IconGridColumn

    public int IconGridColumn
    {
      get { return (int)GetValue(IconGridColumnProperty); }
      set { SetValue(IconGridColumnProperty, value); }
    }

    public static readonly DependencyProperty IconGridColumnProperty =
      DependencyProperty.Register(
        nameof(IconGridColumn),
        typeof(int),
        typeof(PathButton),
        new PropertyMetadata(0));


    #endregion

    #region ContentGridColumn

    public int ContentGridColumn
    {
      get { return (int)GetValue(ContentGridColumnProperty); }
      set { SetValue(ContentGridColumnProperty, value); }
    }

    public static readonly DependencyProperty ContentGridColumnProperty =
      DependencyProperty.Register(
        nameof(ContentGridColumn),
        typeof(int),
        typeof(PathButton),
        new PropertyMetadata(1));


    #endregion

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

    #region Glyph Properties

    #region Glyph

    public string Glyph
    {
      get { return (string)GetValue(GlyphProperty); }
      set { SetValue(GlyphProperty, value); }
    }

    public static readonly DependencyProperty GlyphProperty =
      DependencyProperty.Register(
        nameof(Glyph),
        typeof(string),
        typeof(PathButton),
        new PropertyMetadata(null));


    #endregion

    #region GlyphFontFamily

    public FontFamily GlyphFontFamily
    {
      get { return (FontFamily)GetValue(GlyphFontFamilyProperty); }
      set { SetValue(GlyphFontFamilyProperty, value); }
    }

    public static readonly DependencyProperty GlyphFontFamilyProperty =
      DependencyProperty.Register(
        nameof(GlyphFontFamily),
        typeof(FontFamily),
        typeof(PathButton),
        new PropertyMetadata(TextBlock.FontFamilyProperty.DefaultMetadata.DefaultValue));


    #endregion

    #region GlyphFontSize

    public double GlyphFontSize
    {
      get { return (double)GetValue(GlyphFontSizeProperty); }
      set { SetValue(GlyphFontSizeProperty, value); }
    }

    public static readonly DependencyProperty GlyphFontSizeProperty =
      DependencyProperty.Register(
        nameof(GlyphFontSize),
        typeof(double),
        typeof(PathButton),
        new PropertyMetadata(12.0));


    #endregion

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

    #region Icon properties

    #region Icon colors

    #region IconCheckedColor

    public Color IconCheckedColor
    {
      get { return (Color)GetValue(IconCheckedColorProperty); }
      set { SetValue(IconCheckedColorProperty, value); }
    }

    public static readonly DependencyProperty IconCheckedColorProperty =
      DependencyProperty.Register(
        nameof(IconCheckedColor),
        typeof(Color),
        typeof(PathButton),
        new PropertyMetadata(Colors.Red, (x, y) =>
        {
          if (x is PathButton pathButton && pathButton.IsChecked == true)
          {
            SetIconBrush(x, y);
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
        new PropertyMetadata((Color)ColorConverter.ConvertFromString("#f0f8ff"), (x, y) =>
        {
          if (x is PathButton pathButton && pathButton.IsMouseOver)
          {
            SetIconBrush(x, y);
          }
        }));


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
        new PropertyMetadata((Color)ColorConverter.ConvertFromString("#252525"), (x, y) =>
        {
          if (x is PathButton pathButton && pathButton.IsChecked != true)
          {
            SetIconBrush(x, y);
          }
        }));

    #endregion

    #region SetIconBrush

    private static void SetIconBrush(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs change)
    {
      if (dependencyObject is PathButton buttonWithIcon)
      {
        if (change.NewValue is Color newColor && buttonWithIcon.IconBrush is SolidColorBrush solidColorBrush)
        {
          if (solidColorBrush.Color != newColor)
          {
            if (buttonWithIcon.IsLoaded)
            {
              var brushClone = buttonWithIcon.GetAnimation((Color) change.OldValue, newColor);

              buttonWithIcon.IconBrush = brushClone;
            }
            else
            {
              buttonWithIcon.IconBrush = new SolidColorBrush(newColor);
            }
          }
        }
      }
    }

    #endregion

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

    [TypeConverter(typeof(LengthConverter))]
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

    [TypeConverter(typeof(LengthConverter))]
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


    #endregion

    #region EnableBorderAnimation

    public bool EnableBorderAnimation
    {
      get { return (bool)GetValue(EnableBorderAnimationProperty); }
      set { SetValue(EnableBorderAnimationProperty, value); }
    }

    public static readonly DependencyProperty EnableBorderAnimationProperty =
      DependencyProperty.Register(
        nameof(EnableBorderAnimation),
        typeof(bool),
        typeof(PathButton),
        new PropertyMetadata(false));


    #endregion

    #region Foreground

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
        new FrameworkPropertyMetadata(Colors.Red, (x, y) =>
        {
          if (x is PathButton pathButton && pathButton.IsChecked == true)
          {
            SetForeground(x, y);
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
        new PropertyMetadata((Color)ColorConverter.ConvertFromString("#ffffff"), (x, y) =>
        {
          if (x is PathButton pathButton && pathButton.IsMouseOver)
          {
            SetForeground(x, y);
          }
        }));


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
        new PropertyMetadata(Colors.White, SetForeground));

    #endregion

    #region SetForeground

    private static void SetForeground(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs change)
    {
      if (dependencyObject is PathButton buttonWithIcon)
      {
        if (change.NewValue is Color newColor && buttonWithIcon.Foreground is SolidColorBrush solidColorBrush)
        {
          if (solidColorBrush.Color != newColor)
          {
            if (buttonWithIcon.IsLoaded)
            {
              if (change.OldValue is Color oldColor)
              {
                var brushClone = buttonWithIcon.GetAnimation(oldColor, newColor);

                buttonWithIcon.Foreground = brushClone;
              }

            }
            else
            {
              buttonWithIcon.Foreground = new SolidColorBrush(newColor);
            }
          }
        }
      }
    }

    #endregion

    #endregion

    #region Border

    #region BorderCheckedColor

    public Color BorderCheckedColor
    {
      get { return (Color)GetValue(BorderCheckedColorProperty); }
      set { SetValue(BorderCheckedColorProperty, value); }
    }

    public static readonly DependencyProperty BorderCheckedColorProperty =
      DependencyProperty.Register(
        nameof(BorderCheckedColor),
        typeof(Color),
        typeof(PathButton),
        new FrameworkPropertyMetadata(Colors.Red, FrameworkPropertyMetadataOptions.AffectsRender, (x, y) =>
        {
          if (x is PathButton pathButton && pathButton.IsChecked == true)
          {
            var clone = pathButton.GetAnimation(pathButton.BorderCheckedColor, (Color)y.NewValue);

            pathButton.BorderBrush = clone;
          }
        }));


    #endregion

    #region BorderHoverColor

    public Color BorderHoverColor
    {
      get { return (Color)GetValue(BorderHoverColorProperty); }
      set { SetValue(BorderHoverColorProperty, value); }
    }

    public static readonly DependencyProperty BorderHoverColorProperty =
      DependencyProperty.Register(
        nameof(BorderHoverColor),
        typeof(Color),
        typeof(PathButton),
        new PropertyMetadata((Color)ColorConverter.ConvertFromString("#ffffff")));


    #endregion

    #region BorderDefaultColor

    public Color BorderDefaultColor
    {
      get { return (Color)GetValue(BorderDefaultColorProperty); }
      set { SetValue(BorderDefaultColorProperty, value); }
    }

    public static readonly DependencyProperty BorderDefaultColorProperty =
      DependencyProperty.Register(
        nameof(BorderDefaultColor),
        typeof(Color),
        typeof(PathButton),
        new PropertyMetadata(Colors.White, (x, y) =>
        {
          if (x is PathButton buttonWithIcon)
          {
            if (y.NewValue is Color newColor && buttonWithIcon.IconBrush is SolidColorBrush solidColorBrush)
            {
              //if (buttonWithIcon.IsChecked == false && buttonWithIcon.IsEnabled)
              {
                buttonWithIcon.BorderBrush = new SolidColorBrush(newColor);
              }
            }
          }
        }));

    #endregion

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
      if (IsChecked != true)
      {
        var iconBrushColone = GetAnimation(IconDefaultColor, IconHoverColor);
        var foregroundBrushColone = GetAnimation(ForegroundDefaultColor, ForegroundHoverColor);

        Brush borderBrushColone = null;

        if (EnableBorderAnimation && BorderBrush != null)
        {
          borderBrushColone = GetAnimation(BorderDefaultColor, BorderHoverColor);
        }


        SetBrushes(iconBrushColone, foregroundBrushColone, borderBrushColone);
      }
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
      if (IsChecked != true)
      {
        var iconBrushColone = GetAnimation(IconHoverColor, IconDefaultColor);
        var foregroundBrushColone = GetAnimation(ForegroundHoverColor, ForegroundDefaultColor);


        Brush borderBrushColone = null;

        if (EnableBorderAnimation && BorderBrush != null)
        {
          borderBrushColone = GetAnimation( BorderHoverColor, BorderDefaultColor);
        }


        SetBrushes(iconBrushColone, foregroundBrushColone, borderBrushColone);
      }
    }

    #endregion

    #endregion

    #region Methods

    #region OnToggle

    protected override void OnToggle()
    {
      if (!IsReadOnly)
      {
        base.OnToggle();
      }
    }

    #endregion

    #region OnChecked

    protected override void OnChecked(RoutedEventArgs e)
    {
      base.OnChecked(e);

      Brush iconColor = null;

      if (IsMouseOver)
      {
        iconColor = GetAnimation(IconHoverColor, IconCheckedColor);
      }
      else
      {
        iconColor = GetAnimation(IconDefaultColor, IconCheckedColor);
      }

    
      var foregroundBrushColone = GetAnimation(ForegroundDefaultColor, ForegroundCheckedColor);
      Brush borderBrushColone = null;

      if (EnableBorderAnimation && BorderBrush != null)
      {
        borderBrushColone = GetAnimation(BorderHoverColor, BorderCheckedColor);
      }


      SetBrushes(iconColor, foregroundBrushColone, borderBrushColone);
    }

    #endregion

    #region OnUnchecked

    protected override void OnUnchecked(RoutedEventArgs e)
    {
      base.OnUnchecked(e);

      var iconBrushColone = GetAnimation(IconCheckedColor, IconDefaultColor);
      var foregroundBrushColone = GetAnimation(ForegroundCheckedColor, ForegroundDefaultColor);


      Brush borderBrushColone = null;

      if (EnableBorderAnimation && BorderBrush != null)
      {
        borderBrushColone = GetAnimation(BorderCheckedColor, BorderDefaultColor);
      }


      SetBrushes(iconBrushColone, foregroundBrushColone, borderBrushColone);

    }

    #endregion

    #region GetAnimation

    private Brush GetAnimation(Color from, Color to)
    {
      var newElementBrush = new SolidColorBrush(to);

      var hoverUp = new ColorAnimation();

      hoverUp.From = from;
      hoverUp.To = to;

      hoverUp.Duration = new Duration(TimeSpan.FromSeconds(animationInSeconds));

      newElementBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
      newElementBrush.BeginAnimation(SolidColorBrush.ColorProperty, hoverUp);

      return newElementBrush;

    }

    #endregion

    #region SetBrushes

    private void SetBrushes(Brush iconBrush, Brush foregroundBrush, Brush borderBrush)
    {
      IconBrush = iconBrush;
      Foreground = foregroundBrush;

      if (EnableBorderAnimation)
        BorderBrush = borderBrush;
    }

    #endregion

    #region OnMouseLeftButtonDown

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);

      AnimateImageDefaultColor();

    }

    #endregion

    #region OnAnimateDefaultColor

    public void OnAnimateDefaultColor()
    {
      AnimateImageDefaultColor();
    }

    #endregion 

    #endregion

  }
}
