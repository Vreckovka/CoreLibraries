using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VCore.AttachedProperties
{
  public class AttachedProperty
  {
    #region RememberInitialWidth

    public static readonly DependencyProperty RememberInitialWidthProperty = DependencyProperty.RegisterAttached(
      "RememberInitialWidth",
      typeof(bool),
      typeof(AttachedProperty),
      new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, OnRememberInitialSize)
    );

    private static void OnRememberInitialSize(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
      if ((bool) dependencyPropertyChangedEventArgs.NewValue)
      {
        ((FrameworkElement) dependencyObject).Loaded += AttachedProperty_Loaded;
      }
    }

    private static void AttachedProperty_Loaded(object sender, RoutedEventArgs e)
    {
      var frameworkElement = (FrameworkElement) sender;
      var isBubbleSource = GetRememberInitialWidth(frameworkElement);
      if (isBubbleSource)
      {
        frameworkElement.Width = frameworkElement.ActualWidth;
      }
    }

    public static void SetRememberInitialWidth(UIElement element, bool value)
    {
      element.SetValue(RememberInitialWidthProperty, value);
    }

    public static bool GetRememberInitialWidth(UIElement element)
    {
      return (bool) element.GetValue(RememberInitialWidthProperty);
    }

    #endregion

    #region ScrollSpeed

    public static double GetScrollSpeed(DependencyObject obj)
    {
      return (double) obj.GetValue(ScrollSpeedProperty);
    }

    public static void SetScrollSpeed(DependencyObject obj, double value)
    {
      obj.SetValue(ScrollSpeedProperty, value);
    }

    public static readonly DependencyProperty ScrollSpeedProperty =
      DependencyProperty.RegisterAttached(
        "ScrollSpeed",
        typeof(double),
        typeof(AttachedProperty),
        new FrameworkPropertyMetadata(
          1.0,
          FrameworkPropertyMetadataOptions.Inherits & FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
          new PropertyChangedCallback(OnScrollSpeedChanged)));

    public static DependencyObject GetScrollViewer(DependencyObject o)
    {
      // Return the DependencyObject if it is a ScrollViewer
      if (o is ScrollViewer)
      {
        return o;
      }

      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
      {
        var child = VisualTreeHelper.GetChild(o, i);

        var result = GetScrollViewer(child);
        if (result == null)
        {
          continue;
        }
        else
        {
          return result;
        }
      }

      return null;
    }

    private static void OnScrollSpeedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      var host = o as UIElement;
      host.PreviewMouseWheel += new MouseWheelEventHandler(OnPreviewMouseWheelScrolled);
    }

    private static void OnPreviewMouseWheelScrolled(object sender, MouseWheelEventArgs e)
    {
      DependencyObject scrollHost = sender as DependencyObject;

      double scrollSpeed = (double) (scrollHost).GetValue(AttachedProperty.ScrollSpeedProperty);

      ScrollViewer scrollViewer = GetScrollViewer(scrollHost) as ScrollViewer;

      if (scrollViewer != null)
      {
        double offset = scrollViewer.VerticalOffset - (e.Delta * scrollSpeed / 6);
        if (offset < 0)
        {
          scrollViewer.ScrollToVerticalOffset(0);
        }
        else if (offset > scrollViewer.ExtentHeight)
        {
          scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);
        }
        else
        {
          scrollViewer.ScrollToVerticalOffset(offset);
        }

        e.Handled = true;
      }
      else
      {
        throw new NotSupportedException("ScrollSpeed Attached Property is not attached to an element containing a ScrollViewer.");
      }
    }

    #endregion

  }
}
